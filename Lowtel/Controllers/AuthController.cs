using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Lowtel.Controllers
{
    public class AuthController : Controller
    {
		private readonly IAuthenticationSchemeProvider authenticationSchemeProvider;

		public AuthController(IAuthenticationSchemeProvider authenticationSchemeProvider)
		{
			this.authenticationSchemeProvider = authenticationSchemeProvider;
		}

        // This function returns the view of logins options
        public async Task<IActionResult> FacebookLogin()
        {
			var allSchemeProvider = (await authenticationSchemeProvider.GetAllSchemesAsync())
				.Select(n => n.DisplayName).Where(n => !String.IsNullOrEmpty(n));

			return View(allSchemeProvider);
        }

        // This function redirect to facebook login page, and can input username and password then login successly
        public IActionResult SignIn()
		{
			return Challenge(new AuthenticationProperties { RedirectUri = "/" }, "Facebook");
		}

        // This functions signs the current user out of the application
        public async Task<IActionResult> SignOut()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}
	}
}