using EducationPortalAPI.DAL;
using EducationPortalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationContentPortalAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EducationContentContentController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public EducationContentContentController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var EducationContents = await _appDbContext.EducationContents.ToListAsync();

                if (EducationContents.Any())
                    return Ok(EducationContents);
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
                var EducationContent = await _appDbContext.EducationContents.FindAsync(id);
                if (EducationContent is not null)
                    return Ok(EducationContent);
                else
                    return NotFound($"{id} id'li kayıt bulunmamaktadır.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(EducationContent model)
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
        public async Task<IActionResult> Update(EducationContent model)
        {
            if (model is null)
                return BadRequest("Eklenecek veri yok.");

            var educationContent = await _appDbContext.EducationContents.FindAsync(model.ID);
            if (educationContent is null)
                return NotFound($"{model.ID} id'li veri bulunmamaktadır.");

            educationContent.EducationID = model.EducationID;
            educationContent.ContentName = model.ContentName;
            educationContent.ContentType = model.ContentType;
            educationContent.ContentPath = model.ContentPath;

            await _appDbContext.SaveChangesAsync();
            return Ok("Kullanıcı güncellendi");
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var EducationContent = await _appDbContext.EducationContents.FindAsync(id);
            if (EducationContent is not null)
            {
                _appDbContext.EducationContents.Remove(EducationContent);
                await _appDbContext.SaveChangesAsync();
                return Ok("Kayıt silindi");
            }
            else
                return NotFound($"{id} id'li kayıt bulunmamaktadır.");
        }
    }
}
