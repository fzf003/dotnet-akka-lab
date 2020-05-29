using Mongodb.MessageQueue.Internals;
using Mongodb.MessageQueue.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mongodb.MessageQueue
{
    public class Consumer
    {
        readonly Config _config;
        readonly SemaphoreSlim _semaphore;

        
        public string QueueName { get; }

      
        public Consumer(Config config, string queueName)
        {
            _config = config;
            QueueName = queueName;
            _semaphore = new SemaphoreSlim(_config.MaxParallelism, _config.MaxParallelism);
        }

       
        public async Task Ack(string messageId)
        {
            var collection = _config.Collection;

            await _semaphore.WaitAsync();

            try
            {
                var deleteResult = await collection.DeleteOneAsync(doc => doc["_id"] == messageId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

      
        public async Task Nack(string messageId)
        {
            var collection = _config.Collection;

            var abandonUpdate = new BsonDocument
            {
                {"$set", new BsonDocument {{Fields.ReceiveTime, DateTime.MinValue}}}
            };

            await _semaphore.WaitAsync();

            try
            {
                await collection.UpdateOneAsync(doc => doc["_id"] == messageId, new BsonDocumentUpdateDefinition<BsonDocument>(abandonUpdate));
            }
            catch
            {
                 
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Renew(string messageId)
        {
            var collection = _config.Collection;

            var renewUpdate = new BsonDocument
            {
                {"$set", new BsonDocument {{Fields.ReceiveTime, DateTime.Now}}}
            };

            await _semaphore.WaitAsync();

            try
            {
                await collection.UpdateOneAsync(doc => doc["_id"] == messageId, new BsonDocumentUpdateDefinition<BsonDocument>(renewUpdate));
            }
            catch
            {
                 
            }
            finally
            {
                _semaphore.Release();
            }
        }

 
        public async Task<bool> Exists(string messageId)
        {
            var collection = _config.Collection;

            var criteria = new BsonDocument
            {
                {"_id", messageId }
            };

            await _semaphore.WaitAsync();

            try
            {
                return await collection.CountAsync(new BsonDocumentFilterDefinition<BsonDocument>(criteria)) > 0;
            }
            finally
            {
                _semaphore.Release();
            }
        }

      
        public async Task<ReceivedMessage> LoadAsync(string messageId)
        {
            if (messageId == null) throw new ArgumentNullException(nameof(messageId));

            var collection = _config.Collection;

            await _semaphore.WaitAsync();

            try
            {
                using (var cursor = await collection.FindAsync(d => d["_id"] == messageId))
                {
                    var document = await cursor.FirstOrDefaultAsync();

                    if (document == null) return null;

                    return GetReceivedMessage(document);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        
        public async Task<ReceivedMessage> GetNextAsync()
        {
            var now = DateTime.UtcNow;

            var receiveTimeCriteria = new BsonDocument { { "$lt", now.Subtract(_config.DefaultMessageLease) } };

            var filter = new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument
            {
                {Fields.DestinationQueueName, QueueName},
                {Fields.ReceiveTime, receiveTimeCriteria},
                {Fields.DeliveryAttempts, new BsonDocument {{"$lt", _config.MaxDeliveryAttempts}}},
            });

            var update = new BsonDocumentUpdateDefinition<BsonDocument>(new BsonDocument
            {
                {"$set", new BsonDocument {{Fields.ReceiveTime, now}}},
                {"$inc", new BsonDocument {{Fields.DeliveryAttempts, 1}}}
            });

            var options = new FindOneAndUpdateOptions<BsonDocument>
            {
                ReturnDocument = ReturnDocument.After
            };

            var collection = _config.Collection;

            await _semaphore.WaitAsync();

            try
            {
                var document = await collection.FindOneAndUpdateAsync(filter, update, options);

                if (document == null) return null;

                return GetReceivedMessage(document);

            }
            finally
            {
                _semaphore.Release();
            }
        }

        ReceivedMessage GetReceivedMessage(BsonValue document)
        {
            try
            {
                var body = document[Fields.Body].AsByteArray;

                var headers = document[Fields.Headers].AsBsonArray
                    .ToDictionary(value => value[Fields.Key].AsString, value => value[Fields.Value].AsString);

                var id = document["_id"].AsString;

                var deliveryCount = document[Fields.DeliveryAttempts].AsInt32;

                var message = new ReceivedMessage(
                    headers: headers, body: body,
                    ack: () => Ack(id),
                    nack: () => Nack(id),
                    renew: () => Renew(id),
                    deliveryCount: deliveryCount
                );
                return message;
            }
            catch (Exception exception)
            {
                throw new FormatException($"无法读取该文档: {document}", exception);
            }
        }
    }
}
