using MetronicAddressBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MetronicAddressBook.BAL;

namespace MetronicAddressBook.Controllers
{
    [CheckAccess]
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        public IActionResult Index()
        {
            ViewBag.UserID = HttpContext.Session.GetString("UserID");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}