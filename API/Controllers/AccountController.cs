using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context;
        }
        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if(await UserExist(registerDto.userName)) return BadRequest("Username is taken");
            using var hmac = new HMACSHA512();
            var user = new AppUser{
                UserName = registerDto.userName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.passWord)),
                PasswordSalt = hmac.Key
            };
            _context.Add(user);
            await _context.SaveChangesAsync(); 
            return user;
    }
    private async Task<bool> UserExist(string userName)
    {
        return await _context.Users.AnyAsync(x=>x.UserName.Equals(userName.ToLower()));
    }
    [HttpPost("login")]
    public async Task<ActionResult<AppUser>> Login(LoginDto loginDto)
    {
        var user = await _context.Users.
        SingleOrDefaultAsync(x=>x.UserName == loginDto.userName);
        if(user == null) return Unauthorized("Invalid UserName");
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.passWord));
        for(int i = 0; i < computeHash.Length;i++)
        {
            if(computeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Passdword");

        }
        return user;


    }

        
    }
    
}