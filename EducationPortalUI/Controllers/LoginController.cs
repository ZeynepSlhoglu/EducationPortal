using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EducationPortalUI.Models;
using System.Text;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Azure.Core;

namespace EducationPortalUI.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(User model)
        {
            try
            {
                HttpClient client = new();

                StringContent body = new(JsonConvert.SerializeObject(new
                {
                    email = model.Email,
                    password = model.Password
                }), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://localhost:7145/login", body);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject responseObj = JObject.Parse(responseBody);
                    string accessToken = (string)responseObj["accessToken"];

                    using (var userInfoRequest = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7145/userinfo"))
                    {
                        userInfoRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        userInfoRequest.Content = new StringContent("{}", Encoding.UTF8, "application/json");

                        HttpResponseMessage userInfoResponse = await client.SendAsync(userInfoRequest);

                        if (userInfoResponse.IsSuccessStatusCode)
                        {
                            var userInfoResponseBody = await userInfoResponse.Content.ReadAsStringAsync();
                            var userInfoJson = JObject.Parse(userInfoResponseBody);
                            var userId = (string)userInfoJson["userId"];
                            var email = (string)userInfoJson["email"];
                            var name = (string)userInfoJson["name"];
                            var instructorStatus = (string)userInfoJson["instructorStatus"]; 

                            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId),
                        new Claim(ClaimTypes.Email, email),
                        new Claim(ClaimTypes.Name, name),
                        new Claim("Token", accessToken),
                        new Claim("instructorStatus", instructorStatus)
                    };

                            ClaimsIdentity claimsIdentity = new(
                                claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            AuthenticationProperties authProperties = new()
                            {
                                ExpiresUtc = DateTime.UtcNow.AddYears(2),
                                IssuedUtc = DateTime.UtcNow
                            };

                            await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity),
                                authProperties);

                            return Json(new { success = true, redirectUrl = "/Home", accessToken = accessToken, userId = userId });
                        }
                        else
                        {
                            Console.WriteLine($"Error: {userInfoResponse.StatusCode}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View(model);
        }
 

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }

    }
}
