using Akka.Streams.Dsl;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Config = Mongodb.MessageQueue.Config;
using System.Linq;
using Akka.Streams.IO;
using Akka.IO;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ShardNode;
using Akka;
using System.Net.Http;
using MongoDB.Driver;
using Mongodb.MessageQueue.Model;
using ShardNode;
namespace ReactiveMessageQueue
{
    class Program
    {
        const string queueName = "fzf003";
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static Source<TOut, NotUsed> RestartSourceFactory<TOut, TMat>(Func<Source<TOut, TMat>> flowFactory, TimeSpan minBackoff, TimeSpan maxBackoff, double randomFactor, int maxRestarts, bool onlyOnFailures)
        {
            return onlyOnFailures
                ? RestartSource.OnFailuresWithBackoff(flowFactory, minBackoff, maxBackoff, randomFactor, maxRestarts)
                : RestartSource.WithBackoff(flowFactory, minBackoff, maxBackoff, randomFactor, maxRestarts);
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .UseAkka(action: (services) =>
                       {
                           services.AddHostedService<Worker>();
                           services.AddHttpClient<IServiceGateway, DemoServiceGateway>(client =>
                           {
                               client.BaseAddress = new Uri("http://v1.jinrishici.com/");
                           });
                       });
                       
        }

        public static Sink<string, Task<IOResult>> LineSink(string filename)
        {
            return Flow.Create<string>()
                      .Select(s => ByteString.FromString($"{s}\n"))
                      .ToMaterialized(FileIO.ToFile(new FileInfo(filename)), Keep.Right);
        }

        static void Start()
        {
            Config config = new Config(MongoUrl.Create("mongodb://localhost:27017/queue"), "Messages", maxDeliveryAttempts: 5);

            var producer = config.CreateProducer();

            var consumer = config.CreateConsumer(queueName);

            var ReceivedSource = Observable.Create<ReceivedMessage>(observer =>
            {
                CompositeDisposable disposables = new CompositeDisposable();

                disposables.Add(NewThreadScheduler.Default.ScheduleLongRunning(async cancel =>
                {
                    while (!cancel.IsDisposed)
                    {
                        var message = await consumer.GetNextAsync();
                        if (message == null)
                        {
                            await Task.Delay(100);
                            // Console.WriteLine("*********");
                            continue;
                        }

                        observer.OnNext(message);
                        await Task.Delay(100);
                        Console.WriteLine("........");
                    }
                }));

                return disposables;

            });





            var OutDataStream = ReceivedSource
                                          .Where(p => p != null)
                                          .Do(value =>
                                          {
                                              Console.ForegroundColor = ConsoleColor.Green;
                                              if (value != null)
                                              {
                                                  Console.WriteLine("Message: {0}--{1}", Encoding.UTF8.GetString(value?.Body), value?.DeliveryCount);
                                              }
                                              else
                                              {
                                                  Console.WriteLine("没有数据......");
                                              }
                                              Console.ResetColor();
                                          }, ex =>
                                          {
                                              Console.ForegroundColor = ConsoleColor.Red;
                                              Console.WriteLine("Error: {0}", ex.Message);
                                              Console.ResetColor();
                                          })

                                          //.Timeout(TimeSpan.FromSeconds(1))
                                          .Catch<ReceivedMessage, TimeoutException>(ex =>
                                          {
                                              Console.WriteLine("TimeoutException:{0}", ex.Message);
                                              return Observable.Return<ReceivedMessage>(null);
                                          }).Catch<ReceivedMessage, Exception>(ex =>
                                          {
                                              Console.WriteLine("ex:{0}", ex.Message);
                                              return Observable.Return<ReceivedMessage>(null);
                                          }).Where(p => p != null);

            OutDataStream.Subscribe(async p =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(p.Body);
                    Console.WriteLine(message);
                    await p.Ack();

                }
                catch (Exception ex)
                {
                    await p.Nack();
                    Console.WriteLine("Sub:{0}", ex.Message);
                }
            }, ex =>
            {
                Console.WriteLine("mYex:{0}", ex.Message);
            }, () =>
            {
                Console.WriteLine("完成...");
            });


            for (; ; )
            {

                //await producer.SendAsync(queueName, new Mongodb.MessageQueue.Model.Message(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N"))));
                Console.WriteLine("+++++++++++++++++++++++++++++++");
                Console.ReadKey();
            }
        }
    }
}
