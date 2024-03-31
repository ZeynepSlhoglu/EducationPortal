using EducationPortalAPI.DAL;
using EducationPortalAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationPortalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public CategoryController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var categories = await _appDbContext.Categories.ToListAsync();
                if (categories.Any())
                    return Ok(categories);
                else
                    return NotFound("Kategori bulunmamaktadır.");
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
                var category = await _appDbContext.Categories.FindAsync(id);
                if (category is not null)
                    return Ok(category);
                else
                    return NotFound($"{id} id'li kategori bulunmamaktadır.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category model)
        {
            try
            {
                _appDbContext.Add(model);
                await _appDbContext.SaveChangesAsync();
                return Ok("Kategori eklendi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(Category model)
        {
            if (model is null)
                return BadRequest("Eklenecek kategori verisi bulunamadı.");

            var category = await _appDbContext.Categories.FindAsync(model.ID);
            if (category is null)
                return NotFound($"{model.ID} id'li kategori bulunmamaktadır.");

            category.CategoryName = model.CategoryName; 
            await _appDbContext.SaveChangesAsync();
            return Ok("Kategori güncellendi");
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _appDbContext.Categories.FindAsync(id);
            if (category is not null)
            {
                _appDbContext.Categories.Remove(category);
                await _appDbContext.SaveChangesAsync();
                return Ok("Kategori silindi");
            }
            else
                return NotFound($"{id} id'li kategori bulunmamaktadır.");
        }


    }
}
