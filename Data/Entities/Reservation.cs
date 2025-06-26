// Data/Entities/Reservation.cs
using System.ComponentModel.DataAnnotations;

namespace WlodCar.Data.Entities;

public enum ReservationStatus { Active = 0, Cancelled = 1, Finished = 2 }

public class Reservation
{
    [Key] public int Id { get; set; }
    public Guid UserId { get; set; }
    public string CarId { get; set; } = default!;   // z tabeli Cars
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public decimal TotalPrice { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;
}