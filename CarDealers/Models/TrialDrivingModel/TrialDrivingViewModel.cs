namespace CarDealers.Models.TrialDrivingModel
{
    public class TrialDrivingViewModel
    {
        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public int CarId { get; set; }

        public DateTime DateBooking { get; set; }

        public string DriverLicense { get; set; } = null!;

        public string? Request { get; set; }
    }
}
