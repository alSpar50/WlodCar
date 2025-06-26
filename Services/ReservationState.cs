namespace WlodCar.Services

{
    public class ReservationState
    {
        public string? PickupLocation { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public CarVm? SelectedCar { get; set; }
        public List<ExtraVm> Extras { get; } = new();

        public DriverVm? Driver { get; set; }

        public void Reset() => (PickupLocation, PickupDate, ReturnDate,
                                SelectedCar, Driver) = (null, null, null, null, null);
    }

    /* proste DTO do prezentacji – w prawdziwej appce zastąpisz encjami */
    public record CarVm(string Id, string Name, decimal PricePerDay, string Img);
    public record ExtraVm(string Id, string Name, decimal Price);
    public record DriverVm(string FirstName, string LastName, string LicenceNo);
}
