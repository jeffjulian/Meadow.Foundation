# Meadow.Foundation.Sensors.Environmental.DFRobotGravityDOMeter

**DFRobot analog gravity dissolved oxygen sensor**

The **DFRobotGravityDOMeter** library is designed for the [Wilderness Labs](www.wildernesslabs.co) Meadow .NET IoT platform and is part of [Meadow.Foundation](https://developer.wildernesslabs.co/Meadow/Meadow.Foundation/).

The **Meadow.Foundation** peripherals library is an open-source repository of drivers and libraries that streamline and simplify adding hardware to your C# .NET Meadow IoT application.

For more information on developing for Meadow, visit [developer.wildernesslabs.co](http://developer.wildernesslabs.co/).

To view all Wilderness Labs open-source projects, including samples, visit [github.com/wildernesslabs](https://github.com/wildernesslabs/).

## Usage

```csharp
DFRobotGravityDOMeter sensor;

public override Task Initialize()
{
    Resolver.Log.Info("Initialize...");

    sensor = new DFRobotGravityDOMeter(Device.Pins.A01);

    // Example that uses an IObservable subscription to only be notified when the saturation changes
    var consumer = DFRobotGravityDOMeter.CreateObserver(
        handler: result =>
        {
            string oldValue = (result.Old is { } old) ? $"{old.MilligramsPerLiter:n0}" : "n/a";
            string newValue = $"{result.New.MilligramsPerLiter:n0}";
            Resolver.Log.Info($"New: {newValue}mg/l, Old: {oldValue}mg/l");
        },
        filter: null
    );
    sensor.Subscribe(consumer);

    // optional classical .NET events can also be used:
    sensor.Updated += (sender, result) =>
    {
        string oldValue = (result.Old is { } old) ? $"{old.MilligramsPerLiter}mg/l" : "n/a";
        Resolver.Log.Info($"Updated - New: {result.New.MilligramsPerLiter:n0}mg/l, Old: {oldValue}");
    };

    return Task.CompletedTask;
}

public override async Task Run()
{
    Resolver.Log.Info("Run...");

    await ReadSensor();

    sensor.StartUpdating(TimeSpan.FromSeconds(2));
}

protected async Task ReadSensor()
{
    var concentration = await sensor.Read();
    Resolver.Log.Info($"Initial concentration: {concentration.MilligramsPerLiter:N0}mg/l");
}

```
## How to Contribute

- **Found a bug?** [Report an issue](https://github.com/WildernessLabs/Meadow_Issues/issues)
- Have a **feature idea or driver request?** [Open a new feature request](https://github.com/WildernessLabs/Meadow_Issues/issues)
- Want to **contribute code?** Fork the [Meadow.Foundation](https://github.com/WildernessLabs/Meadow.Foundation) repository and submit a pull request against the `develop` branch


## Need Help?

If you have questions or need assistance, please join the Wilderness Labs [community on Slack](http://slackinvite.wildernesslabs.co/).