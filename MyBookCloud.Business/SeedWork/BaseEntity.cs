using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBookCloud.Business.SeedWork
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}
