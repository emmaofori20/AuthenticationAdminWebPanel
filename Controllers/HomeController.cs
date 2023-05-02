using AuthenticationAdminWebPanel.Models;
using AuthenticationAdminWebPanel.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shared.DTOs;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace AuthenticationAdminWebPanel.Controllers
{
	[SessionExist]
	public class HomeController : Controller
	{
        private HttpClient apiClient;

        public HomeController(IHttpClientFactory factory)
		{
            apiClient = factory.CreateClient("myApi");
        }

        public async Task<IActionResult> Index()
        {

            var response = await apiClient.GetAsync("");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<UserDTO>>();

                return View(result);

            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View();
            }

        } 
        
        public async Task<IActionResult> Profile()
        {

                ViewBag.ErrorMessage = "Invalid username or password.";
                return View();
            

        }

        public IActionResult CreateUser()
        {
            return View();
        }


        public async Task<IActionResult> Delete(string UserId)
        {
            var sessionVar = GetToken();
            apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessionVar);
            var response = await apiClient.DeleteAsync($"{UserId}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User model)
        {
            try
            {
                var sessionVar = GetToken();
                apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessionVar);
                var response = await apiClient.PostAsync("register", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.CreationError = "An error occured";
                return View(model);
            }
            catch (Exception er)
            {
                return View();

            }
        }

        public async Task<IActionResult> AssignUser(string UserId)
        {
            var sessionVar = GetToken();
            apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessionVar);
            var response = await apiClient.GetAsync("Applications");
            var UserResponse = await apiClient.GetAsync($"{UserId}");
            var AssignmentResponse = await apiClient.GetAsync($"GetUserApplicationAssignment/{UserId}");
            List<IsUserAssigned> _isUserAssigned = new List<IsUserAssigned>();

            if (response.IsSuccessStatusCode && UserResponse.IsSuccessStatusCode)
            {
                var myapps = await response.Content.ReadFromJsonAsync<List<Applications>>();
                var User = await UserResponse.Content.ReadFromJsonAsync<UserDTO>();
                var AssignedApps = await AssignmentResponse.Content.ReadFromJsonAsync<List<UserApplication>>();

                foreach (var item in myapps)
                {
                    var userApplication = AssignedApps.Where(x => x.UserId == UserId && x.ApplicationId == item.ApplicationId).FirstOrDefault();
                    var addedItem = new IsUserAssigned()
                    {
                        ApplicationId = item.ApplicationId,
                        Name = item.Name,
                        IsAssigned = AssignedApps.Any(x => x.UserId == UserId && x.ApplicationId == item.ApplicationId),
                        userApplicationId = userApplication == null ? 0 : userApplication.ApplicationId ,
                    };

                    _isUserAssigned.Add(addedItem);
                }

                UserAssignmentViewModel userAssignment = new UserAssignmentViewModel
                {
                    username = User.username,
                    UserId = User.UserId,
                    userAssigned = _isUserAssigned
                };
                return View(userAssignment);

            }
            ViewBag.Error = "An error occured";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> AssignUser(shared.DTOs.UserAssignmentViewModel model)
        {
            var sessionVar = GetToken();
            FinalAssignment assignemnt = new FinalAssignment();
            //assignemnt.UserId = model.UserId;
            //List<IsUserAssigned> AppIds = new List<int>();
            //foreach (var item in model.userAssigned)
            //{
            //    if (item.IsAssigned)
            //    {
            //        AppIds.Add(item.ApplicationId);
            //    }
            //}

            //assignemnt.ApplicationId = AppIds;
            apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessionVar);
            var response = await apiClient.PostAsync("AssignUserToApplication", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));


            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("Index", "Home");

            }
            ViewBag.Error = "An error occured";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
        private string GetToken()
        {
            return HttpContext.Session.GetString("AccessToken");
        }
    }
}