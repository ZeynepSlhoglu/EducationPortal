using EducationPortalAPI.DAL;
using EducationPortalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EducationContentPortalAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EducationContentController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public EducationContentController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var educationContents = await _appDbContext.EducationContents.ToListAsync();

                if (educationContents.Any())
                    return Ok(educationContents);
                else
                    return NotFound("Kayıt bulunmamaktadır.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetEducationContent")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var educationContent = await _appDbContext.EducationContents.FirstOrDefaultAsync(ec => ec.EducationID == id);
                if (educationContent != null)
                    return Ok(educationContent);
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
            if (model == null)
                return BadRequest("Eklenecek veri yok.");

            var educationContent = await _appDbContext.EducationContents.FindAsync(model.ID);
            if (educationContent == null)
                return NotFound($"{model.ID} id'li veri bulunmamaktadır.");

            educationContent.EducationID = model.EducationID;
            educationContent.ContentName = model.ContentName;
            educationContent.ContentType = model.ContentType;
            educationContent.ContentPath = model.ContentPath;

            await _appDbContext.SaveChangesAsync();
            return Ok("Kullanıcı güncellendi");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var educationContent = await _appDbContext.EducationContents.FindAsync(id);
            if (educationContent != null)
            {
                _appDbContext.EducationContents.Remove(educationContent);
                await _appDbContext.SaveChangesAsync();
                return Ok("Kayıt silindi");
            }
            else
                return NotFound($"{id} id'li kayıt bulunmamaktadır.");
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] Education model)
        {
            using (var transaction = _appDbContext.Database.BeginTransaction())
            {
                try
                {
                    var addedModel = _appDbContext.Add(model).Entity;
                    await _appDbContext.SaveChangesAsync(); 

                    if (addedModel.ID != null || addedModel.ID != 0)
                    {
                        if (file == null || file.Length == 0)
                            return BadRequest("Dosya boş."); 
                         
                        var fileName = Path.GetFileName(file.FileName);
                        var contentType = file.ContentType;
                         
                        var contentPath = $"Uploads/{fileName}";
                         
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), contentPath);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var educationContent = new EducationContent
                        {
                            EducationID = addedModel.ID,
                            ContentName = fileName,
                            ContentType = contentType,
                            ContentPath = contentPath  
                        }; 

                        _appDbContext.Add(educationContent);

                        await _appDbContext.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Ok(educationContent);
                    }
                    else
                    {
                        return BadRequest(); 
                    }

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest($"Bir hata oluştu: {ex.Message}");
                }
            }

        }
    }
}
