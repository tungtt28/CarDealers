using Microsoft.AspNetCore.Mvc;

namespace CarDealers.Areas.Admin.Controllers
{
    public class TrialDrivingController : Controller
    {
        [Area("admin")]
        [Route("admin/[controller]/[action]")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
