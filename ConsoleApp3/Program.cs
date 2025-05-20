using System.Net.Http.Json;

namespace ConsoleApp3;

/// dotnet run -- 200 32
///   200  – сколько кастомеров
///   32   – (опц.) степень параллелизма
internal class Program
{
    private const int CustomerCount = 160;
    private const int UpdatesPerCustomer = 120;

    private static async Task Main(string[] args)
    {
        int total = args.Length > 0 && int.TryParse(args[0], out var n) ? n : CustomerCount;
        int degree = args.Length > 1 && int.TryParse(args[1], out var d)
            ? d
            : 5;

        string baseUrl = Environment.GetEnvironmentVariable("CUSTOMER_API")
                         ?? "http://localhost:80";

        using var client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout     = TimeSpan.FromMinutes(5)
        };

        Console.WriteLine($"Seeding {total} customers × {UpdatesPerCustomer} updates " +
                          $"(parallel = {degree}) …");

        // Очистим csv
        await File.WriteAllTextAsync("customers.csv", "id,totalEvents\n");
        object csvLock = new();                       // защита записи в файл

        // ------- параллельное создание ----------
        await Parallel.ForEachAsync(
            Enumerable.Range(1, total),
            new ParallelOptions { MaxDegreeOfParallelism = degree },
            async (idx, ct) =>
            {
                // 1. CREATE -------------------------
                var createDto = new
                {
                    firstName   = $"Name{idx}",
                    lastName    = $"Test{idx}",
                    gender      = "None",
                    email       = $"u{idx}_{Guid.NewGuid():N}@example.com",
                    dateOfBirth = "1990-01-01"
                };

                HttpResponseMessage createResp =
                    await client.PostAsJsonAsync("/api/customers", createDto, ct);

                if (!createResp.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[{idx}] CREATE ERR – {(int)createResp.StatusCode}");
                    return;
                }

                var created = await createResp.Content.ReadFromJsonAsync<CreatedDto>(cancellationToken: ct);
                if (created?.Result == null)
                {
                    Console.WriteLine($"[{idx}] CREATE ERR – пустое тело");
                    return;
                }

                Guid customerId = created.Result.Id;
                Console.WriteLine($"[{idx}] CREATE OK  – {customerId}");

                lock (csvLock)
                {
                    File.AppendAllText("customers.csv",
                        $"{customerId},{1 + UpdatesPerCustomer}{Environment.NewLine}");
                }

                // 2. UPDATEs -------------------------
                for (int u = 1; u <= UpdatesPerCustomer; u++)
                {
                    var updateDto = new { id = customerId, email = $"{customerId}_v{u}@example.com" };

                    HttpResponseMessage updateResp =
                        await client.PutAsJsonAsync("/api/customers", updateDto, ct);

                    if (!updateResp.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"    ↳ [{idx}] UPDATE {u} ERR – {(int)updateResp.StatusCode}");
                    }
                }
            });

        Console.WriteLine("Done.");
    }

    // ----- модели ответа --------------------------
    public record CreatedDto
    {
        public ResultDto Result   { get; init; } = default!;
        public bool      Success  { get; init; }
        public int       StatusCode { get; init; }
        public string[]  Errors   { get; init; } = Array.Empty<string>();
    }
    public record ResultDto
    {
        public Guid Id { get; init; }
    }
}