namespace Lider_V_APIServices.Models
{
    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
    }
}
