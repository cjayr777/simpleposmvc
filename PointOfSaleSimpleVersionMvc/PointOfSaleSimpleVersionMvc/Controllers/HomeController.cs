using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSimpleVersionMvc.Models;
using Pos.ViewModel;

namespace PointOfSaleSimpleVersionMvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorVm { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
