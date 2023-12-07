using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class PostController : ControllerBase
    {
        [HttpGet]
        [Route("v1/posts")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
        {
            var posts = await context.Posts.AsNoTracking().Select(x =>
            new {
                x.Id
            }).ToListAsync();
            return Ok(posts);
        }
    }
}
