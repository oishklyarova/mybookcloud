namespace MyBookCloud.Application.AutoMapperProfiles
{
    using AutoMapper;
    using MyBookCloud.Application.Dto;
    using MyBookCloud.Business.Books;

    public class BookProfile : Profile
    {
        public BookProfile()
        {
            this.CreateMap<BookEntity, BookData>();
            this.CreateMap<BookData, BookEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
