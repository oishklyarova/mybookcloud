using System;

namespace MyBookCloud.Common.Messages
{
    public interface IBookEnrichedMessage
    {
        Guid BookId { get; set; }

        string CoverThumbnailUrl { get; set; }

        int PageCount { get; set; }

        Guid UserId { get; set; }
    }
}

