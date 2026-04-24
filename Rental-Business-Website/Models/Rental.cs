using System;

namespace RentalBusinessSystem.Models
{
    public class Rental
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public DateTime RentalStartDate { get; set; }
        public DateTime RentalEndDate { get; set; }
        public int Quantity { get; set; }
        public decimal TotalCost { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string RentalStatus { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
