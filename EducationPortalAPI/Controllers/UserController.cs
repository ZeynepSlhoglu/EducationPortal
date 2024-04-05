using EducationPortalAPI.DAL;
using EducationPortalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationPortalAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public UserController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var Users = await _appDbContext.Users.ToListAsync(); 
                if (Users.Any())
                    return Ok(Users);
                else
                    return NotFound("Kullanıcı bulunmamaktadır.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var User = await _appDbContext.Users.FindAsync(id);
                if (User is not null)
                    return Ok(User);
                else
                    return NotFound($"{id} id'li kullanıcı bulunmamaktadır.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("update/{id}/{instructorStatus}")]
        public async Task<IActionResult> Update(string id, bool instructorStatus)
        {
            var user = await _appDbContext.Users.FindAsync(id);
            if (user is null)
                return NotFound($"{id} ID'li kullanıcı bulunamadı.");

            user.InstructorStatus = instructorStatus;

            await _appDbContext.SaveChangesAsync();

            return Ok("Kullanıcı güncellendi.");
        }



        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var User = await _appDbContext.Users.FindAsync(id);
            if (User is not null)
            {
                _appDbContext.Users.Remove(User);
                await _appDbContext.SaveChangesAsync();
                return Ok("kullanıcı silindi");
            }
            else
                return NotFound($"{id} id'li kullanıcı bulunmamaktadır.");
        }

    }
}
