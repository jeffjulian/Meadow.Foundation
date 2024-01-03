﻿using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Displays.Tft.Ili9341_Sample
{
    public class MeadowApp : App<F7FeatherV2>
    {
        //<!=SNIP=>

        MicroGraphics graphics;
        Ili9341 display;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initializing ...");

            Resolver.Log.Info("Create display driver instance");

            display = new Ili9341
            (
                spiBus: Device.CreateSpiBus(),
                chipSelectPin: Device.Pins.D13,
                dcPin: Device.Pins.D14,
                resetPin: Device.Pins.D15,
                width: 240, height: 320
            );

            graphics = new MicroGraphics(display)
            {
                IgnoreOutOfBoundsPixels = true,
                CurrentFont = new Font12x16()
            };

            return base.Initialize();
        }

        public override Task Run()
        {
            graphics.Clear();

            graphics.DrawTriangle(10, 30, 50, 50, 10, 50, Color.Red);
            graphics.DrawRectangle(20, 45, 40, 20, Color.Yellow, false);
            graphics.DrawCircle(50, 50, 40, Color.Blue, false);
            graphics.DrawText(5, 5, "Meadow F7", Color.White);

            graphics.Show();

            DisplayTest();

            return base.Run();
        }

        //<!=SNOP=>

        void DisplayTest()
        {
            while (true)
            {
                //   PartialUpdate();

                //   Thread.Sleep(3000);

                graphics.Clear();

                CharacterTest();

                Thread.Sleep(3000);

                DrawMeadowLogo();

                Thread.Sleep(3000);

                FontTest();

                Thread.Sleep(3000);

                TestDisplay();
            }
        }

        void PartialUpdate()
        {
            graphics.Clear(true);
            graphics.DrawRectangle(0, 0, 240, 320, Color.Teal, true);
            //   graphics.Show(0, 0, 240, 10);

            for (int x = 0; x < 200; x += 20)
            {
                for (int y = 0; y < 300; y += 20)
                {
                    display.Show(x, y, x + 20, y + 20);
                }
            }
        }

        void DrawMeadowLogo()
        {
            graphics.Clear();

            var bottom = 200;
            var height = 54;

            graphics.DrawLine(4, bottom, 44, bottom - height, Color.White);
            graphics.DrawLine(4, bottom, 44, bottom, Color.White);
            graphics.DrawLine(44, 200 - height, 64, bottom - height / 2, Color.White);
            graphics.DrawLine(44, bottom, 84, bottom - height, Color.White);
            graphics.DrawLine(84, bottom - height, 124, bottom, Color.White);

            //mountain fill
            int lineWidth, x, y;

            for (int i = 0; i < height - 1; i++)
            {
                y = bottom - i;
                x = 5 + i * 20 / 27;

                //fill bottom of mountain
                if (i < height / 2)
                {
                    lineWidth = 38;
                    graphics.DrawLine(x, y, x + lineWidth, y, Color.YellowGreen);
                }
                else
                { //fill top of mountain
                    lineWidth = 38 - (i - height / 2) * 40 / 27;
                    graphics.DrawLine(x, y, x + lineWidth, y, Color.YellowGreen);
                }
            }

            graphics.Show();

        }

        void CharacterTest()
        {
            graphics.Clear();

            graphics.CurrentFont = new Font12x20();

            string msg = string.Empty;

            int yPos = 12;
            int count = 0;

            for (int i = 32; i < 254; i++)
            {
                if (i == 127)
                    i += 33;

                if (count >= 18 || i >= 254)
                {
                    Resolver.Log.Info(msg);

                    graphics.DrawText(12, yPos, msg, Color.LawnGreen);

                    yPos += 24;

                    count = 0;
                    msg = string.Empty;
                }

                msg += (char)(i);
                Resolver.Log.Info($"i = {i}");
                count++;
            }

            graphics.Show();
        }

        void FontTest()
        {
            graphics.Clear();

            int yPos = 0;

            graphics.CurrentFont = new Font4x8();
            graphics.DrawText(0, yPos, "Font_4x8: ABCdef123@#$", Color.Red);
            yPos += 12;

            graphics.CurrentFont = new Font8x8();
            graphics.DrawText(0, yPos, "Font_8x8: ABCdef123@#$", Color.Orange);
            yPos += 12;

            graphics.CurrentFont = new Font8x12();
            graphics.DrawText(0, yPos, "Font_8x12: ABCdef123@#$", Color.Yellow);
            yPos += 16;

            graphics.CurrentFont = new Font12x16();
            graphics.DrawText(0, yPos, "Font_12x16: ABCdef123@#$", Color.LawnGreen);
            yPos += 20;

            graphics.CurrentFont = new Font12x20();
            graphics.DrawText(0, yPos, "Font_12x20: ABCdef123@#$", Color.Cyan);
            yPos += 22;

            graphics.Show();
        }

        void TestDisplay()
        {
            //force a collection
            GC.Collect();

            Resolver.Log.Info("Draw");

            for (int i = 0; i < 30; i++)
            {
                display.DrawPixel(i, 120 + i, true);
                display.DrawPixel(30 + i, 120 + i, true);
                display.DrawPixel(60 + i, 120 + i, true);
            }

            // Draw with Display Graphics Library
            graphics.CurrentFont = new Font8x8();
            graphics.Clear();
            graphics.DrawTriangle(10, 10, 50, 50, 10, 50, Color.Red);
            graphics.DrawRectangle(20, 15, 40, 20, Color.Yellow, false);
            graphics.DrawCircle(50, 50, 40, Color.Blue, false);

            graphics.DrawText(5, 5, "Meadow F7 SPI", Color.White);
            graphics.Show();
        }
    }
}