using April8SimchaFundHw.Data;
using April8SimchaFundHW.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;

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
                Total = db.GetTotal(),
               
                
            };
            if (TempData["success-message"] != null)
            {
                vm.Message = (string)TempData["success-message"];
            }
           
            return View(vm);
        }
        [HttpPost]
        public IActionResult New(Contributor contributor,Deposit deposit)
        {
            var db = new SimchFundDb(_connectionString);
            db.NewContributor(contributor);
            deposit.ContributorId = contributor.Id;
           
            db.AddDeposite(deposit);
            TempData["success-message"] = "new contibutor added successfully!";
            return Redirect("/Contributors");
        }
        [HttpPost]
        public IActionResult Deposit(Deposit deposit)
        {
            var db = new SimchFundDb(_connectionString);
            db.AddDeposite(deposit);
            TempData["success-message"] = "deposit added!";
            return Redirect("/Contributors");
        }
     
        [HttpPost]
        public IActionResult Update(Contributor contributor)
        {
            var db = new SimchFundDb(_connectionString);
            db.UpdateContributor(contributor);
            TempData["success-message"] = "contibutor updated successfully!";
            return Redirect("/Contributors");
        }
        public IActionResult History(int contribid)
        {
            var vm = new ContributorsViewModel();
            var db = new SimchFundDb(_connectionString);
           
          
            vm.Name = db.GetContributorNamebyId(contribid);
            vm.Balance = db.GetBalanceById(contribid);
            List<HistoryTransacions> historyTransacions = new();
            historyTransacions.AddRange(db.DepositsHistory(contribid));
            historyTransacions.AddRange(db.HistoryContributions(contribid));
            vm.HistoryTransacions = historyTransacions.OrderBy(ht => ht.Date).ToList();
          

            return View(vm);
        }
    }
}
