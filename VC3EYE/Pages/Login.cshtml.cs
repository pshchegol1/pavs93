using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using VC3EYE.Data;
using VC3EYE.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace VC3EYE.Pages
{
    public class LoginModel : PageModel
    {
        private readonly Vc3eyeContext _db;
        public LoginModel(Vc3eyeContext db)
        {
            _db = db;
        }

        [BindProperty]
        public LoginInfoModel Login { get; set; }

        
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new LoginManager(_db).GetUserLoggedIn(Login);

                string userEmail = "";
                string userName = "";
                string userRole = "";
                string userID = "";

                if (user != null)
                {
                    userEmail = user.Email ?? "";
                    userName = user.FirstName + " " + user.LastName;
                    userRole = user.Role.Description.ToLower();
                    userID = user.UserId.ToString();

                    if (!string.IsNullOrEmpty(userEmail) && !string.IsNullOrEmpty(userName))
                    {
                        var claims = new List<Claim> {
                        new Claim(ClaimTypes.Email, userEmail),
                        new Claim(ClaimTypes.Name, userName),
                        new Claim(ClaimTypes.NameIdentifier, userID)
                    };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, userRole));
                        AuthenticationProperties authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                               new ClaimsPrincipal(claimsIdentity), authProperties);
                    }                 
                }
                else
                {
                    ModelState.AddModelError("Login.LoginError", "Incorrect Email or Password");
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync();
          
            return RedirectToPage("/Index");
        }
    }
}
