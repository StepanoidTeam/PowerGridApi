using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PowerGrid.API.Models;
//todo: move auth logic to separate controller?
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace PowerGrid.API.Controllers
{
    [Route("api/[controller]")]
    public class NoteController : Controller
    {
        private INoteRepository _repo;
        public NoteController(INoteRepository repo)
        {
            this._repo = repo;
        }

        [HttpGet("login/{password}")]
        public IActionResult Login(string password)
        {
            if (password != "12345")
            {
                return NotFound();
            }
            var identity = new ClaimsIdentity("password");
            identity.AddClaim(new Claim(ClaimTypes.Name, "DefaultUser"));
            identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
            //identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            HttpContext.Authentication.SignInAsync("CustomAuth", new ClaimsPrincipal(identity)).Wait();
            return new ObjectResult("success");
        }

        [HttpGet]
        //[Authorize(Roles = "User")]
        //[Authorize(Roles = "Admin")]
        public IEnumerable<Note> GetAll()
        {
            return _repo.GetAll();
        }

        [HttpGet("{id}", Name = "GetNote")]
        public IActionResult GetById(string id)
        {
            var item = _repo.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Note item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            _repo.Add(item);
            return CreatedAtRoute("GetNote", new { controller = "Note", id = item.Key }, item);
        }

        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _repo.Remove(id);
        }
    }
}