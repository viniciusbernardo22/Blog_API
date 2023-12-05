using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using SecureIdentity.Password;

namespace Blog.Controllers;


[ApiController]
public class AccountController : ControllerBase
{
    [HttpPost("v1/account/register")]
    public async Task<ActionResult> Register([FromBody] RegisterViewModel model, [FromServices] BlogDataContext context)
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
    public async Task<IActionResult> Login([FromServices] TokenService tokenService, [FromBody] LoginViewModel model, [FromServices] BlogDataContext context)
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


    //[Authorize(Roles = "user")]
    //[HttpGet("v1/user")]
    //public IActionResult GetUser() => Ok(User.Identity.Name);
    
    //[Authorize(Roles = "author")]
    //[HttpGet("v1/author")]
    //public IActionResult GetAuthor() => Ok(User.Identity.Name);
    
    //[Authorize(Roles = "admin")]
    //[HttpGet("v1/admin")]
    //public IActionResult GetAdmin() => Ok(User.Identity.Name);

}