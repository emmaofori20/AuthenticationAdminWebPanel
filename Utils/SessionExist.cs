using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAdminWebPanel.Utils
{
	public class SessionExist : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var sessionVar = filterContext.HttpContext.Session.Get("AccessToken");
			if (sessionVar == null)
			{
				filterContext.Result = new RedirectResult("~/Account/SignOut");
				return;
			}
			base.OnActionExecuting(filterContext);
		}
	}
}
