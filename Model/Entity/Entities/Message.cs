using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entity.Entities
{
    public class Message : BaseEntity
    {
        public Guid MessageId { get; set; }

        public string? Content { get; set; }

        public Guid SenderId { get; set; } = Guid.Empty;

        public Guid ReceiverId { get; set; } = Guid.Empty;

        public bool IsBotReply { get; set; } = false;
    }

    public class MessagePayload
    {
        public List<MessageMapping>? messages { get; set; }

        public double temperature { get; set; } = 0.7;

        public double top_p { get; set; } = 0.95;

        public int max_tokens { get; set; } = 800;

    }

    public class MessageMapping
    {

        public string? role { get; set; } = "system";

        public MessageMappingContent? content { get; set; }
    }

    public class MessageMappingContent
    {

        public string type { get; set; } = "text";

        public string? text { get; set; }
    }
}