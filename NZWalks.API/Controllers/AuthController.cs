using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager,ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }

        //CREATE USER
        //POST : /api/Auth/Register
        [HttpPost]
        [Route("Register")]

        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDTO.Username,
                Email = registerRequestDTO.Username,
            };
           var identityResult= await userManager.CreateAsync(identityUser,registerRequestDTO.Password);

            if(identityResult.Succeeded)
            {
                if(registerRequestDTO.Roles != null && registerRequestDTO.Roles.Any())
                {
                   identityResult= await userManager.AddToRolesAsync(identityUser, registerRequestDTO.Roles);
                }
                if (identityResult.Succeeded)
                {
                    return Ok("User was registered. Please login!");
                }
                
            }
            return BadRequest("Something went wrong");

        }

        //Check User
        //GET : /api/Auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var name = await userManager.FindByEmailAsync(loginRequestDTO.Username);
            if (name != null) {
              var checkPasswordResult=  await userManager.CheckPasswordAsync(name, loginRequestDTO.Password);
                if (checkPasswordResult )
                {
                    var roles =await userManager.GetRolesAsync(name);
                    if (roles != null)
                    {
                       var jwtToken= tokenRepository.CreateJWTToken(name, roles.ToList());
                        var response = new LoginResponseDTO
                        {
                            JWTToken = jwtToken,
                        };
                        return Ok(response);
                    }
                }

            }
            return BadRequest("Username or Password is incorrect");

        }
    }
}
