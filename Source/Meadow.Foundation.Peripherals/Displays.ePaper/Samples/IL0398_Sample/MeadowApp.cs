﻿using System;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.ePaper;
using Meadow.Foundation.Graphics;

namespace Displays.ePaper.IL0398_Sample
{
    public class MeadowApp : App<F7FeatherV1>
    {
        //<!=SNIP=>

        Il0398 display;
        MicroGraphics graphics;

        public override Task Initialize()
        {
            Console.WriteLine("Initialize ...");
 
            display = new Il0398(device: Device,
                spiBus: Device.CreateSpiBus(),
                chipSelectPin: Device.Pins.D03,
                dcPin: Device.Pins.D02,
                resetPin: Device.Pins.D01,
                busyPin: Device.Pins.D00);

            graphics = new MicroGraphics(display)
            {
                Rotation = RotationType._270Degrees
            };

            return Task.CompletedTask;
        }

        public override Task Run()
        {
            Console.WriteLine("Run");

            for (int i = 0; i < 100; i++)
            {
                graphics.DrawPixel(i, i, Color.Black);
            }

            graphics.DrawRectangle(10, 40, 160, 60, Color.Black, true);
            graphics.DrawRectangle(20, 80, 200, 90, Color.Yellow, true);

            graphics.CurrentFont = new Font12x16();
            graphics.DrawText(2, 20, "Meadow F7", Color.Black);
            graphics.DrawText(30, 50, "Yellow", Color.Yellow);
            graphics.DrawText(50, 90, "Black", Color.Black);
            graphics.DrawText(50, 120, "White", Color.White);

            graphics.Show();

            Console.WriteLine("Run complete");

            return Task.CompletedTask;
        }

        //<!=SNOP=>
    }
}