using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CruxIT.Library
{
    public static class CxStringHelper
    {
        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256 instance
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash of the input string as bytes
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(input.ToLower());
        }

        public static bool IsEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase);
        }

        public static bool IsPhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            value = value.Trim().Replace(" ", "").Replace("-", "");

            string pattern = @"^(\+)?([0-9]| |-)+";
            return Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase);
        }
    }
}
