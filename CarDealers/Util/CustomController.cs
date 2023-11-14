using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace CarDealers.Util
{
    public class CustomController : Controller
    {
        public string? UserId { get; set; }
        public string? UserRole { get; set; }
        public string[] authorizedRoles = new string[] { };

        //Check xem user duoc phep truy cap vao trang hay khong
        public bool HasAuthorized()
        {
            UserId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            UserRole = HttpContext.Session.GetString(Constant.LOGIN_USERROLE_SESSION_NAME);

            //NOT LOGIN + GUEST can access --> true
            if (UserId is null)
            {
                if (IsGuestFeature())
                {
                    return true;
                }
            }
            //HAS LOGIN + check role
            else
            {
                foreach (string r in authorizedRoles)
                {
                    if (r == UserRole)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //Dieu huong trang 
        public IActionResult RedirectBaseOnPage()
        {
            //Neu chua login
            if (UserId is null)
            {
                //check if authorizedRoles has admin --> login admin
                if (HasAdminRole())
                {
                    return Redirect(Constant.LOGIN_ADMIN_PAGE);
                }
                // else customer login
                return Redirect(Constant.LOGIN_PAGE);
            }
            //Neu da dang nhap
            else
            {
                return DefaultPageByRole(UserId, UserRole);
            }
        }

        private bool HasAdminRole()
        {
            for (int i = 0; i < authorizedRoles.Length; i++)
            {
                if (authorizedRoles[i] == Constant.ADMIN_ROLE)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsGuestFeature()
        {
            foreach (string role in authorizedRoles)
            {
                if (role == Constant.GUEST_ROLE)
                {
                    return true;
                }
            }
            return false;
        }
        private IActionResult DefaultPageByRole(string UserId, string UserRole)
        {
            if (UserRole?.ToLower() == Constant.CUSTOMER_ROLE)
            {
                return Redirect(Constant.DEFAULT_CUSTOMER_PAGE);
            }
            else if (UserRole?.ToLower() == Constant.ADMIN_ROLE)
            {
                return Redirect(Constant.DEFAULT_ADMIN_PAGE);
            }
            else
            {
                return Redirect(Constant.DEFAULT_GUEST_PAGE);
            }
        }
    }
}

