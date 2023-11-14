using Microsoft.AspNetCore.Mvc;

namespace CarDealers.Controllers
{
    public class CheckOutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
