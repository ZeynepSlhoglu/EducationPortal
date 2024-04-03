using EducationPortalUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace EducationPortalUI.Controllers
{
    public class ParticipationController : Controller
    {
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
                var educations = await GetParticipationEducations();

                return View(educations);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        public async Task<List<Education>> GetParticipationEducations()
        {
            var accessToken = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                     
                    HttpResponseMessage response = await client.GetAsync($"https://localhost:7145/api/Education");
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("API'den Education verisi alınamadı.");
                    }
                    string responseData = await response.Content.ReadAsStringAsync();
                    var educationInfos = JsonConvert.DeserializeObject<List<Education>>(responseData);
                     
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    HttpResponseMessage responseParticipation = await client.GetAsync($"https://localhost:7145/api/Participation/GetParticipationById/" + userId);
                    if (!responseParticipation.IsSuccessStatusCode)
                    {
                        throw new Exception("API'den Participation verisi alınamadı.");
                    }
                    string responseParticipationData = await responseParticipation.Content.ReadAsStringAsync();
                    var participationInfos = JsonConvert.DeserializeObject<List<Participation>>(responseParticipationData);
                     
                    var excludedEducationIds = participationInfos.Select(p => p.EducationID).Distinct();
                    var filteredEducationInfos = educationInfos.Where(e => excludedEducationIds.Contains(e.ID)).ToList();

                    return filteredEducationInfos;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Bir hata oluştu: {ex.Message}");
            }
        }
    }
}
