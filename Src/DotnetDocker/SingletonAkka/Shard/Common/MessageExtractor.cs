﻿using Akka.Cluster.Sharding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public  class ShardEnvelope
    {
        public readonly string EntityId;
        public readonly object Payload;

        public ShardEnvelope(string entityId, object payload)
        {
            EntityId = entityId;
            Payload = payload;
        }
    }

    public sealed class MessageExtractor : HashCodeMessageExtractor
    {
        public MessageExtractor(int maxNumberOfShards) : base(maxNumberOfShards) { }

        public override string EntityId(object message) => (message as ShardEnvelope)?.EntityId;

        public override object EntityMessage(object message) => (message as ShardEnvelope)?.Payload;
    }
}
