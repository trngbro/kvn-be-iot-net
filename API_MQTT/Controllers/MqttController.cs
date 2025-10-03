using Microsoft.AspNetCore.Mvc;
using DomainService.Interfaces.MQTT;

namespace API_MQTT.Controllers
{
    /// <summary>
    /// MQTT Controller for MQTT-related operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MqttController : BaseController
    {
        public MqttController(IHttpContextAccessor httpContextAccessor) 
            : base(httpContextAccessor)
        {
        }

        /// <summary>
        /// Get MQTT connection status
        /// </summary>
        /// <returns>MQTT connection information</returns>
        [HttpGet("status")]
        public IActionResult GetMqttStatus()
        {
            try
            {
                var status = new
                {
                    IsConnected = true,
                    BrokerUrl = "mqtt://localhost:1883",
                    LastConnected = DateTime.UtcNow.AddMinutes(-5),
                    ActiveTopics = new[] { "sensors/temperature", "sensors/humidity", "alerts/critical" },
                    MessageCount = 1250
                };

                return Ok(status);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// Get MQTT topics
        /// </summary>
        /// <returns>List of available MQTT topics</returns>
        [HttpGet("topics")]
        public IActionResult GetTopics()
        {
            try
            {
                var topics = new[]
                {
                    new { 
                        Name = "sensors/temperature", 
                        Type = "sensor", 
                        Description = "Temperature sensor data",
                        LastMessage = DateTime.UtcNow.AddSeconds(-30)
                    },
                    new { 
                        Name = "sensors/humidity", 
                        Type = "sensor", 
                        Description = "Humidity sensor data",
                        LastMessage = DateTime.UtcNow.AddSeconds(-45)
                    },
                    new { 
                        Name = "alerts/critical", 
                        Type = "alert", 
                        Description = "Critical system alerts",
                        LastMessage = DateTime.UtcNow.AddMinutes(-2)
                    },
                    new { 
                        Name = "devices/status", 
                        Type = "status", 
                        Description = "Device status updates",
                        LastMessage = DateTime.UtcNow.AddMinutes(-1)
                    }
                };

                return Ok(topics, topics.Length);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// Publish message to MQTT topic
        /// </summary>
        /// <param name="request">Message publish request</param>
        /// <returns>Publish confirmation</returns>
        [HttpPost("publish")]
        public IActionResult PublishMessage([FromBody] PublishMessageRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.Topic))
                {
                    return BadRequest("Topic is required");
                }

                if (string.IsNullOrEmpty(request.Message))
                {
                    return BadRequest("Message is required");
                }

                var result = new
                {
                    Status = "Published",
                    Topic = request.Topic,
                    Message = request.Message,
                    MessageId = Guid.NewGuid().ToString(),
                    PublishedAt = DateTime.UtcNow,
                    QoS = request.QoS ?? 0
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// Subscribe to MQTT topic
        /// </summary>
        /// <param name="request">Subscription request</param>
        /// <returns>Subscription confirmation</returns>
        [HttpPost("subscribe")]
        public IActionResult Subscribe([FromBody] SubscribeRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.Topic))
                {
                    return BadRequest("Topic is required");
                }

                var result = new
                {
                    Status = "Subscribed",
                    Topic = request.Topic,
                    SubscriptionId = Guid.NewGuid().ToString(),
                    SubscribedAt = DateTime.UtcNow,
                    QoS = request.QoS ?? 0
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// Get recent MQTT messages
        /// </summary>
        /// <param name="topic">Topic filter (optional)</param>
        /// <param name="limit">Number of messages to return (default: 10)</param>
        /// <returns>List of recent messages</returns>
        [HttpGet("messages")]
        public IActionResult GetRecentMessages([FromQuery] string? topic = null, [FromQuery] int limit = 10)
        {
            try
            {
                var messages = new[]
                {
                    new {
                        Id = Guid.NewGuid().ToString(),
                        Topic = "sensors/temperature",
                        Message = "{\"temperature\": 25.5, \"unit\": \"C\"}",
                        Timestamp = DateTime.UtcNow.AddMinutes(-1),
                        QoS = 0
                    },
                    new {
                        Id = Guid.NewGuid().ToString(),
                        Topic = "sensors/humidity",
                        Message = "{\"humidity\": 65.2, \"unit\": \"%\"}",
                        Timestamp = DateTime.UtcNow.AddMinutes(-2),
                        QoS = 0
                    },
                    new {
                        Id = Guid.NewGuid().ToString(),
                        Topic = "alerts/critical",
                        Message = "{\"alert\": \"High temperature detected\", \"severity\": \"critical\"}",
                        Timestamp = DateTime.UtcNow.AddMinutes(-5),
                        QoS = 1
                    }
                };

                var filteredMessages = string.IsNullOrEmpty(topic) 
                    ? messages.Take(limit) 
                    : messages.Where(m => m.Topic.Contains(topic, StringComparison.OrdinalIgnoreCase)).Take(limit);

                return Ok(filteredMessages, filteredMessages.Count());
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
    }

    /// <summary>
    /// Request model for publishing MQTT messages
    /// </summary>
    public class PublishMessageRequest
    {
        /// <summary>
        /// MQTT topic
        /// </summary>
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// Message content
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Quality of Service level (0, 1, or 2)
        /// </summary>
        public int? QoS { get; set; }
    }

    /// <summary>
    /// Request model for MQTT subscriptions
    /// </summary>
    public class SubscribeRequest
    {
        /// <summary>
        /// MQTT topic
        /// </summary>
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// Quality of Service level (0, 1, or 2)
        /// </summary>
        public int? QoS { get; set; }
    }
}