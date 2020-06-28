using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SimulatedSensor
{
  class Program
  {
    public static int Main(string[] args)
    {
      var rootCommand = new RootCommand
      {
         new Option<string>("--id", "The ID of the simulated device"),
         new Option<SensorsContainer>("--sensors", "A sensor to emulate: Format of value is name1:min1:max1:interval1;name2:min2:max2:interval2. Name is a string, min/max are doubles and interval is an integer in ms. Sensors are separated by ';,' values are separated by ':'"),
      };

      rootCommand.Handler = CommandHandler.Create<string, SensorsContainer, int>(RunSensorSimulation);

      return rootCommand.Invoke(args);
    }

    public static void RunSensorSimulation(string id, SensorsContainer sensors, int interval)
    {
      List<Task> tasks = new List<Task>();
      foreach (var sensor in sensors.Sensors)
      {
        tasks.Add(Task.Run(() => DataGeneration.GenerateSensorData(sensor, "")));
        
      }
      Task.WaitAll(tasks.ToArray());
    }
  }
}
