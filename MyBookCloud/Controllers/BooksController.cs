using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyBookCloud.Business.Books;
using MyBookCloud.Core.Api.Dto;

namespace MyBookCloud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IBookRepository bookRepository;

        public BooksController(IMapper mapper, IBookRepository bookRepository)
        {
            this.mapper = mapper;
            this.bookRepository = bookRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var books = this.bookRepository.GetAll();
            return this.Ok(this.mapper.Map<List<BookData>>(books));
        }
    }
}
