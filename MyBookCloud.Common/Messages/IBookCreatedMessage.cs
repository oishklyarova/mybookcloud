using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBookCloud.Common.Messages
{
    public interface IBookCreatedMessage
    {
        public Guid BookId { get; set; }

        public string Isbn { get; set; }

        public Guid UserId { get; set; }
    }
}
