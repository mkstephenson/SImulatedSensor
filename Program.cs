using InfluxDB.Client;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace SimulatedSensor
{
  public static class Program
  {
    public static string DeviceID;
    public static string ServerBucket;
    public static string ServerOrg;

    public static int Main(string[] args)
    {
      var rootCommand = new RootCommand
      {
         new Option<string>("--id", "The ID of the simulated device"),
         new Option<SensorsContainer>("--sensors", "A sensor to emulate: Format of value is name1:min1:max1:interval1;name2:min2:max2:interval2. Name is a string, min/max are doubles and interval is an integer in ms. Sensors are separated by ';,' values are separated by ':'"),
         new Option<string>("--serverUrl", "The URL of the server"),
         new Option<string>("--serverToken", "The token for the server"),
         new Option<string>("--serverBucket", "The bucket on the server"),
         new Option<string>("--serverOrg", "The organisation on the server")
      };

      rootCommand.Handler = CommandHandler.Create<string, SensorsContainer, string, string, string, string>(RunSensorSimulation);

      return rootCommand.Invoke(args);
    }

    public static void RunSensorSimulation(string id, SensorsContainer sensors, string serverUrl, string serverToken, string serverBucket, string serverOrg)
    {
      Program.DeviceID = id;
      Program.ServerBucket = serverBucket;
      Program.ServerOrg = serverOrg;

      using var influxDBClient = InfluxDBClientFactory.Create(serverUrl, serverToken.ToCharArray());
      influxDBClient.DisableGzip();
      List<Task> tasks = new List<Task>();
      foreach (var sensor in sensors.Sensors)
      {
        tasks.Add(Task.Run(() => DataGeneration.GenerateSensorData(sensor, influxDBClient)));
      }
      Task.WaitAll(tasks.ToArray());
    }
  }
}
