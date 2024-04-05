using EducationPortalAPI.Models;
using EducationPortalUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace EducationPortalUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInfo()
        {
            var accessToken = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value; 
             
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await client.PostAsync("https://localhost:7145/userInfo", null);

                if (response.IsSuccessStatusCode)
                {

                    return Json(new { success = true, redirectUrl = "/Login/Logout" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Ýstek gönderilemedi.");
                }
            }
        }




    }
}
