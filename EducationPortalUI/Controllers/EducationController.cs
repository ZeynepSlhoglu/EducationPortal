﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;
using EducationPortalUI.Models;
using Microsoft.AspNetCore.Authorization;
using EducationPortalAPI.DAL;
using Azure.Core;
using System.Net.Mime;

namespace EducationPortalUI.Controllers
{
    [Authorize]
    public class EducationController : Controller
    {
        public IActionResult Create()
        {
            return View();
        } 
        public async Task<IActionResult> GetAll()
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
                var educations = await GetEducations();

                return View(educations);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
                var educations = await GetEducationsById(userId, accessToken);

                return View(educations);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<List<Education>> GetEducationsById(string id, string accessToken)
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
         
        public async Task<List<Education>> GetEducations()
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
                    var filteredEducationInfos = educationInfos.Where(e => !excludedEducationIds.Contains(e.ID)).ToList();

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
