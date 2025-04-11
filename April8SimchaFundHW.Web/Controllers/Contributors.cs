using April8SimchaFundHw.Data;
using April8SimchaFundHW.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace April8SimchaFundHW.Web.Controllers
{
    public class Contributors : Controller
    {
        private string _connectionString = @"Data Source=10.211.55.2; Initial Catalog=SimchaFund;User Id=sa;Password=Foobar1@;TrustServerCertificate=true;";
        public IActionResult Index()
        {
            var db = new SimchFundDb(_connectionString);
            var vm = new ContributorsViewModel
            {
                Contributors = db.GetContributors(),
                Total = db.GetTotal()
            };
            return View(vm);
        }
        [HttpPost]
        public IActionResult New(Contributor contributor,Deposit deposit)
        {
            var db = new SimchFundDb(_connectionString);
            db.NewContributor(contributor);
            deposit.ContributorId = contributor.Id;
           
            db.AddDeposite(deposit);
            return Redirect("/Contributors");
        }
        [HttpPost]
        public IActionResult Deposit(Deposit deposit)
        {
            var db = new SimchFundDb(_connectionString);
            db.AddDeposite(deposit);
            return Redirect("/Contributors");
        }
     
        [HttpPost]
        public IActionResult Update(Contributor contributor)
        {
            var db = new SimchFundDb(_connectionString);
            db.UpdateContributor(contributor);
            return Redirect("/Contributors");
        }
        public IActionResult History(int contribid)
        {
            return View();
        }
    }
}
