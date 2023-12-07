using Blog.Data;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class PostController : ControllerBase
    {
        [HttpGet]
        [Route("v1/posts")]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25)
        {

            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();
                var posts = await context.Posts
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .Include(x => x.Author)
                    .Select(x => new ListPostsViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = x.Author.Name
                    })
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(x => x.LastUpdateDate)
                    .ToListAsync();
                return Ok(new ResultViewModel<dynamic>(
                    new
                    {
                        total = count,
                        page,
                        pageSize,
                        posts
                    }
                ));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("Erro interno no servidor"));
            }
            
        }
    }
}
