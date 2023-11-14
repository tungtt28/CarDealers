using System.Text.RegularExpressions;

namespace CarDealers.Util
{
    public static class Common
    {
        public static string TrimWhenNotNull(this string input)
        {
            return string.IsNullOrEmpty(input) ? input : input.Trim();
        }

        public static string ReplaceSpace(this string input)
        {
            return string.IsNullOrEmpty(input) ? input.TrimWhenNotNull() : Regex.Replace(input, @"\s+", " ");
        }

        public static string CapitalizeFullName(this string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return string.Empty;

            string[] nameParts = fullName.Split(' ');
            for (int i = 0; i < nameParts.Length; i++)
            {
                if (nameParts[i].Length > 0)
                {
                    nameParts[i] = char.ToUpper(nameParts[i][0]) +
                                   (nameParts[i].Length > 1 ? nameParts[i].Substring(1).ToLower() : "");
                }
            }
            return string.Join(' ', nameParts);
        }
    }
}
