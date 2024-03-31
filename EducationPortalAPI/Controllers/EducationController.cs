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
    public class EducationController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public EducationController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var Educations = await _appDbContext.Educations.ToListAsync();

                if (Educations.Any())
                    return Ok(Educations);
                else
                    return NotFound("Kayıt bulunmamaktadır.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var Education = await _appDbContext.Educations.FindAsync(id);
                if (Education is not null)
                    return Ok(Education);
                else
                    return NotFound($"{id} id'li kayıt bulunmamaktadır.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Education model)
        {
            try
            {
                _appDbContext.Add(model);
                await _appDbContext.SaveChangesAsync();
                return Ok("Kayıt eklendi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(Education model)
        {
            if (model is null)
                return BadRequest("Eklenecek veri yok.");

            var Education = await _appDbContext.Educations.FindAsync(model.ID);
            if (Education is null)
                return NotFound($"{model.ID} id'li veri bulunmamaktadır.");

            Education.Duration = model.Duration;
            Education.CategoryName = model.CategoryName;
            Education.Capacity = model.Capacity;
            Education.Price = model.Price;  
            Education.Title = model.Title;  
            Education.UserId = model.UserId;

            await _appDbContext.SaveChangesAsync();
            return Ok("Kullanıcı güncellendi");
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var Education = await _appDbContext.Educations.FindAsync(id);
            if (Education is not null)
            {
                _appDbContext.Educations.Remove(Education);
                await _appDbContext.SaveChangesAsync();
                return Ok("Kayıt silindi");
            }
            else
                return NotFound($"{id} id'li kayıt bulunmamaktadır.");
        }
    }
}
