using System;
namespace BackendAPI.Extentions
{
    public static class DatetimeExtensions
    {
        public static int CalculateAge(this DateTime birthday)
        {
            DateTime today = DateTime.Now;

            int age = today.Year - birthday.Year;

            if (birthday.Date > today.AddYears(-age)) age--;

            return age;
        }
    }
}
