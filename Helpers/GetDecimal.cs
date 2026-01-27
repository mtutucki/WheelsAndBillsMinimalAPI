using System.Globalization;
using System.Text.Json;

namespace WheelsAndBillsAPI.Helpers
{
    public  class GetDecimal
    {
        public static decimal GetDecimalByJsonElement(JsonElement el)
        {
            return el.ValueKind switch
            {
                JsonValueKind.Number => el.GetDecimal(),
                JsonValueKind.String => decimal.Parse(
                    el.GetString()!,
                    CultureInfo.InvariantCulture
                ),
                _ => throw new InvalidOperationException(
                    $"Expected number or string, got {el.ValueKind}"
                )
            };
        }
    }
}
