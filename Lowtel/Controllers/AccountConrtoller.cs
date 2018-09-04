using Lowtel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Lowtel.Controllers
{
    public class AccountConrtoller : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountConrtoller(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public ViewResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(User vm)
        {

            if (ModelState.IsValid)
            {

                var user = new ApplicationUser { FirstName = vm.FirstName, LastName = vm.LastName,Email = vm.Email };
                var resault = await _userManager.CreateAsync(user, vm.Password);

                if (resault.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in resault.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(vm);
        }
    }

}