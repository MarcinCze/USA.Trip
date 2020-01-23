using System;

namespace USA.Trip.Models
{
    public class OutcomeEntry
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public PaymentMethod Payment { get; set; }

        public OutcomeEntry() { }

        public OutcomeEntry(string name, double amount, PaymentMethod method)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Name = name;
            this.Amount = amount;
            this.Payment = method;
            this.Date = DateTime.Now;
        }

        public OutcomeEntry(string name, double amount, PaymentMethod method, DateTime date)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Name = name;
            this.Amount = amount;
            this.Payment = method;
            this.Date = date;
        }
    }
}