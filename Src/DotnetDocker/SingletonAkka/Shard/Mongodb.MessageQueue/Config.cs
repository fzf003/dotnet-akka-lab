using Mongodb.MessageQueue.Internals;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mongodb.MessageQueue
{
    public class Config
    {
      
        public const int DefaultMessageLeaseSeconds = 60;

        
        public const int DefaultMaxParallelism = 20;

        
        public const int DefaultMaxDeliveryAttempts = 5;

        
        public Config(MongoUrl mongoUrl, string collectionName, int defaultMessageLeaseSeconds = DefaultMessageLeaseSeconds, int maxParallelism = DefaultMaxParallelism, int maxDeliveryAttempts = DefaultMaxDeliveryAttempts)
            : this(mongoUrl.GetMongoDatabase(), collectionName, defaultMessageLeaseSeconds, maxParallelism, maxDeliveryAttempts)
        {
        }
 
        public Config(string connectionString, string collectionName, int defaultMessageLeaseSeconds = DefaultMessageLeaseSeconds, int maxParallelism = DefaultMaxParallelism, int maxDeliveryAttempts = DefaultMaxDeliveryAttempts)
            : this(connectionString.GetMongoDatabase(), collectionName, defaultMessageLeaseSeconds, maxParallelism, maxDeliveryAttempts)
        {
        }

        
        public Config(IMongoDatabase database, string collectionName, int defaultMessageLeaseSeconds = DefaultMessageLeaseSeconds, int maxParallelism = DefaultMaxParallelism, int maxDeliveryAttempts = DefaultMaxDeliveryAttempts)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            if (defaultMessageLeaseSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(defaultMessageLeaseSeconds), defaultMessageLeaseSeconds, "Please specify a positive number of seconds for the lease duration");
            }

            if (maxDeliveryAttempts <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxDeliveryAttempts), maxDeliveryAttempts, "Please specify a positions number for the number of delivery attempts to accept for each message");
            }

            MaxParallelism = maxParallelism;
            Collection = InitializeCollection(collectionName, database);
            DefaultMessageLease = TimeSpan.FromSeconds(defaultMessageLeaseSeconds);
            MaxDeliveryAttempts = maxDeliveryAttempts;
        }

        static IMongoCollection<BsonDocument> InitializeCollection(string collectionName, IMongoDatabase mongoDatabase)
        {
            var collection = mongoDatabase.GetCollection<BsonDocument>(collectionName);
            var index = new BsonDocument
            {
                {Fields.DestinationQueueName, 1},
                {Fields.ReceiveTime, 1},
                {Fields.DeliveryAttempts, 1},
            };
            collection.Indexes.CreateOne(new BsonDocumentIndexKeysDefinition<BsonDocument>(index));
            return collection;
        }
 
        public Producer CreateProducer() => new Producer(this);

        
        public Consumer CreateConsumer(string queueName) => new Consumer(this, queueName);

        internal IMongoCollection<BsonDocument> Collection { get; }

        internal int MaxParallelism { get; }

        internal int MaxDeliveryAttempts { get; }

        internal TimeSpan DefaultMessageLease { get; }
    }
}
