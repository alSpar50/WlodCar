// Data/Entities/LoyaltyTransaction.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace WlodCar.Data.Entities
{
    /// <summary>
    /// Pojedynczy ruch na koncie programu lojalnościowego.
    /// Pozytywne Points → naliczenie, ujemne → wykorzystanie.
    /// </summary>
    public enum LoyaltyTransactionType { Earned = 0, Redeemed = 1 }

    public class LoyaltyTransaction
    {
        [Key] public int Id { get; set; }

        public Guid UserId { get; set; }

        /// <summary>
        /// Liczba punktów (+ naliczenie, – wykorzystanie)
        /// </summary>
        public int Points { get; set; }

        public LoyaltyTransactionType Type { get; set; }

        /// <summary>
        /// Krótki opis (np. „Rezerwacja #37”)
        /// </summary>
        public string? Description { get; set; }

        public DateTime DateUtc { get; set; } = DateTime.UtcNow;
    }
}