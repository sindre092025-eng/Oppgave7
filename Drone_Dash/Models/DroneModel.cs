using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Drone_Dash.Models;

public class DroneModel
{
    public string Name {get;}
    public int MaxCheckpoints {get;}
    public int DelayMs {get;}
    public int FailureAtCheckpoint {get;}

    public DroneModel(string name, int maxCheckpoints, int delayMs, int failureAtCheckpoint = -1)
    {
        Name = name;
        MaxCheckpoints = maxCheckpoints;
        DelayMs = delayMs;
        FailureAtCheckpoint = failureAtCheckpoint;
    }
}

public class DroneModelAPI
{
    public string Name { get; init; } = "";
    public int MaxCheckpoints { get; set; }
    public int DelayMs { get; set; }
    public int FailureAtCheckpoint { get; init; } = -1;
}
