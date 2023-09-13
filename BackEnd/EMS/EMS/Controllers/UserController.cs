using EMS.Context;
using EMS.Helpers;
using EMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace EMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AddDbContext _authContext;
        public UserController(AddDbContext addDbContext)
        {
            _authContext = addDbContext;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            if (userObj == null)
            
                return BadRequest();

            var user = await _authContext.Users.
                 FirstOrDefaultAsync(x=>x.Username == userObj.Username );
                
            if (user == null)
                
                return NotFound(new { Message = "User Not Found! " });

            if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                return BadRequest(new { Message = " Incorrect Credentials!!" });
            }

                
            user.Token = CreateJwt(user);

            return Ok(new
              {
                  Token = user.Token,
                  Message = "Login Successful!!"
              }) ;
            
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {
            if(userObj == null)
                return BadRequest();

            //Check username
            if(await CheckUserNameExistAsync(userObj.Username))
                return BadRequest(new {Message="Username Already Exist!!"});


            //Check Email
            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "Email Already Taken!!" });

            var email = EmailIsValid(userObj.Email);
            if(!string.IsNullOrEmpty(email))
                return BadRequest(new {Message  = email.ToString()});

            //Check password strength
            var pass = CheckPasswordStrength(userObj.Password);
            if(!string.IsNullOrEmpty(pass))
                return BadRequest(new {Message = pass.ToString()});

            userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            userObj.Role = "User";
            userObj.Token = "";
            await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                Message = "Signup Successful!!"
            });
        }

        private Task<bool> CheckUserNameExistAsync(string username)
              => _authContext.Users.AnyAsync(x => x.Username == username);
        
        private Task<bool> CheckEmailExistAsync(string email)
              => _authContext.Users.AnyAsync(x => x.Email == email);

        private string EmailIsValid(string email)
        {
            StringBuilder sb = new StringBuilder();
            if(!Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
            {
                sb.Append("Email is not valid" + Environment.NewLine);
            }
            return sb.ToString();
        }
        

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if(password.Length < 8)
                sb.Append("Minimum password length should be 8 characters" + Environment.NewLine);

            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password should be Alphanumeric" + Environment.NewLine);

            if(!Regex.IsMatch(password,"[<,>,@,!,#,$,%,^,&,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                sb.Append("Password should contain special character" + Environment.NewLine);

            return sb.ToString();

        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name,$"{user.FirstName} {user.LastName}")
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256);

            var tokenDescripter = new SecurityTokenDescriptor
            {
                Subject = identity,
                //Expires = DateTime.Now.AddSeconds(10),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescripter);
            return jwtTokenHandler.WriteToken(token);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await _authContext.Users.ToListAsync());
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            var user = await _authContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateRole([FromRoute] int id, User updateRoleRequest)
        {
            var user = await _authContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            user.Role = updateRoleRequest.Role;

            await _authContext.SaveChangesAsync();
            return Ok(user);
        }
    }
}
