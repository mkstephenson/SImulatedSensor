using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedSensor
{
  public static class DataGeneration
  {
    public static async Task GenerateSensorData(Sensor sensor, string connectionString)
    {
      var range = sensor.Max - sensor.Min;
      var random = new Random();
      var currentValue = (random.NextDouble() * range) + sensor.Min;
      var modifier = Math.Abs(range / 100.0);

      List<string> linesToWrite = new List<string>();
      while (true)
      {
        currentValue += modifier * Math.Max(random.NextDouble(), 0.6);
        if (currentValue <= sensor.Min || currentValue >= sensor.Max || random.NextDouble() > Math.Max(Math.Tanh((currentValue - sensor.Min) / range), 0.8))
        {
          modifier *= -1;
        }
        linesToWrite.Add($"{DateTimeOffset.UtcNow:o};{currentValue}");
        if (linesToWrite.Count > 10)
        {
          File.AppendAllLines($"{sensor.Name}.csv", linesToWrite);
          linesToWrite.Clear();
        }
        Console.WriteLine($"Name: {sensor.Name} - Value: {currentValue}");
        await Task.Delay(sensor.Interval);
      }
    }
  }
}
