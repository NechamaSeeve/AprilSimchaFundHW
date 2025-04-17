using April8SimchaFundHw.Data;

namespace April8SimchaFundHW.Web.Models
{
    public class ContributorsViewModel
    {
        public List<Contributor> Contributors { get; set; }
        public int Total { get; set; }
        public List<HistoryTransacions> HistoryTransacions { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set;}
        
    }
}
