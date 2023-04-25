using AuthenticationAdminWebPanel.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace AuthenticationAdminWebPanel.Controllers
{
	public class AccountController : Controller
	{
		private HttpClient apiClient;

		public AccountController(IHttpClientFactory factory)
        {
			apiClient = factory.CreateClient("myApi");

		}
		public async Task<IActionResult> Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			var response = await apiClient.PostAsync("login", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
			if (response.IsSuccessStatusCode)
			{
				var accessuser = await response.Content.ReadFromJsonAsync<LoginUser>();
				if (accessuser.roles.Any(x => x.Contains("Admin")))
				{
					// Save the access token in the user's session
					HttpContext.Session.SetString("AccessToken", accessuser.Token);
					HttpContext.Session.SetString("Username", accessuser.Username);
					return RedirectToAction("Index", "Home");
				}
				return RedirectToAction("Authorised", "Account");
			}
			else
			{
				ViewBag.ErrorMessage = "Invalid username or password.";
				return View(model);
			}
		}

		public async Task<IActionResult> Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login", "Account");
		}
		public async Task<IActionResult> Authorised()
		{
			return View();
		}
		public async Task<IActionResult> SignOut()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login", "Account");
		}
	}
}
