namespace WebcalAPI.Models
{
    public class ConfirmAccountViewModel
    {
        public int UserId { get; set; }

        public string Code { get; set; }

        public string Password { get; set; }
    }
}