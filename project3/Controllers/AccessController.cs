using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using project3.Models;
using project3.Data;
using project3.IRepository;

namespace project3.Controllers
{
    public class AccessController : Controller
    {
        private readonly FelixDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;

        public AccessController(FelixDbContext context,IUserRepository userRepository, ICompanyRepository companyRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _companyRepository = companyRepository;

        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "User");


            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginVM modelLogin)
        {
            var user = await _userRepository.GetByEmailAsync(modelLogin.Email);

            if (user != null)
            {
                if (modelLogin.PassWord == user.user_password)
                {
                    HttpContext.Session.SetInt32("UserId", user.user_id);
                    HttpContext.Session.SetString("UserName", user.user_name);
                    HttpContext.Session.SetString("UserEmail", user.user_email);
                    HttpContext.Session.SetString("UserImage", user.user_image);
                    HttpContext.Session.SetInt32("UserRole", user.user_role);
                    List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, modelLogin.Email),
                    new Claim("OtherProperties","Example Role")

                    };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties properties = new AuthenticationProperties()
                    {

                        AllowRefresh = true,
                        IsPersistent = modelLogin.KeepLoggedIn
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), properties);

                    return RedirectToAction("Index", "User");
                }
                ViewData["ValidateMessage"] = "Incorrect Password";
                return View();
            }




            ViewData["ValidateMessage"] = "User Does not Exist";
            return View();
        }


        public IActionResult CompanyLogin()
        {
            ClaimsPrincipal claimCompany = HttpContext.User;

            if (claimCompany.Identity.IsAuthenticated)
                return RedirectToAction("Dashboard", "Company");


            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CompanyLogin(LoginVM modelLogin)
        {
            var userCompany = await _companyRepository.GetByEmailAsync(modelLogin.Email);

            if (userCompany != null)
            {
                if (modelLogin.PassWord == userCompany.password)
                {
                    HttpContext.Session.SetInt32("companyID", userCompany.company_id);
                    HttpContext.Session.SetString("companyName", userCompany.company_name);
                    HttpContext.Session.SetString("companyEmail", userCompany.company_email);
                    HttpContext.Session.SetString("companyImage", userCompany.company_image);
                    
                    List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, modelLogin.Email),
                    new Claim("OtherProperties","Example Role")

                    };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties properties = new AuthenticationProperties()
                    {

                        AllowRefresh = true,
                        IsPersistent = modelLogin.KeepLoggedIn
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), properties);

                    return RedirectToAction("Dashboard", "Company");
                }
                ViewData["ValidateMessage"] = "Incorrect Password";
                return View();
            }




            ViewData["ValidateMessage"] = "Company Does not Exist";
            return View();
        }
    }
}
