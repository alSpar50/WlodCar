// Data/Entities/Car.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WlodCar.Data.Entities;

public class Car
{
    public int Id { get; set; }

    [Required, MaxLength(32)]
    public string RegistrationNumber { get; set; } = default!;

    [Required] public string Brand { get; set; } = default!;
    [Required] public string Model { get; set; } = default!;

    [Range(1, 9)] public int Seats { get; set; }
    [Range(0, 9999)] public decimal PricePerDay { get; set; }

    public bool IsAvailable { get; set; } = true;

    // Nowe pola dla zarządzania flotą
    public DateTime? LastServiceDate { get; set; }
    public DateTime? NextServiceDate { get; set; }
    public string? ServiceNotes { get; set; }
    public bool InService { get; set; } = false;

    // Pole do przechowywania nazwy pliku zdjęcia
    public string? ImageFileName { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = [];

    [NotMapped]
    public string Name => $"{Brand} {Model}";

    [NotMapped]
    public string Img
    {
        get
        {
            // Jeśli mamy wgrane zdjęcie, użyj go
            if (!string.IsNullOrEmpty(ImageFileName))
                return $"/uploads/cars/{ImageFileName}";

            // W przeciwnym razie użyj domyślnego
            var file = Model.Replace(" ", "").ToLowerInvariant() + ".png";
            return $"images/cars/{file}";
        }
    }
}