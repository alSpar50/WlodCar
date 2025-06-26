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

    public ICollection<Reservation> Reservations { get; set; } = [];

    /* ──────────────────────────────────────────────────────────
       Alias-właściwości tylko do warstwy prezentacji (UI).
       EF Core NIE utworzy dla nich kolumn – dzięki atrybutowi
       [NotMapped].                                                  */

    /// <summary>Pełna nazwa widoczna w ofercie (Brand + Model)</summary>
    [NotMapped]
    public string Name => $"{Brand} {Model}";

    /// <summary>Ścieżka do zdjęcia w wwwroot/images/cars/…</summary>
    [NotMapped]
    public string Img
    {
        get
        {
            // plik np. images/cars/fabia.png
            var file = Model.Replace(" ", "").ToLowerInvariant() + ".png";
            return $"images/cars/{file}";
        }
    }
}