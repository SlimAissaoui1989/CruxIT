using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CruxIT.Library.GeoLocations
{
    public class CxIPLocation
    {
        public static async Task<CxIPLocation?> GetGeoLocationByIp(string ip)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://ip-api.com/");
                using (HttpResponseMessage response = await client.GetAsync($"json/{ip}"))
                {
                    if (response.IsSuccessStatusCode)
                        return await response.Content.ReadFromJsonAsync<CxIPLocation>();
                    return null;
                }
            }
        }

        public static async Task<CxIPLocation?> GetGeoLocationByIp(IPAddress ip)
        {
            return await GetGeoLocationByIp(ip.ToString());
        }

        private CxIPLocation()
        {
        }

        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("country")]
        public string Country { get; set; } = null!;

        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; } = null!;

        [JsonPropertyName("countryCode3")]
        public string? CountryCode3 { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; } = null!;

        [JsonPropertyName("regionname")]
        public string RegionName { get; set; } = null!;

        [JsonPropertyName("city")]
        public string City { get; set; } = null!;

        [JsonPropertyName("zip")]
        public string Zip { get; set; } = null!;

        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        [JsonPropertyName("timezone")]
        public string TimeZone { get; set; } = null!;

        [JsonPropertyName("isp")]
        public string Isp { get; set; } = null!;

        [JsonPropertyName("org")]
        public string Org { get; set; } = null!;

        [JsonPropertyName("as")]
        public string As { get; set; } = null!;

        [JsonPropertyName("query")]
        public string Query { get; set; } = null!;
    }
}
