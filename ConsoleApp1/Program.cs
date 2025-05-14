using System.Net.Http.Json;

/// dotnet run -- 200          ← создаст 200 клиентов * 100 модификаций = 20 200 событий
internal class Program
{
    private const int UpdatesPerCustomer = 100; // ← сколько UPDATE‑событий

    private static async Task Main(string[] args)
    {
        int count = args.Length > 0 && int.TryParse(args[0], out var n) ? n : 500;
        string baseUrl = Environment.GetEnvironmentVariable("CUSTOMER_API")
                         ?? "http://localhost:80";

        using var client = new HttpClient {BaseAddress = new Uri(baseUrl)};

        Console.WriteLine(
            $"Seeding {count} customers (each +{UpdatesPerCustomer} updates) to {baseUrl}/api/customers …");

        for (int i = 0; i < count; i++)
        {
            // ---------- 1. CREATE ----------
            var createDto = new
            {
                firstName = $"Name{i}",
                lastName = $"Test{i}",
                gender = "None",
                email = $"u{i}_{Guid.NewGuid():N}@example.com",
                dateOfBirth = "1990-01-01"
            };

            HttpResponseMessage createResp =
                await client.PostAsJsonAsync("/api/customers", createDto);

            if (!createResp.IsSuccessStatusCode)
            {
                Console.WriteLine($"[{i + 1}/{count}] CREATE ERR – HTTP {(int)createResp.StatusCode}");
                continue; // переходим к следующему клиенту
            }

            var created = await createResp.Content.ReadFromJsonAsync<CreatedDto>();
            if (created?.Result is null)
            {
                Console.WriteLine($"[{i + 1}/{count}] CREATE ERR – пустое тело");
                continue;
            }

            Guid customerId = created.Result.Id;

            // сохраним CustomerId в CSV (len = 1 + UpdatesPerCustomer)
            await File.AppendAllTextAsync("customers.csv",
                $"{customerId},{1 + UpdatesPerCustomer}{Environment.NewLine}");

            Console.WriteLine($"[{i + 1}/{count}] CREATE OK – {customerId}");

            // ---------- 2. 100 UPDATE‑событий ----------
            for (int u = 1; u <= UpdatesPerCustomer; u++)
            {
                var updateDto = new {id = customerId, email = $"{customerId}_v{u}@example.com"};

                HttpResponseMessage updateResp =
                    await client.PutAsJsonAsync("/api/customers", updateDto);

                if (!updateResp.IsSuccessStatusCode)
                {
                    Console.WriteLine($"    ↳ UPDATE {u}/{UpdatesPerCustomer} ERR – HTTP {(int)updateResp.StatusCode}");
                    // при необходимости break; но лучше попытаться остальные
                }
            }
        }

        Console.WriteLine("Done.");
    }


    // ----- модели ответа ----------------------------------------------------
    public record CreatedDto
    {
        public ResultDto Result { get; init; } = default!;
        public bool Success { get; init; }
        public int StatusCode { get; init; }
        public string[] Errors { get; init; } = Array.Empty<string>();
    }

    public record ResultDto
    {
        public Guid Id { get; init; }
    }
}