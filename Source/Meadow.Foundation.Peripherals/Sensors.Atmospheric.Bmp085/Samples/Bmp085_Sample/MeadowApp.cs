﻿using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Atmospheric;
using System;
using System.Threading.Tasks;

namespace Sensors.Atmospheric.Bmp085_Sample
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        //<!—SNIP—>

        Bmp085 sensor;

        public MeadowApp()
        {
            Console.WriteLine("Initializing...");

            sensor = new Bmp085(Device.CreateI2cBus());

            var consumer = Bmp085.CreateObserver(
                handler: result => 
                {
                    Console.WriteLine($"Observer: Temp changed by threshold; new temp: {result.New.Temperature?.Celsius:N2}C, old: {result.Old?.Temperature?.Celsius:N2}C");
                },
                filter: result => 
                {
                    //c# 8 pattern match syntax. checks for !null and assigns var.
                    if (result.Old is { } old) 
                    { 
                        return (
                        (result.New.Temperature.Value - old.Temperature.Value).Abs().Celsius > 0.5); // returns true if > 0.5°C change.
                    }
                    return false;
                }
            );
            sensor.Subscribe(consumer);

            sensor.Updated += (sender, result) => 
            {
                Console.WriteLine($"  Temperature: {result.New.Temperature?.Celsius:N2}C");
                Console.WriteLine($"  Pressure: {result.New.Pressure?.Bar:N2}bar");
            };

            ReadConditions().Wait();

            sensor.StartUpdating(TimeSpan.FromSeconds(1));
        }

        async Task ReadConditions()
        {
            var conditions = await sensor.Read();
            Console.WriteLine($"Temperature: {conditions.Temperature?.Celsius}°C, Pressure: {conditions.Pressure?.Pascal}Pa");
        }

        //<!—SNOP—>
    }
}