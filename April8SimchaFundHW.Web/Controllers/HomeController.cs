using April8SimchaFundHw.Data;
using April8SimchaFundHW.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace April8SimchaFundHW.Web.Controllers
{
    public class HomeController : Controller
    {

        private string _connectionString = @"Data Source=10.211.55.2; Initial Catalog=SimchaFund;User Id=sa;Password=Foobar1@;TrustServerCertificate=true;";

        public IActionResult Index()
        {
            var db = new SimchFundDb(_connectionString);
            var vm = new SimchaViewModel
            {
                simchas = db.GetSimchas(),
                ContributorsCount = db.GetCountContributores()

                
            };
            if (TempData["message"] != null)
            {
                vm.Message = (string)TempData["message"];
            }

            return View(vm);
        }
        public IActionResult Contributions(int simchaId)
        {
            var db = new SimchFundDb(_connectionString);
            var vm = new ContributionsViewModel
            {
                contributors = db.GetContributionsForID(),
                SimchaName = db.GetSimchaNamebyId(simchaId),
                SimchaId = simchaId


            };
            return View(vm);
        }
        [HttpPost]
        public IActionResult Updatecontributions(List<Contributions> contributors, int simchaId)
        {
            var db = new SimchFundDb(_connectionString);
            db.UpdateContributions(contributors,simchaId);
            TempData["message"] = "contribution Updated!";
            return Redirect("/");
        }
        [HttpPost]
        public IActionResult New(Simcha simcha)
        {
            var db = new SimchFundDb(_connectionString);
            db.NewSimcha(simcha);
            TempData["message"] = "new Simcha added successfully!";
            return Redirect("/");
        }



    }
}
