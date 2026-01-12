using AutoMapper;
using MyBookCloud.Core.Api.Dto;
using MyBookCloud.Data.Entities;

namespace MyBookCloud.Core.Api.AutoMapperProfiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            this.CreateMap<BookEntity, BookData>();
        }
    }
}
