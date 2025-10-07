using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Rokys.Events.Command.Interfaces.Events;

namespace Rokys.Audit.Subscription.Hub.Events
{
    public class EventWrapper
    {
        [JsonProperty("EventId")]
        public Guid EventId { get; set; }

        [JsonProperty("EventName")]
        public string EventName { get; set; } = string.Empty;

        [JsonProperty("Version")]
        public int Version { get; set; }

        [JsonProperty("OccurredOn")]
        public DateTime OccurredOn { get; set; }

        [JsonProperty("Data")]
        public IDomainEvent Data { get; set; } = null!;
    }

    /// <summary>
    /// Wrapper genérico para eventos tipados
    /// </summary>
    /// <typeparam name="T">Tipo del evento</typeparam>
    public class EventWrapper<T> where T : IDomainEvent
    {
        [JsonProperty("EventId")]
        public Guid EventId { get; set; }

        [JsonProperty("EventName")]
        public string EventName { get; set; } = string.Empty;

        [JsonProperty("Version")]
        public int Version { get; set; }

        [JsonProperty("OccurredOn")]
        public DateTime OccurredOn { get; set; }

        [JsonProperty("Data")]
        public T Data { get; set; } = default!;
    }
}