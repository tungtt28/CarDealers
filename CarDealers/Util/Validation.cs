using System.Text.RegularExpressions;

namespace CarDealers.Util
{
    public static class Validation
    {
        public static bool CheckPassword(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\p{P}).{6,}$";

            return Regex.IsMatch(password, pattern);
        }

        public static bool CheckEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(email, pattern);
        }

        public static bool CheckFullName(string fullName)
        {
            string pattern = @"^[a-zA-Z\s\u00C0-\u1EF9]+$";

            return Regex.IsMatch(fullName, pattern);
        }

        public static bool CheckPhone(string phone)
        {
            string pattern = @"^0\d{9}$"; 

            return Regex.IsMatch(phone, pattern);
        }

        public static bool CheckDriverLicense(string driverLicense)
        {
            string pattern = @"^\d{12}$";

            return Regex.IsMatch(driverLicense, pattern);
        }

        public static bool CheckBookingDate(DateTime dateBooking)
        {
            if(dateBooking <= DateTime.Now)
            {
                return false;
            }
            return true;
        }
    }
}
