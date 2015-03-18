using System.Web.Mvc;

namespace Demo.MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Html5()
        {
            return View();
        }

        public ActionResult jQuery()
        {
            return View();
        }

        public ActionResult Bootstrap()
        {
            return View();
        }

        public ActionResult Submit(string name, string email, string message, string datetime)
        {
            this.ViewBag.Name = name;
            this.ViewBag.Email = email;
            this.ViewBag.Message = message;
            this.ViewBag.DateTime = datetime;

            return View();
        }
    }
}
