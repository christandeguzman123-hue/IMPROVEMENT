using System;

namespace RentalBusinessSystem.Models
{
    public class RentalItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal DailyRate { get; set; }
        public int QuantityAvailable { get; set; }
        public string Category { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
