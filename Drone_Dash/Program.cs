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
    
    

