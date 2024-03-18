// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.Json;
using CurrencyWorker;

// https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?valcode=EUR&date=20240302&json

// Send the data for a different days of February
for (int i = 1; i < 10; i++)
{
    var (rate, currency) = await GetExchangeRate("0"+i);
    await SendAnalyticsRequest(rate, currency);
}


        
static async Task<(double rate, string cur)> GetExchangeRate(string date)
{
    var apiUrl = $"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?valcode=USD&date=202402{date}&json";

    using (var client = new HttpClient())
    {
        var response = await client.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            List<CurrencyResponse> currencyResponses = JsonSerializer.Deserialize<List<CurrencyResponse>>(jsonResponse);
            Console.WriteLine("Response received successfully:");
            Console.WriteLine(jsonResponse);
            var result = currencyResponses.First();
            return (result.rate, result.cc);
        }
        else
        {
            Console.WriteLine($"Failed to get exchange rate. Status code: {response.StatusCode}");
            return (0, null)!;
        }
    }
}
static async Task SendAnalyticsRequest(double rate, string cur)
{
    var apiUrl = "https://www.google-analytics.com/mp/collect?api_secret=tJIXaCNUSbCmZjGF3_zvSw&measurement_id=G-9CLXQYV8YP";
    var payload = $@"{{
        ""client_id"": ""881563173.1710270852"",
        ""timestamp_micros"": ""1710433823023000"",
        ""non_personalized_ads"": false,
        ""events"": [
            {{
                ""name"": ""currency_info12"",
                ""params"": {{
                    ""items"": [],
                    ""USD"": ""{rate}""
                }}
            }}
        ]
    }}";
    using (var client = new HttpClient())
    {
        client.DefaultRequestHeaders.Add("Host", "www.google-analytics.com");

        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Analytics request sent successfully.");
        }
        else
        {
            Console.WriteLine($"Failed to send analytics request. Status code: {response.StatusCode}");
        }
    }
}
