using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;
using EducationPortalUI.Models;
using Microsoft.AspNetCore.Authorization;

namespace EducationPortalUI.Controllers
{
    [Authorize]
    public class EducationController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var userId = "";
            var accessToken = "";

            var nameIdentifierClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (nameIdentifierClaim != null)
            {
                userId = nameIdentifierClaim.Value;
            }

            var accessTokenClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token");
            if (accessTokenClaim != null)
            {
                accessToken = accessTokenClaim.Value;
            }

            try
            {
                var educations = await GetEducations(userId, accessToken);

                return View(educations);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } 
        public async Task<List<Education>> GetEducations(string id, string accessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    HttpResponseMessage response = await client.GetAsync($"https://localhost:7145/api/Education/GetEducation?id={id}");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();

                        var educationInfos = JsonConvert.DeserializeObject<List<Education>>(responseData);

                        return educationInfos;
                    }
                    else
                    {
                        throw new Exception("API'den veri alınamadı.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Bir hata oluştu: {ex.Message}");
            }
        }

    

        public async Task<IActionResult> CreateEducationsFunc(Education model)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var accessToken = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var educationData = new
                    {
                        title = model.Title,
                        categoryName = model.CategoryName,
                        userId,
                        model.Capacity,
                        model.Duration,
                        model.Price
                    };

                    var jsonContent = JsonConvert.SerializeObject(educationData);
                    var response = await client.PostAsync("https://localhost:7145/api/Education", new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();

                        var createdEducation = JsonConvert.DeserializeObject<Education>(responseData);

                        return Ok(createdEducation);
                    }
                    else
                    {
                        throw new Exception("API'den veri alınamadı.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Bir hata oluştu: {ex.Message}");
            }
        }


    }
}
