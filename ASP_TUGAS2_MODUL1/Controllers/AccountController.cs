using System.Web.Mvc;
using System.Web.Security;
using ASP_TUGAS2_MODUL1.Models.EntityManager;
using ASP_TUGAS2_MODUL1.Models.ViewModel;

namespace ASP_TUGAS2_MODUL1.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(UserSignUpView USV)
        {
            if (ModelState.IsValid)
            {
                UserManager UM = new UserManager();
                if (!UM.IsLoginNameExist(USV.LoginName))
                {
                    UM.AddUserAccount(USV);
                    FormsAuthentication.SetAuthCookie(USV.FirstName, false);
                    return RedirectToAction("Welcome", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login Name already taken.");
                }
            }
            return View();
        }
    }
}