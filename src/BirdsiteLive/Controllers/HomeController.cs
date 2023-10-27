using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BirdsiteLive.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BirdsiteLive.Models;

namespace BirdsiteLive.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISocialMediaService _socialMediaService;

        public HomeController(ISocialMediaService socialMediaService, ILogger<HomeController> logger)
        {
            _socialMediaService = socialMediaService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var viewData = new HomeViewModel()
            {
                ServiceName = _socialMediaService.ServiceName,
            };
            return View(viewData);
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

        [HttpPost]
        public IActionResult Index(string handle)
        {
            return RedirectToAction("Index", "Users", new {id = handle});
        }
    }
}
