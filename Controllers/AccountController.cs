using System.Text.RegularExpressions;
using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers;


[ApiController]
public class AccountController : ControllerBase
{
    [HttpPost("v1/account/register")]
    public async Task<ActionResult> Register(
        [FromBody] RegisterViewModel model,
        [FromServices] EmailService emailService, 
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Slug = model.Email.Replace("@", "-").Replace(".", "-")
        };

        var password = PasswordGenerator.Generate(25);
        user.PasswordHash = PasswordHasher.Hash(password);

        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            emailService.Send(user.Name, user.Email, subject:"Teste de Desenvolvimento", body: $"Sua senha é <strong>{password}</strong>");
            return Ok(new ResultViewModel<dynamic>(new
            {
                user = user.Email, password

            }));


        }
        catch (Exception e)
        {
            return StatusCode(400, new ResultViewModel<string>("Esse e-mail ja está cadastrado."));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("Falha interna no servidor."));
        }
        
    }

    [HttpPost("v1/account/login")]
    public async Task<IActionResult> Login([FromServices] TokenService tokenService, 
        [FromBody] LoginViewModel model, 
        [FromServices] BlogDataContext context)
    {
        if(!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user = await context.Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(user => user.Email == model.Email);

        var invalidMsg = StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválido."));

        if (user == null)
            return invalidMsg;

        if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
            return invalidMsg;

        try
        {
            var token = tokenService.GenerateToken(user);
            return Ok(new ResultViewModel<string>(token, null));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("Falha interna no servidor."));
        }

    }

    [Authorize]
    [HttpPost("v1/account/upload-image")]
    public async Task<IActionResult> UploadImage(
        [FromBody] UploadImageViewModel model, 
        [FromServices] BlogDataContext context)
    {
        var fileName = $"{Guid.NewGuid()}.jpg";
        var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(model.Base64Image, string.Empty);
        var bytes = Convert.FromBase64String(data);

        try
        {
            await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);

        }
        catch
        {
            return StatusCode(500, $"Falha interna no servidor ao carregar a imagem {fileName}");
        }

        var user = await context.Users.FirstOrDefaultAsync(user => user.Email == User.Identity.Name);

        if (user == null)
            return NotFound(new ResultViewModel<string>("Usuário não encontrado, operação cancelada."));

        user.Image = $"https://localhost:0000/images{fileName}";


        try
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<string>(user.Image, null));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("05XPTOIMG - Falha interna no servidor."));
        }

    }
    
}