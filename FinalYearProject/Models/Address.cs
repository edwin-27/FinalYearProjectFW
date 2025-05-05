namespace FinalYearProject.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int CustomerId {  get; set; }
        
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string line1 { get; set; }
        public string? line2 { get; set; }
        public string townOrCity { get; set;}
        public string postcode {  get; set; }
    }
}
