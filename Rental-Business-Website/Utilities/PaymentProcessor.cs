using RentalBusinessSystem.Models;
using System;
using System.Collections.Generic;

namespace RentalBusinessSystem.Utilities
{
    public class PaymentProcessor
    {
        public enum PaymentMethod
        {
            CreditCard,
            DebitCard,
            BankTransfer,
            Cash,
            OnlinePayment
        }

        public static class PaymentMethods
        {
            public static List<string> GetAllPaymentMethods()
            {
                return new List<string>
                {
                    "Credit Card",
                    "Debit Card",
                    "Bank Transfer",
                    "Cash",
                    "Online Payment"
                };
            }
        }

        public static decimal CalculateTotalCost(decimal dailyRate, int days, int quantity)
        {
            return dailyRate * days * quantity;
        }

        public static bool ValidatePaymentAmount(decimal amount)
        {
            return amount > 0;
        }

        public static string GenerateTransactionId()
        {
            return "TXN" + DateTime.Now.Ticks.ToString().Substring(0, 12);
        }
    }
}
