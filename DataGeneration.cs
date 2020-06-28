using InfluxDB.Client;
using InfluxDB.Client.Writes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimulatedSensor
{
  public static class DataGeneration
  {
    public static async Task GenerateSensorData(Sensor sensor, InfluxDBClient client)
    {
      var range = sensor.Max - sensor.Min;
      var random = new Random();
      var currentValue = (random.NextDouble() * range) + sensor.Min;
      var modifier = Math.Abs(range / 100.0);
      using var writeApi = client.GetWriteApi();
      var dataCache = new List<PointData>();

      Console.WriteLine($"Simulating sensor {sensor.Name} for device {Program.DeviceID} at an interval of {sensor.Interval}ms");
      
      while (true)
      {
        currentValue += modifier * Math.Max(random.NextDouble(), 0.6);
        if (currentValue <= sensor.Min || currentValue >= sensor.Max || random.NextDouble() > Math.Max(Math.Tanh((currentValue - sensor.Min) / range), 0.8))
        {
          modifier *= -1;
        }
        dataCache.Add(PointData
          .Measurement("simulatedValues")
          .Tag("deviceId", Program.DeviceID)
          .Field(sensor.Name, currentValue));

        if (dataCache.Count > 50)
        {
          writeApi.WritePoints(Program.ServerBucket, Program.ServerOrg, dataCache);
          dataCache.Clear();
        }

        await Task.Delay(sensor.Interval);
      }
    }
  }
}
