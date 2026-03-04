using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Domain.Entities.Abstracts
{
    public abstract class BaseEntity : IEntity
    {
        public int Id { get; set; }
        public string CreatedChannelCode { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedTranCode { get; set; } = string.Empty;
        public string CreatedUserCode { get; set; } = string.Empty;
        public string? LastUpdatedChannelCode { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string? LastUpdatedTranCode { get; set; }
        public string? LastUpdatedUserCode { get; set; }
    }
}
