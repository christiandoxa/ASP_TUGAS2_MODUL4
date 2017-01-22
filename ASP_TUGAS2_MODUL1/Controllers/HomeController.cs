using System.Web.Mvc;

namespace ASP_TUGAS2_MODUL1.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Welcome()
        {
            return View();
        }
    }
}