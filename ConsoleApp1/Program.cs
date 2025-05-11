using System.Net.Http.Json;

internal class Program
{
    /// <summary>
    ///  dotnet run -- 1000  ↔  создаст 1000 клиентов.
    ///  Переменная окружения CUSTOMER_API задаёт базовый URL (по умолчанию http://localhost:5000).
    /// </summary>
    private static async Task Main(string[] args)
    {
        int count = args.Length > 0 && int.TryParse(args[0], out var n) ? n : 500;
        string baseUrl = //Environment.GetEnvironmentVariable("CUSTOMER_API") ??
            "http://localhost:58413";

        using var client = new HttpClient {BaseAddress = new Uri(baseUrl)};

        Console.WriteLine($"Seeding {count} customers to {baseUrl}/api/customers …");

        for (int i = 0; i < count; i++)
        {
            var dto = new
            {
                firstName = $"Name{i}",
                lastName = $"Test{i}",
                gender = "None",
                email = $"u{i}_{Guid.NewGuid():N}@example.com", // гарантированно уникальный
                dateOfBirth = "1990-01-01"
            };

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/customers", dto);

            if (response.IsSuccessStatusCode)
            {
                // ① читаем JSON {"id":"..."}
                var created = await response.Content.ReadFromJsonAsync<CreatedDto>();
                // ② пишем в CSV: id,len
                if (created == null)
                {
                    Console.WriteLine($"[{i + 1}/{count}] ERR –  {(int)response.StatusCode} but without result");
                    continue;
                }
                await File.AppendAllTextAsync("customers.csv",
                 $"{created.Result.Id},0{Environment.NewLine}"); // len пока 0, обновите при надобности
                Console.WriteLine($"[{i + 1}/{count}] OK  –  {created.Result.Id} {dto.email}");
            }
            else
                Console.WriteLine(
                    $"[{i + 1}/{count}] ERR –  {(int)response.StatusCode} {await response.Content.ReadAsStringAsync()}");
        }

        Console.WriteLine("Done.");
    }

    public record CreatedDto
    {
        public ResultDto Result      { get; init; } = default!;
        public bool      Success     { get; init; }
        public int       StatusCode  { get; init; }
        public string[]  Errors      { get; init; } = Array.Empty<string>();
    }

    public record ResultDto
    {
        public Guid Id { get; init; }
    }
}