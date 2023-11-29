using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
        {
            List<Category> categories = await context.Categories.ToListAsync();
            return Ok(categories);

        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id, [FromServices] BlogDataContext context)
        {
            Category selectedCategory = await context.Categories.FirstOrDefaultAsync(cat => cat.Id == id);
            if (selectedCategory != null)
            {
                return Ok(selectedCategory);
            }

            return NotFound();


        }

    }
}
