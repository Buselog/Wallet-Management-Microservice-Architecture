using System.Text;

namespace Wallet.Application.Helpers
{
    public static class IbanHelper
    {
        private const string CountryCode = "TR";

        public static bool Validate(string iban)
        {
            if (string.IsNullOrWhiteSpace(iban)) return false;
            iban = iban.Replace(" ", "").ToUpper();
            if (iban.Length != 26 || !iban.StartsWith(CountryCode)) return false;

            string rearranged = iban.Substring(4) + iban.Substring(0, 4);
            string numeric = ConvertLettersToNumbers(rearranged);
            return CalculateMod97(numeric) == 1;
        }

        public static string CalculateCheckDigits(string bban)
        {
            string rearranged = bban + CountryCode + "00";
            string numeric = ConvertLettersToNumbers(rearranged);
            int remainder = CalculateMod97(numeric);
            int checkDigit = 98 - remainder;

            return checkDigit.ToString("00");
        }

        private static string ConvertLettersToNumbers(string input)
        {
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                sb.Append(char.IsLetter(c) ? (c - 'A' + 10).ToString() : c.ToString());
            }
            return sb.ToString();
        }

        private static int CalculateMod97(string numeric)
        {
            int checksum = 0;
            foreach (char c in numeric)
                checksum = (checksum * 10 + (c - '0')) % 97;
            return checksum;
        }
    }
}
