using shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationAdminWebPanel.Models
{
	public class LoginViewModel
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class Applications
	{
		public int ApplicationId { get; set; }
		public string Name { get; set; }
	}

	

	public class FinalAssignment
	{
		public string UserId { get; set; }
		public List<int> ApplicationId { get; set; }

	}



	public class UserApplication
	{
		public int UserApplicationId { get; set; }
		public int ApplicationId { get; set; }
		public string UserId { get; set; }
		public string UserCrendentials { get; set; }
	}

	public class LoginUser
	{
		public string Username { get; set; }
		public DateTime expirationDate { get; set; }
		public string Token { get; set; }
		public string Id { get; set; }
		public List<string> roles { get; set; }
	}

	public class User
	{
		public int UserId { get; set; }
		public string Username { get; set; } = null!;
		public string Password { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Role { get; set; } = "User";
	}
}
