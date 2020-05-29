using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mongodb.MessageQueue.Internals
{
    static class MongoExtensions
    {
        public static IMongoDatabase GetMongoDatabase(this MongoUrl mongoUrl)
        {
            var databaseName = mongoUrl.DatabaseName;

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException($"MongoDB URL不包含数据库名称!");
            }

            return new MongoClient(mongoUrl).GetDatabase(databaseName);
        }

        public static IMongoDatabase GetMongoDatabase(this string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

            return new MongoUrl(connectionString).GetMongoDatabase();
        }
    }
}
