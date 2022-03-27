﻿using System.Threading;
using Meadow.Hardware;

namespace Meadow.Foundation.Displays.Lcd
{
    public class GroveCharacterDisplay : I2cCharacterDisplay
    {
        public GroveCharacterDisplay(II2cBus i2cBus,
            byte address = (byte)Addresses.Address_0x3E,
            byte rows = 2, byte columns = 16)
            : base(i2cBus, address, rows, columns)
        {
        }

        protected override void Initialize()
        {
            var displayFunction = (byte)(LCD_4BITMODE | LCD_1LINE | LCD_5x8DOTS);

            if (DisplayConfig.Height > 1)
            {
                displayFunction |= LCD_2LINE;
            }

            Thread.Sleep(50);

            Command((byte)((byte)I2CCommands.LCD_FUNCTIONSET | displayFunction));
            Thread.Sleep(50);

            Command((byte)((byte)I2CCommands.LCD_FUNCTIONSET | displayFunction));
            Thread.Sleep(2);

            Command((byte)((byte)I2CCommands.LCD_FUNCTIONSET | displayFunction));

            Command((byte)((byte)I2CCommands.LCD_FUNCTIONSET | displayFunction));

            // turn the display on with no cursor or blinking default
            displayControl = (byte)(LCD_DISPLAYON | LCD_CURSOROFF | LCD_BLINKOFF);
            DisplayOn();

            // clear it off
            ClearLines();

            // Initialize to default text direction (for romance languages)
            displayMode = (byte)(LCD_ENTRYLEFT | LCD_ENTRYSHIFTDECREMENT);
            // set the entry mode
            Command((byte)((byte)I2CCommands.LCD_ENTRYMODESET | displayMode));
        }

        // send command
        protected override void Command(byte value)
        {
            var data = new byte[] { 0x80, value };
            i2cPeripheral.Write(data);
        }

        protected override void Send(byte value, byte mode)
        {
            var data = new byte[] { 0x40, value };
            i2cPeripheral.Write(data);
        }
    }
}