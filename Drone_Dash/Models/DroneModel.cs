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