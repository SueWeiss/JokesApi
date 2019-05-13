using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using _5_08HW.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace _5_08HW.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString;
        public AccountController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoggingIn(string email, string password)
        {
            Manager mgr = new Manager(_connectionString);
            var user = mgr.Login(email, password);
            if (user == null)
            {
                TempData["message"] = "Invalid login attempt";
                return Redirect("/account/login");
            }

            var claims = new List<Claim>
                {
                    new Claim("user", email)
                };
            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();
            return Redirect("/home/jokes");
        }
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(User user, string password)
        {
            Manager mgr = new Manager(_connectionString);
            mgr.AddUser(user, password);
            return RedirectToAction("GetJokes", "Home");
        }
    }
}