﻿using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Lis3Mdl_Sample
{
    public class MeadowApp : App<F7FeatherV1>
    {
        //<!=SNIP=>

        Lis3Mdl sensor;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize hardware...");
            sensor = new Lis3Mdl(Device.CreateI2cBus());

            // classical .NET events can also be used:
            sensor.Updated += HandleResult;

            // Example that uses an IObservable subscription to only be notified when the filter is satisfied
            var consumer = Lis3Mdl.CreateObserver(handler: result => HandleResult(this, result),
                                                 filter: result => FilterResult(result));

            sensor.Subscribe(consumer);

            sensor.StartUpdating(TimeSpan.FromMilliseconds(2000));

            return base.Initialize();
        }

        bool FilterResult(IChangeResult<MagneticField3D> result)
        {
            return result.New.Z > new MagneticField(0.1, MagneticField.UnitType.Gauss);
        }

        void HandleResult(object sender,
            IChangeResult<MagneticField3D> result)
        {
            var mag = result.New;

            Resolver.Log.Info($"MagX={mag.X.Gauss:0.##}gauss, MagY={mag.Y.Gauss:0.##}gauss, GyroZ={mag.Z.Gauss:0.##}gauss");
        }

        //<!=SNOP=>
    }
}