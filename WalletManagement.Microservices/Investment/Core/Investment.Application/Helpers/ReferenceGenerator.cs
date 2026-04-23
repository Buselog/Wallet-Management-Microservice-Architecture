using Investment.Application.Services;

namespace Investment.Application.Helpers
{
    public class ReferenceGenerator : IReferenceGenerator
    {
        public string GenerateTradeReference()
        {
            var timestamp = DateTime.UtcNow.Ticks.ToString();

            var randomPart = Guid.NewGuid().GetHashCode().ToString().Substring(1, 3);

            return $"REF-{timestamp.Substring(timestamp.Length - 10)}{randomPart}";
        }
    }
}
