using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace SimulatedSensor
{
  public class Sensor
  {
    public Sensor(string value)
    {
      var parameters = value.Split(':');
      Name = parameters[0];
      Min = double.Parse(parameters[1]);
      Max = double.Parse(parameters[2]);
      Interval = int.Parse(parameters[3]);
    }

    public string Name { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public int Interval { get; set; }
  }

  public class SensorsContainer
  {
    public readonly Sensor[] Sensors;
    public SensorsContainer(string value)
    {
      var sensors = value.Split(';').ToList();
      Sensors = sensors.Select(i => new Sensor(i)).ToArray();
    }
  }
}
