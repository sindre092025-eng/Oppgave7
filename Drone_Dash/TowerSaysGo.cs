 using System;
using Drone_Dash.Models;

namespace Drone_Dash;

public class TowerSaysGo
{
    public static Task StartAllDrones()
    {
        var drone1 = new DroneModel("drone1", 5, 800);
        var drone2 = new DroneModel("drone2", 7, 500);
        var drone3 = new DroneModel("drone3", 8, 700, failureAtCheckpoint: 3);

        var task1 = RunDrone(drone1);
        var task2 = RunDrone(drone2);
        var task3 = RunDrone(drone3);

        return Task.WhenAll(task1, task2, task3);
    }

    private static Task RunDrone(DroneModel drone)
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        Task.Run(() =>
        {
            try
            {
                Console.WriteLine($"{drone.Name} starts.");

                for (int i = 0; i < drone.MaxCheckpoints; i++)
                {
                    Console.WriteLine($"{drone.Name} checkpoint {i + 1}/{drone.MaxCheckpoints}");

                    Task.Delay(drone.DelayMs).GetAwaiter().GetResult();

                    if (drone.FailureAtCheckpoint >= 0 && i == drone.FailureAtCheckpoint)
                    {
                        throw new Exception($"{drone.Name}: Engine failure at checkpoint {i + 1}");
                    }
                }

                tcs.TrySetResult(true);
                Console.WriteLine($"{drone.Name} Complete, without mistakes.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{drone.Name} Thrown exeption: {ex.GetType().Name}: {ex.Message}");
                tcs.TrySetException(ex);
            }
        });

        return tcs.Task;
    }
}
         