using April8SimchaFundHw.Data;

namespace April8SimchaFundHW.Web.Models
{
    public class ContributionsViewModel
    {
        public List<Contributions> contributors { get; set; }
        public string SimchaName { get; set; }
        public int SimchaId { get; set; }
        public Contributor Contributor { get; set; }

    }
}
