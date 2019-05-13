using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using _5_08HW.Models;
using Microsoft.Extensions.Configuration;
using _5_08HW.Data;
using Newtonsoft.Json;


namespace _5_08HW.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString;
        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Jokes()
        {
            Manager mgr = new  Manager(_connectionString);
            var jvm = new JokesViewModel();
            jvm.Joke= mgr.GetAPIJoke();

            if (User.Identity.IsAuthenticated)
            {
                jvm.LoggedIn = true;
                User logged= mgr.GetByEmail( User.Identity.Name);
                if (logged.JokesLiked.Any(j => j.JokeId == jvm.Joke.Id))
                {
                    jvm.CouldLike = false;                   
                }                
            }
            return View(jvm);
        }
        [HttpPost]
        public IActionResult AddLike(int Id)
        {
            Manager mgr = new Manager(_connectionString);
            mgr.AddLike(mgr.GetJokeWithId(Id), mgr.GetByEmail(User.Identity.Name));
            return Json("Thank You");
        }
        public IActionResult AllJokes()
        {
            Manager mgr = new Manager(_connectionString);
            IEnumerable<Jokes> All=mgr.GetAllJokes();
            return View(All);
        }
    }
}
