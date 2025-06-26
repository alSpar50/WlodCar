// Services/LoyaltyService.cs
using Microsoft.EntityFrameworkCore;
using WlodCar.Data;
using WlodCar.Data.Entities;

namespace WlodCar.Services;

public interface ILoyaltyService
{
    /// <summary>Nalicza punkty = 10 pkt za każde rozpoczęte 100 zł.</summary>
    Task<int> AwardForPriceAsync(Guid userId, decimal totalPrice, string reservationInfo);

    /// <summary>Próbuje wykorzystać pointsToRedeem (>=10 && wielokrotność 10).</summary>
    Task<int> RedeemAsync(Guid userId, int pointsToRedeem, string reservationInfo);
}

public class LoyaltyService : ILoyaltyService
{
    private readonly ApplicationDbContext _db;

    public LoyaltyService(ApplicationDbContext db) => _db = db;

    public async Task<int> AwardForPriceAsync(Guid userId, decimal totalPrice, string desc)
    {
        int points = (int)(Math.Floor(totalPrice / 100m) * 10);
        if (points == 0) return 0;

        var user = await _db.Users.FirstAsync(u => u.Id == userId.ToString());
        user.LoyaltyPoints += points;

        _db.LoyaltyTransactions.Add(new LoyaltyTransaction
        {
            UserId = userId,
            Points = points,
            Type = LoyaltyTransactionType.Earned,
            Description = desc
        });

        await _db.SaveChangesAsync();
        return points;
    }

    public async Task<int> RedeemAsync(Guid userId, int points, string desc)
    {
        if (points % 10 != 0 || points <= 0)
            throw new ArgumentException("Punkty muszą być dodatnią wielokrotnością 10.");

        var user = await _db.Users.FirstAsync(u => u.Id == userId.ToString());

        points = Math.Min(points, user.LoyaltyPoints);   // nie zejdź poniżej zera
        if (points == 0) return 0;

        user.LoyaltyPoints -= points;

        _db.LoyaltyTransactions.Add(new LoyaltyTransaction
        {
            UserId = userId,
            Points = -points,
            Type = LoyaltyTransactionType.Redeemed,
            Description = desc
        });

        await _db.SaveChangesAsync();
        return points;
    }
}