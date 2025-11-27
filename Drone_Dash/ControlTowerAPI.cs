using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Drone_Dash.Models;

namespace Drone_Dash;

public static class ControlTowerAPI
{

    private static readonly HttpClient _http = new HttpClient
    {
        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/"),
        Timeout = TimeSpan.FromSeconds(4)
    };

    public static async Task<DroneModelAPI> PrepareDroneAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Drone name missing");

        Console.WriteLine($"[Client] Getting plan for {name}");

        int id = Math.Abs(name.GetHashCode()) % 100 + 1;

        var url = $"posts/{id}";
        HttpResponseMessage resp;
        try
        {
            resp = await _http.GetAsync(url, ct).ConfigureAwait(false);
        }
        catch (TaskCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during call to {url}: {ex.Message}", ex);
        }

        if (!resp.IsSuccessStatusCode)
        {
            var text = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new HttpRequestException($"Route call failed: {resp.StatusCode} {text}");
        }

        var post = await resp.Content.ReadFromJsonAsync<PostDto>(cancellationToken: ct).ConfigureAwait(false)
                   ?? throw new InvalidOperationException("Empty response from route API");

        int maxCheckpoints = Math.Clamp(post.title?.Length ?? 0 % 10 + 3, 1, 20);
        int baseDelay = 200 + (post.userId % 5) * 100; // 200..600 ms

        int failureAt = (post.body?.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0) ? 1 : -1;

        Console.WriteLine($"[Client] {name} plan recived: checkpoints={maxCheckpoints}, delay={baseDelay}ms (postId={post.id})");

        return new DroneModelAPI
        {
            Name = name,
            MaxCheckpoints = maxCheckpoints,
            DelayMs = baseDelay,
            FailureAtCheckpoint = failureAt
        };
    }
}

public static class DroneRunner
{
    public static async Task RunAsync(DroneModelAPI drone, CancellationToken ct)
    {
        Console.WriteLine($"[Drone] {drone.Name} starts with {drone.MaxCheckpoints} checkpoints, delay {drone.DelayMs}ms");
        for (int i = 0; i < drone.MaxCheckpoints; i++)
        {
            ct.ThrowIfCancellationRequested();
            Console.WriteLine($"[Drone] {drone.Name} checkpoint {i + 1}/{drone.MaxCheckpoints}");
            await Task.Delay(drone.DelayMs, ct).ConfigureAwait(false);

            if (drone.FailureAtCheckpoint >= 0 && i == drone.FailureAtCheckpoint)
                throw new Exception($"{drone.Name}: engine failure at checkpoint {i + 1}");
        }
        Console.WriteLine($"[Drone] {drone.Name} complete.");
    }
}

