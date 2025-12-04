using System.Text.Json;

namespace KartverketRegister.Utils
{
    //henter høydeverdi fra ekstern URL som returnerer JSON med "value"-felt
    public static class HeightFetcher
    {
        private static readonly HttpClient client = new HttpClient();

        public class HoydeResponse
        {
            public string value { get; set; } // matches the JSON field "value"
        }

        public static async Task<double?> GetHeightFromUrlAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                string json = await response.Content.ReadAsStringAsync();
                var hoyde = JsonSerializer.Deserialize<HoydeResponse>(json);

                if (hoyde == null || string.IsNullOrEmpty(hoyde.value)) return null;

                if (double.TryParse(hoyde.value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double height))
                {
                    return height;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
