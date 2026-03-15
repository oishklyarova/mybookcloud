using Microsoft.AspNetCore.Mvc;
using MyBookCloud.Core.Api.Dto;
using MyBookCloud.Core.Api.Services;

namespace MyBookCloud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var books = await _bookService.GetAllBooksAsync();
            return this.Ok(books);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BookData bookData)
        {
            var result = await _bookService.AddBookAsync(bookData);
            return this.Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] BookData bookData)
        {
            var result = await _bookService.UpdateBookAsync(id, bookData);
            if (result == null)
            {
                return this.NotFound();
            }

            return this.Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (!result)
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
    }
}
