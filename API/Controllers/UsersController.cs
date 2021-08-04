using System.Collections.Generic;
using System.Linq;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult<IEnumerable<AppUser>> getUsers(){
            return _context.Users.ToList();
        }
        [Authorize]
        [HttpGet("{Id}")]
        public ActionResult<AppUser> getUser(int id){
            return _context.Users.Find(id);
        }



    }
}