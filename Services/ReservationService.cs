// Services/ReservationService.cs
using Microsoft.EntityFrameworkCore;
using WlodCar.Data;
using WlodCar.Data.Entities;

namespace WlodCar.Services;

public interface IReservationService
{
    Task<bool> IsCarAvailable(string carId, DateTime from, DateTime to);

    /* -------------- UWAGA: opcjonalny parametr ------------------
       Jeżeli go nie podasz (tak jak dotąd) – weźmie wartość 0 i
       zachowa się identycznie jak Twoja wcześniejsza wersja.       */
    Task<Reservation> CreateAsync(Reservation reservation, int pointsToRedeem = 0);

    Task<List<Reservation>> GetForUserAsync(Guid userId);
    Task CancelAsync(int reservationId, Guid userId);
}

public class ReservationService : IReservationService
{
    private readonly ApplicationDbContext _db;
    private readonly ILoyaltyService _loyalty;

    public ReservationService(ApplicationDbContext db, ILoyaltyService loyalty)
        => (_db, _loyalty) = (db, loyalty);

    /* ----------------- dostępność auta ------------------------- */
    public Task<bool> IsCarAvailable(string carId, DateTime from, DateTime to)
        => _db.Reservations
              .Where(r => r.CarId == carId && r.Status == ReservationStatus.Active)
              .AllAsync(r => r.DateTo <= from || r.DateFrom >= to);

    /* ----------------- tworzenie rezerwacji -------------------- */
    public async Task<Reservation> CreateAsync(Reservation r, int pointsToRedeem /* =0 */)
    {
        /* --- rabat 1 % za każde 10 pkt ------------------------- */
        var discountPercent = pointsToRedeem / 10;          // 30 pkt → 3 %
        var discountFactor = 1m - (discountPercent / 100m);
        r.TotalPrice = Math.Round(r.TotalPrice * discountFactor, 2);

        _db.Reservations.Add(r);
        await _db.SaveChangesAsync();

        /* --- program lojalnościowy ----------------------------- */
        if (pointsToRedeem > 0)
            await _loyalty.RedeemAsync(r.UserId, pointsToRedeem,
                                       $"Rabat za rezerwację #{r.Id}");

        await _loyalty.AwardForPriceAsync(r.UserId, r.TotalPrice,
                                          $"Rezerwacja #{r.Id}");

        return r;
    }

    /* ----------------- lista rezerwacji użytkownika ----------- */
    public Task<List<Reservation>> GetForUserAsync(Guid userId)
        => _db.Reservations
              .Where(r => r.UserId == userId)
              .OrderByDescending(r => r.DateFrom)
              .ToListAsync();

    /* ----------------- anulowanie ----------------------------- */
    public async Task CancelAsync(int id, Guid userId)
    {
        var r = await _db.Reservations
                         .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (r is not null && r.Status == ReservationStatus.Active)
        {
            r.Status = ReservationStatus.Cancelled;
            await _db.SaveChangesAsync();
        }
    }
}