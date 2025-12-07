namespace TKGroopBG.Models
{
    public class OrderRequest
    {
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }

        public string CartJson { get; set; }
    }
}


