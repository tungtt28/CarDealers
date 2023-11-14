using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class RoleController : CustomController
    {
        private readonly CarDealersContext _context;

        public RoleController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        public IActionResult ListRole()
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            //take date from db
            var role = _context.UserRoles.Where(x => x.DeleteFlag == false).ToList();
            return View(role);
        }

        public ActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateRole(UserRole model)
        {
            if (ModelState.IsValid)
            {
                var roles = _context.UserRoles.Where(x => x.DeleteFlag == false).ToList();
                if (roles.Any(x => x.RoleName.Equals(model.RoleName)))
                {
                    ModelState.AddModelError("RoleName", "The name already exists. Please choose a different name.");
                    return View(model);
                }

                var role = new UserRole { RoleName = model.RoleName };
                _context.UserRoles.Add(role);
                _context.SaveChanges();
            }
            return RedirectToAction("ListRole");
        }

        [HttpGet]
        public IActionResult DeleteRole(int id)
        {
            if (ModelState.IsValid)
            {
                var role = _context.UserRoles.Where(x => x.UserRoleId == id).FirstOrDefault();
                role.DeleteFlag = true;
                _context.UserRoles.Update(role);
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListRole");
        }

        [HttpPost]
        public IActionResult DeleteListRole(List<int> selectedIds)
        {
            if (ModelState.IsValid)
            {
                foreach (var id in selectedIds)
                {
                    var role = _context.UserRoles.Find(id);
                    if (role != null)
                    {
                        role.DeleteFlag = true;
                        _context.UserRoles.Update(role);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListRole");
        }


        [HttpGet]
        public ActionResult UpdateRole(int id)
        {
            if (ModelState.IsValid)
            {
                var role = _context.UserRoles.Where(x => x.UserRoleId == id && x.DeleteFlag == false).FirstOrDefault();

                if (role == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                return View(role);
            }
            return RedirectToAction("RecordNotFound");
        }

        [HttpPost]
        public ActionResult UpdateRole(UserRole model)
        {
            if (ModelState.IsValid)
            {
                var roles = _context.UserRoles.Where(x => x.DeleteFlag == false).ToList();
                var role = roles.Where(x => x.UserRoleId == model.UserRoleId).FirstOrDefault();

                if (role == null)
                {
                    return RedirectToAction("RecordNotFound");
                }

                if (roles.Any(x=>x.RoleName.Equals(model.RoleName) && x.UserRoleId != model.UserRoleId)) {
                    ModelState.AddModelError("RoleName", "The name already exists. Please choose a different name.");
                    return View(model);
                }

                role.RoleName = model.RoleName;
                _context.UserRoles.Update(role);
                _context.SaveChanges();
            }
            return RedirectToAction("ListRole");
        }

    }
}
