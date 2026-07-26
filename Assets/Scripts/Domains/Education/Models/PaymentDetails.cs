namespace SReader.Domains.Education.Models
{
    /// <summary>
    /// A tutor's single source of truth for receiving payments. Shown to
    /// students so they can pay for a module/subject and submit proof. For now
    /// this is FNB bank + Orange Money (Botswana).
    /// </summary>
    public class PaymentDetails
    {
        public string UserId { get; set; }
        public string AccountName { get; set; }
        public string FnbAccount { get; set; }
        public string OrangeMoney { get; set; }

        public bool IsEmpty =>
            string.IsNullOrWhiteSpace(FnbAccount) && string.IsNullOrWhiteSpace(OrangeMoney);
    }
}
