using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly string errorPrefix = "0CCT";

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

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync([FromBody] Category model, [FromServices] BlogDataContext context)
        {
            try
            {
                await context.Categories.AddAsync(model);
                await context.SaveChangesAsync();

                return Created($"v1/categories/${model.Id}", model);
            }
            catch (DbUpdateException exdb)
            {
                string exception = $"{errorPrefix}P01 - Não foi possivel incluir a categoria";

                return StatusCode(500, exception);
            }
            catch (Exception e)
            {
                string exception = $"{errorPrefix}P02 - Falha interna no servidor";

                return StatusCode(500, exception);
            }
        }

        [HttpPut("v1/categories/{id:int}")]
         public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] Category model,[FromServices] BlogDataContext context)
         {
             
             try
             {
                 Category selectedCategory = await context.Categories.FirstOrDefaultAsync(cat => cat.Id == id);

                 if (selectedCategory != null)
                 {
                     selectedCategory.Name = model.Name;
                     selectedCategory.Slug = model.Slug;

                     context.Update(selectedCategory);
                     await context.SaveChangesAsync();

                     return Ok(selectedCategory);

                 } return NotFound();

            } catch (DbUpdateException e) {
                 string exception = $"{errorPrefix}P03 - Não foi possivel editar a categoria";

                 return StatusCode(500, exception);

             } catch (Exception e) {
                 string exception = $"{errorPrefix}P04 - Não foi possivel editar a categoria";

                 return StatusCode(500, exception);
             }

        }

         [HttpDelete("v1/categories/{id:int}")]
         public async Task<IActionResult> DeleteAsync([FromRoute] int id, [FromServices] BlogDataContext context)
         {

             try
             {
                 Category selectedCategory = await context.Categories.FirstOrDefaultAsync(cat => cat.Id == id);

                 if (selectedCategory != null)
                 {
                     context.Categories.Remove(selectedCategory);
                     await context.SaveChangesAsync();
                     return Ok(selectedCategory);
                 }

                 return NotFound();
            } catch (DbUpdateException e) {
                string exception = $"{errorPrefix}P05 - Não foi possivel excluir a categoria";

                return StatusCode(500, exception);
            } catch (Exception e) {
                string exception = $"{errorPrefix}P06 - Não foi possivel excluir a categoria";

                return StatusCode(500, exception);
            }
             
        }
    }
}
