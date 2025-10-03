using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entity.Entities
{
    public class ModelTrain : BaseEntity
    {
        public Guid ModelId { get; set; }
        public string? ModelName { get; set; }
        public string? ModelDescription { get; set; }
        public string? ModelHasUse { get; set; }
    }
}