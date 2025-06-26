using Microsoft.EntityFrameworkCore;
using WlodCar.Data.Entities;

namespace WlodCar.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.Cars.AnyAsync())          // już są – nic nie rób
            return;

        var cars = new[]
        {
            new Car
            {
                RegistrationNumber = "DW 1ABC1",
                Brand  = "Skoda",
                Model  = "Fabia",
                Seats  = 5,
                PricePerDay = 110,
                IsAvailable  = true
            },
            new Car
            {
                RegistrationNumber = "DW 2ABC2",
                Brand  = "Audi",
                Model  = "A3",
                Seats  = 5,
                PricePerDay = 145,
                IsAvailable  = true
            },
            new Car
            {
                RegistrationNumber = "DW 3ABC3",
                Brand  = "Mercedes",
                Model  = "GLA",
                Seats  = 5,
                PricePerDay = 180,
                IsAvailable  = true
            }
        };

        db.Cars.AddRange(cars);
        await db.SaveChangesAsync();
    }
}