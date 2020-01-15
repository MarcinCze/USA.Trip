namespace USA.Trip
{
    public class OutcomeEntry
    {
        public string Name { get; set; }
        public double Amount { get; set; }

        public OutcomeEntry() { }

        public OutcomeEntry(string name, double amount)
        {
            this.Name = name;
            this.Amount = amount;
        }
    }
}