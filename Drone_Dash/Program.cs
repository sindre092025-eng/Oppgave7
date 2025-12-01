using System.Text.RegularExpressions;
using Drone_Dash;
using Drone_Dash.Models;

namespace Drone_Dash;

class Program
{
    public static async Task Main()
    {
       bool job = true;

        while(job)
        {
            Console.WriteLine("Type 'a' for Thread + Join async");
            Console.WriteLine("Type 'b' for Task async");
            Console.WriteLine("Type 'c' for async/await async");
            Console.WriteLine("Type 'd' for async http calls");
            Console.WriteLine("Type 'q' for quitting");
            Console.WriteLine("Select an option!");

            string? input = Console.ReadLine()?.Trim().ToLower();

                
                switch(input)
                {
                    case "a":
                    ThreadRace.CountDroneJoin();
                    break;
                    case "b":
                    var all = TowerSaysGo.StartAllDrones();

                        _ = all.ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                            {
                                Console.WriteLine("One or more drones has failed:");
                                foreach (var inner in t.Exception!.Flatten().InnerExceptions)
                                {
                                    Console.WriteLine($" - {inner.GetType().Name}: {inner.Message}");
                                }
                            }
                            else if (t.IsCanceled)
                            {
                                Console.WriteLine("The operation was interupted.");
                            }
                            else
                            {
                                Console.WriteLine("All drones are complete, without mistakes.");
                            }
                        });

                    Thread.Sleep(8000);
                    break;
                    case "c":
                    await AsyncOrchestration.StartAllDronesAsync();
                    break;
                    case "d":
                    {
                        using var cts = new CancellationTokenSource();

                        _ = Task.Run(() =>
                    {
                        Console.WriteLine("Press 'c' to cancel all drones.");
                        while (true)
                        {
                            var key = Console.ReadKey(true);
                            if (key.KeyChar == 'c' || key.KeyChar == 'C')
                            {
                                Console.WriteLine("Cancelation done by user.");
                                cts.Cancel();
                                break;
                            }
                        }
                    });

                    var droneNames = new[] { "drone1", "drone2", "drone3" };

                    var prepareTasks = new List<Task<DroneModelAPI>>();
                    foreach (var name in droneNames)
                    {
                        prepareTasks.Add(ControlTowerAPI.PrepareDroneAsync(name, cts.Token));
                    }

                    DroneModelAPI[] prepared;
                    try
                    {
                        prepared = await Task.WhenAll(prepareTasks).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during upstart: {ex.Message}");
                        if (ex is AggregateException agg) foreach (var inner in agg.Flatten().InnerExceptions) Console.WriteLine($" - {inner.Message}");
                        return;
                    }

                    var runTasks = new List<Task>();
                    foreach (var d in prepared)
                    {
                        runTasks.Add(DroneRunner.RunAsync(d, cts.Token));
                    }

                    try
                    {
                        await Task.WhenAll(runTasks).ConfigureAwait(false);
                        Console.WriteLine("All drones completed without mistakes.");
                    }
                    catch (Exception)
                    {
                        var every = Task.WhenAll(runTasks);
                        if (every.Exception != null)
                        {
                            Console.WriteLine("One or more drones has failed:");
                            foreach (var inner in every.Exception.Flatten().InnerExceptions)
                            {
                                Console.WriteLine($" - {inner.GetType().Name}: {inner.Message}");
                            }
                        }
                        else
                        {
                        }
                    }
                    break;
                    }
                    case "q":
                    Console.WriteLine("Exiting program");
                    job = false;
                    break;
                    default:
                    Console.WriteLine("Wrong input");
                    break;
                }
            }
        }
        
    } 
    
    

