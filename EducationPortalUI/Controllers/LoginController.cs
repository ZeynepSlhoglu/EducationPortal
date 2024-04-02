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

                            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId),
                        new Claim(ClaimTypes.Email, email),
                        new Claim(ClaimTypes.Name, name),
                        new Claim("Token", accessToken)
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

                            return Json(new { success = true, redirectUrl = "/Home", accessToken = accessToken }); 
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


        [HttpPost]
        public async Task<IActionResult> Logout(string accessToken)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                { 
                    string jsonBody = "{}";  
                    var body = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                     
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7145/logout");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); 
                    request.Content = body;  

                    // Gönderilen isteği işle
                    HttpResponseMessage response = await client.SendAsync(request);


                    if (response.IsSuccessStatusCode)
                    {
                        await HttpContext.SignOutAsync(); 
                        return RedirectToAction("Index", "Login");
                    }
                    else
                    { 
                        ViewBag.ErrorMessage = "Logout request failed with status code: " + response.StatusCode;
                        return View("Error");  
                    }
                }
            }
            catch (Exception ex)
            { 
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");  
            }
        }


    }
}
