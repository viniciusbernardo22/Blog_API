using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModels;
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
           
            try
            {
                List<Category> categories = await context.Categories.ToListAsync();
                return Ok(new ResultViewModel<List<Category>>(categories));
            }
            catch (Exception exc)
            {
                string exception = $"{errorPrefix}GE01 - Não foi possivel coletar as categorias";
                return StatusCode(500, new ResultViewModel<string>(exception));
            }
            
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id, [FromServices] BlogDataContext context)
        {
            try
            {
                Category selectedCategory = await context.Categories.FirstOrDefaultAsync(cat => cat.Id == id);

                if (selectedCategory != null)
                {
                    return Ok(new ResultViewModel<Category>(selectedCategory));
                }

                return NotFound(new ResultViewModel<Category>($"Categoria de Id: {id} não foi encontrada."));
            }
            catch (Exception exc)
            {
                return StatusCode(500, new ResultViewModel<Category>("Falha interna no servidor"));
            }
            


        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync([FromBody] EditorCategoryViewModel model, [FromServices] BlogDataContext context)
        {
            if(!ModelState.IsValid) return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

            try
            {
                Category category = new Category
                {
                    Id = 0,
                    Name = model.Name,
                    Slug = model.Slug.ToLower()
                };

                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"v1/categories/${category.Id}", new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException exdb)
            {
                string exception = $"{errorPrefix}P001 - Não foi possivel incluir a categoria";

                return StatusCode(500, new ResultViewModel<Category>(exception));
            }
            catch
            {
                string exception = $"{errorPrefix}P002 - Falha interna no servidor";

                return StatusCode(500, new ResultViewModel<Category>(exception));

            }
        }

        [HttpPut("v1/categories/{id:int}")]
         public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] EditorCategoryViewModel model,[FromServices] BlogDataContext context)
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

                     return Ok(new ResultViewModel<Category>(selectedCategory));

                 } return NotFound(new ResultViewModel<Category>("Conteúdo não encontrado"));

            } catch (DbUpdateException e) {
                 string exception = $"{errorPrefix}PU01 - Não foi possivel editar a categoria";

                 return StatusCode(500, new ResultViewModel<Category>(exception));

             } catch (Exception e) {
                 string exception = $"{errorPrefix}PU02 - Não foi possivel editar a categoria";

                return StatusCode(500, new ResultViewModel<Category>(exception));
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
                     return Ok(new ResultViewModel<Category>(selectedCategory));
                 }

                 return NotFound(new ResultViewModel<Category>("Conteúdo não encontrado"));
            } catch (DbUpdateException e) {
                string exception = $"{errorPrefix}DE01 - Não foi possivel excluir a categoria";

                return StatusCode(500, new ResultViewModel<Category>(exception));
            } catch (Exception e) {
                string exception = $"{errorPrefix}DE02 - Não foi possivel excluir a categoria";

                return StatusCode(500, new ResultViewModel<Category>(exception));
            }
             
        }
    }
}
