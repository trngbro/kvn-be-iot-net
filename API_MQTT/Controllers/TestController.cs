using Microsoft.AspNetCore.Mvc;
using DomainService.Interfaces.MQTT;
using Common.Constant;

namespace API_MQTT.Controllers
{
    /// <summary>
    /// Test API Controller for basic functionality testing
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : BaseController
    {
        public TestController(IHttpContextAccessor httpContextAccessoruserService) 
            : base(httpContextAccessoruserService)
        {
        }

        /// <summary>
        /// Get API health status
        /// </summary>
        /// <returns>API status information</returns>
        [HttpGet("health")]
        public IActionResult GetHealth()
        {
            try
            {
                var result = new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Version = "1.0.0",
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// Get all test data
        /// </summary>
        /// <returns>List of test items</returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var testData = new[]
                {
                    new { Id = 1, Name = "Test Item 1", Description = "First test item" },
                    new { Id = 2, Name = "Test Item 2", Description = "Second test item" },
                    new { Id = 3, Name = "Test Item 3", Description = "Third test item" }
                };

                return Ok(testData, testData.Length);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// Get test item by ID
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <returns>Test item details</returns>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid ID provided");
                }

                var testItem = new
                {
                    Id = id,
                    Name = $"Test Item {id}",
                    Description = $"Test item with ID {id}",
                    CreatedAt = DateTime.UtcNow.AddDays(-id)
                };

                return Ok(testItem);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// Create a new test item
        /// </summary>
        /// <param name="request">Test item data</param>
        /// <returns>Created test item</returns>
        [HttpPost]
        public IActionResult Create([FromBody] CreateTestItemRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.Name))
                {
                    return BadRequest("Name is required");
                }

                var createdItem = new
                {
                    Id = new Random().Next(1000, 9999),
                    Name = request.Name,
                    Description = request.Description ?? "No description provided",
                    CreatedAt = DateTime.UtcNow
                };

                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// Update an existing test item
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <param name="request">Updated item data</param>
        /// <returns>Updated test item</returns>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateTestItemRequest request)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid ID provided");
                }

                if (string.IsNullOrEmpty(request?.Name))
                {
                    return BadRequest("Name is required");
                }

                var updatedItem = new
                {
                    Id = id,
                    Name = request.Name,
                    Description = request.Description ?? "No description provided",
                    UpdatedAt = DateTime.UtcNow
                };

                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// Delete a test item
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <returns>Deletion confirmation</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid ID provided");
                }

                var result = new
                {
                    Message = $"Test item with ID {id} has been deleted",
                    DeletedAt = DateTime.UtcNow,
                    Id = id
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
    }

    /// <summary>
    /// Request model for creating test items
    /// </summary>
    public class CreateTestItemRequest
    {
        /// <summary>
        /// Item name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Item description
        /// </summary>
        public string? Description { get; set; }
    }

    /// <summary>
    /// Request model for updating test items
    /// </summary>
    public class UpdateTestItemRequest
    {
        /// <summary>
        /// Item name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Item description
        /// </summary>
        public string? Description { get; set; }
    }
}