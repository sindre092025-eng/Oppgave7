/* using System;
using Drone_Dash.Models;
using Microsoft.VisualBasic;

namespace Drone_Dash;

public class ThreadRace
{
    public static void CountDroneJoin()
    {
        Console.WriteLine("Drones are on their way!");
        
        var drone1 = new DroneModel("drone1", 10, 1000);
        var thread1 = new Thread(ThreadCounter);
        thread1.Start(drone1);

        thread1.Join();

        var drone2 = new DroneModel("drone2", 12, 1250);
        var thread2 = new Thread(ThreadCounter);
        thread2.Start(drone2);

        thread2.Join();

        Console.WriteLine("Drones finished!");
        
    }

    private static void ThreadCounter(object? state)
    {
        var counter = (DroneModel)state!;

        Console.WriteLine($"The {counter.Name}: Started...");

        for (int i = 0; i <= counter.MaxCheckpoints; i++)
        {
            Thread.Sleep(counter.DelayMs);
            Console.WriteLine($"{counter.Name} has completed {i} of {counter.MaxCheckpoints}...");
        }

        Console.WriteLine($"Drone {counter.Name} has finished");
    }
}
 */