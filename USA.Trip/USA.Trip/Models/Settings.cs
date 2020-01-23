using System;
using System.Collections.Generic;

namespace USA.Trip.Models
{
    public class Settings
    {
        public DateTime UpdateTimestamp { get; set; }

        public Income Budget { get; set; }

        public List<OutcomeEntry> Expenses { get; set; }
    }
}