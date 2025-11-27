using System;
using Drone_Dash.Models;

namespace Drone_Dash;

public class AsyncOrchestration
{

    public static async Task StartAllDronesAsync()
    {
        var drone1 = new DroneModel("drone1", 5, 300);                   
        var drone2 = new DroneModel("drone2", 6, 200, failureAtCheckpoint: 2);
        var drone3 = new DroneModel("drone3", 4, 400, failureAtCheckpoint: 1);

        var tasks = new[]
        {
            RunDroneAsync(drone1),
            RunDroneAsync(drone2),
            RunDroneAsync(drone3)
        };

        try
        {
            await Task.WhenAll(tasks);
            Console.WriteLine("All drones are done, without mistakes.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("One or more drones failed.");
            var whenAllTask = Task.WhenAll(tasks);
            if (whenAllTask.Exception != null)
            {
                foreach (var inner in whenAllTask.Exception.Flatten().InnerExceptions)
                {
                    Console.WriteLine($" - {inner.GetType().Name}: {inner.Message}");
                }
            }
            else
            {
                Console.WriteLine($" - {ex.GetType().Name}: {ex.Message}");
            }
        }
    }

    private static async Task RunDroneAsync(DroneModel drone)
    {
        Console.WriteLine($"{drone.Name} starts.");

        for (int i = 0; i < drone.MaxCheckpoints; i++)
        {
            Console.WriteLine($"{drone.Name} checkpoint {i + 1}/{drone.MaxCheckpoints}");
            await Task.Delay(drone.DelayMs);

            if (drone.FailureAtCheckpoint >= 0 && i == drone.FailureAtCheckpoint)
                throw new Exception($"{drone.Name}: Engine failure at {i + 1}");
        }

        Console.WriteLine($"{drone.Name} Complete without mistakes.");
    }
}



