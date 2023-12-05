using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public IActionResult Login([FromServices] TokenService tokenService)
    {
        var token = tokenService.GenerateToken(null);
        return Ok(token);
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