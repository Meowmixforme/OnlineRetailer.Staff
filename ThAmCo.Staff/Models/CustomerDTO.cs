namespace ThAmCo.Web.Models
{
    public class CustomerDTO
    {

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string AddressLine1 { get; set; } = null!;
        public string AddressLine2 { get; set; } = null!;
        public string City { get; set; } = null!;
        public string County { get; set; } = null!;
        public string PostCode { get; set; } = null!;
        public double AvailableFunds { get; set; }
    }
}