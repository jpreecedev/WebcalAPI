namespace Webcal.Model
{
    public class MobileAppRequestViewModel<T>
    {
        public string Username { get; set; }

        public string EmailAddress { get; set; }

        public string Thumbprint { get; set; }

        public T Data { get; set; }
    }
}