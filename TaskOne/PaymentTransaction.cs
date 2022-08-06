namespace TaskOne
{
    public class PaymentTransaction
    {
        public string City { get; set; }
        public decimal Total { get; set; }
        public List<Service> Services { get; set; }
    }
    public class Service
    {
        public string Name { get; set; }
        public decimal Total { get; set; }

        public List<Payer> Payers { get; set; }

    }
    public class Payer
    {
        public string Name { get; set; }
        public decimal Payment { get; set; }
        public string date { get; set; }
        public long Account_number { get; set; }
    }
}
