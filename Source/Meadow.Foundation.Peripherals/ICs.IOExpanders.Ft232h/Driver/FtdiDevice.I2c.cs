﻿using System;
using static Meadow.Foundation.ICs.IOExpanders.Native;
using static Meadow.Foundation.ICs.IOExpanders.Native.Ftd2xx;

namespace Meadow.Foundation.ICs.IOExpanders;

internal partial class FtdiDevice
{
    internal static class PinDirection
    {
        public const byte SDAinSCLin = 0x00;
        public const byte SDAinSCLout = 0x01;
        public const byte SDAoutSCLin = 0x02;
        public const byte SDAoutSCLout = 0x03;
    }

    internal static class PinData
    {
        public const byte SDAloSCLhi = 0x01;
        public const byte SDAhiSCLhi = 0x03;
        public const byte SDAloSCLlo = 0x00;
        public const byte SDAhiSCLlo = 0x02;
    }

    public void InitializeI2C()
    {
        // TODO: make sure we're not already set up for SPI

        CheckStatus(
            FT_SetTimeouts(Handle, DefaultTimeoutMs, DefaultTimeoutMs));
        CheckStatus(
            FT_SetLatencyTimer(Handle, DefaultLatencyTimer));
        CheckStatus(
            FT_SetFlowControl(Handle, FT_FLOWCONTROL.FT_FLOW_RTS_CTS, 0x00, 0x00));
        CheckStatus(
            FT_SetBitMode(Handle, 0x00, FT_BITMODE.FT_BITMODE_RESET));
        CheckStatus(
            FT_SetBitMode(Handle, 0x00, FT_BITMODE.FT_BITMODE_MPSSE));

        ClearInputBuffer();
        InitializeMpsse();
    }

    private byte GpioLowData = 0;
    private byte GpioLowDir = 0;
    private byte GpioHighData = 0;
    private byte GpioHighDir = 0;
    private const byte NumberCycles = 6;
    private const byte MaskGpio = 0xF8;

    public uint I2cBusFrequencyKbps { get; set; } = 400;

    internal void I2cStart()
    {
        int count;
        int idx = 0;
        // SDA high, SCL high
        // The behavior is a bit different for FT232H and FT2232H/FT4232H
        if (DeviceType == FtDeviceType.Ft232H)
        {
            GpioLowData = (byte)(PinData.SDAhiSCLhi | (GpioLowData & MaskGpio));
            GpioLowDir = (byte)(PinDirection.SDAoutSCLout | (GpioLowDir & MaskGpio));
        }
        else
        {
            GpioLowData = (byte)(PinData.SDAloSCLlo | (GpioLowData & MaskGpio));
            GpioLowDir = (byte)(PinDirection.SDAinSCLin | (GpioLowDir & MaskGpio));
        }

        Span<byte> toSend = stackalloc byte[(NumberCycles * 3 * 3) + 3];
        for (count = 0; count < NumberCycles; count++)
        {
            toSend[idx++] = (byte)FT_OPCODE.SetDataBitsLowByte;
            toSend[idx++] = GpioLowData;
            toSend[idx++] = GpioLowDir;
        }

        // SDA lo, SCL high
        // The behavior is a bit different for FT232H and FT2232H/FT4232H
        if (DeviceType == FtDeviceType.Ft232H)
        {
            GpioLowData = (byte)(PinData.SDAloSCLhi | (GpioLowData & MaskGpio));
        }
        else
        {
            GpioLowDir = (byte)(PinDirection.SDAoutSCLin | (GpioLowDir & MaskGpio));
        }

        for (count = 0; count < NumberCycles; count++)
        {
            toSend[idx++] = (byte)FT_OPCODE.SetDataBitsLowByte;
            toSend[idx++] = GpioLowData;
            toSend[idx++] = GpioLowDir;
        }

        // SDA lo, SCL lo
        // The behavior is a bit different for FT232H and FT2232H/FT4232H
        if (DeviceType == FtDeviceType.Ft232H)
        {
            GpioLowData = (byte)(PinData.SDAloSCLlo | (GpioLowData & MaskGpio));
        }
        else
        {
            GpioLowDir = (byte)(PinDirection.SDAoutSCLout | (GpioLowDir & MaskGpio));
        }

        for (count = 0; count < NumberCycles; count++)
        {
            toSend[idx++] = (byte)FT_OPCODE.SetDataBitsLowByte;
            toSend[idx++] = GpioLowData;
            toSend[idx++] = GpioLowDir;
        }

        // Release SDA
        // The behavior is a bit different for FT232H and FT2232H/FT4232H
        if (DeviceType == FtDeviceType.Ft232H)
        {
            GpioLowData = (byte)(PinData.SDAhiSCLlo | (GpioLowData & MaskGpio));
        }
        else
        {
            GpioLowDir = (byte)(PinDirection.SDAinSCLout | (GpioLowDir & MaskGpio));
        }

        toSend[idx++] = (byte)FT_OPCODE.SetDataBitsLowByte;
        toSend[idx++] = GpioLowData;
        toSend[idx++] = GpioLowDir;

        Write(toSend);
    }

    internal void I2cStop()
    {
        int count;
        int idx = 0;
        // SDA low, SCL low
        GpioLowData = (byte)(PinData.SDAloSCLlo | (GpioLowData & MaskGpio));
        GpioLowDir = (byte)(PinDirection.SDAoutSCLout | (GpioLowDir & MaskGpio));

        Span<byte> toSend = stackalloc byte[NumberCycles * 3 * 3];
        for (count = 0; count < NumberCycles; count++)
        {
            toSend[idx++] = (byte)FT_OPCODE.SetDataBitsLowByte;
            toSend[idx++] = GpioLowData;
            toSend[idx++] = GpioLowDir;
        }

        // SDA low, SCL high
        // The behavior is a bit different for FT232H and FT2232H/FT4232H
        if (DeviceType == FtDeviceType.Ft232H)
        {
            GpioLowData = (byte)(PinData.SDAloSCLhi | (GpioLowData & MaskGpio));
            GpioLowDir = (byte)(PinDirection.SDAoutSCLout | (GpioLowDir & MaskGpio));
        }
        else
        {
            GpioLowData = (byte)(PinData.SDAloSCLlo | (GpioLowData & MaskGpio));
            GpioLowDir = (byte)(PinDirection.SDAoutSCLin | (GpioLowDir & MaskGpio));
        }

        for (count = 0; count < NumberCycles; count++)
        {
            toSend[idx++] = (byte)FT_OPCODE.SetDataBitsLowByte;
            toSend[idx++] = GpioLowData;
            toSend[idx++] = GpioLowDir;
        }

        // SDA high, SCL high
        // The behavior is a bit different for FT232H and FT2232H/FT4232H
        if (DeviceType == FtDeviceType.Ft232H)
        {
            GpioLowData = (byte)(PinData.SDAhiSCLhi | (GpioLowData & MaskGpio));
            GpioLowDir = (byte)(PinDirection.SDAoutSCLout | (GpioLowDir & MaskGpio));
        }
        else
        {
            GpioLowData = (byte)(PinData.SDAloSCLlo | (GpioLowData & MaskGpio));
            GpioLowDir = (byte)(PinDirection.SDAinSCLin | (GpioLowDir & MaskGpio));
        }

        for (count = 0; count < NumberCycles; count++)
        {
            toSend[idx++] = (byte)FT_OPCODE.SetDataBitsLowByte;
            toSend[idx++] = GpioLowData;
            toSend[idx++] = GpioLowDir;
        }

        Write(toSend);
    }

    internal void I2cLineIdle()
    {
        int idx = 0;
        // SDA low, SCL low
        // The behavior is a bit different for FT232H and FT2232H/FT4232H
        if (DeviceType == FtDeviceType.Ft232H)
        {
            GpioLowData = (byte)(PinData.SDAhiSCLhi | (GpioLowData & MaskGpio));
            GpioLowDir = (byte)(PinDirection.SDAoutSCLout | (GpioLowDir & MaskGpio));
        }
        else
        {
            GpioLowData = (byte)(PinData.SDAloSCLlo | (GpioLowData & MaskGpio));
            GpioLowDir = (byte)(PinDirection.SDAinSCLin | (GpioLowDir & MaskGpio));
        }

        Span<byte> toSend = stackalloc byte[3];
        toSend[idx++] = (byte)FT_OPCODE.SetDataBitsLowByte;
        toSend[idx++] = GpioLowData;
        toSend[idx++] = GpioLowDir;
        Write(toSend);
    }

    internal bool I2cSendByteAndCheckACK(byte data)
    {
        int idx = 0;
        Span<byte> toSend = stackalloc byte[DeviceType == FtDeviceType.Ft232H ? 10 : 13];
        Span<byte> toRead = stackalloc byte[1];
        // The behavior is a bit different for FT232H and FT2232H/FT4232H
        if (DeviceType == FtDeviceType.Ft232H)
        {
            // Just clock with one byte (0 = 1 byte)
            toSend[idx++] = (byte)FT_OPCODE.ClockDataBytesOutOnMinusVeClockMSBFirst;
            toSend[idx++] = 0;
            toSend[idx++] = 0;
            toSend[idx++] = data;
            // Put line back to idle (data released, clock pulled low)
            GpioLowData = (byte)(PinData.SDAhiSCLlo | (GpioLowData & MaskGpio));
            GpioLowDir = (byte)(PinDirection.SDAoutSCLout | (GpioLowDir & MaskGpio));
            toSend[idx++] = (byte)FT_OPCODE.SetDataBitsLowByte;
            toSend[idx++] = GpioLowData;
            toSend[idx++] = GpioLowDir;
            // Clock in (0 = 1 byte)
            toSend[idx++] = (byte)FT_OPCODE.ClockDataBitsInOnPlusVeClockMSBFirst;
            toSend[idx++] = 0;
        }
        else
        {
            // Set directions and clock data
            GpioLowData = (byte)(PinData.SDAloSCLlo | (GpioLowData & MaskGpio));
            GpioLowDir = (byte)(PinDirection.SDAoutSCLout | (GpioLowDir & MaskGpio));
            toSend[idx++] = (byte)FT_OPCODE.SetDataBitsLowByte;
            toSend[idx++] = GpioLowData;
            toSend[idx++] = GpioLowDir;
            // Just clock with one byte (0 = 1 byte)
            toSend[idx++] = (byte)FT_OPCODE.ClockDataBytesOutOnMinusVeClockMSBFirst;
            toSend[idx++] = 0;
            toSend[idx++] = 0;
            toSend[idx++] = data;
            // Put line back to idle (data released, clock pulled low)
            // Set directions and clock data
            GpioLowData = (byte)(PinData.SDAloSCLlo | (GpioLowData & MaskGpio));
            GpioLowDir = (byte)(PinDirection.SDAinSCLout | (GpioLowDir & MaskGpio));
            toSend[idx++] = (byte)FT_OPCODE.SetDataBitsLowByte;
            toSend[idx++] = GpioLowData;
            toSend[idx++] = GpioLowDir;
            // Clock in (0 = 1 byte)
            toSend[idx++] = (byte)FT_OPCODE.ClockDataBitsInOnPlusVeClockMSBFirst;
            toSend[idx++] = 0;
        }

        // And ask it right away
        toSend[idx++] = (byte)FT_OPCODE.SendImmediate;
        Write(toSend);
        ReadInto(toRead);
        // Bit 0 equivalent to acknoledge, otherwise nack
        return (toRead[0] & 0x01) == 0;
    }

    internal bool I2cSendDeviceAddrAndCheckACK(byte Address, bool Read)
    {
        // Set address for read or write
        Address <<= 1;
        if (Read == true)
        {
            Address |= 0x01;
        }

        return I2cSendByteAndCheckACK(Address);
    }

    internal byte I2CReadByte(bool ack)
    {
        int idx = 0;
        Span<byte> toSend = stackalloc byte[DeviceType == FtDeviceType.Ft232H ? 10 : 16];
        Span<byte> toRead = stackalloc byte[1];
        // The behavior is a bit different for FT232H and FT2232H/FT4232H
        if (DeviceType == FtDeviceType.Ft232H)
        {
            // Read one byte
            toSend[idx++] = (byte)FT_OPCODE.ClockDataBytesInOnPlusVeClockMSBFirst;
            toSend[idx++] = 0;
            toSend[idx++] = 0;
            // Send out either ack either nak
            toSend[idx++] = (byte)FT_OPCODE.ClockDataBitsOutOnMinusVeClockMSBFirst;
            toSend[idx++] = 0;
            toSend[idx++] = (byte)(ack ? 0x00 : 0xFF);
            // I2C lines back to idle state
            toSend[idx++] = (byte)FT_OPCODE.SetDataBitsLowByte;
            GpioLowData = (byte)(PinData.SDAhiSCLlo | (GpioLowData & MaskGpio));
            GpioLowDir = (byte)(PinDirection.SDAoutSCLout | (GpioLowDir & MaskGpio));
            toSend[idx++] = GpioLowData;
            toSend[idx++] = GpioLowDir;
        }
        else
        {
            // Make sure no open gain
            GpioLowData = (byte)(PinData.SDAloSCLlo | (GpioLowData & MaskGpio));
            GpioLowDir = (byte)(PinDirection.SDAinSCLout | (GpioLowDir & MaskGpio));
            toSend[idx++] = (byte)FT_OPCODE.SetDataBitsLowByte;
            toSend[idx++] = GpioLowData;
            toSend[idx++] = GpioLowDir;
            // Read one byte
            toSend[idx++] = (byte)FT_OPCODE.ClockDataBytesInOnPlusVeClockMSBFirst;
            toSend[idx++] = 0;
            toSend[idx++] = 0;
            // Change direction
            GpioLowData = (byte)(PinData.SDAloSCLlo | (GpioLowData & MaskGpio));
            GpioLowDir = (byte)(PinDirection.SDAoutSCLout | (GpioLowDir & MaskGpio));
            toSend[idx++] = (byte)FT_OPCODE.SetDataBitsLowByte;
            toSend[idx++] = GpioLowData;
            toSend[idx++] = GpioLowDir;
            // Send out either ack either nak
            toSend[idx++] = (byte)FT_OPCODE.ClockDataBitsOutOnMinusVeClockMSBFirst;
            toSend[idx++] = 0;
            toSend[idx++] = (byte)(ack ? 0x00 : 0xFF);
            // I2C lines back to idle state
            toSend[idx++] = (byte)FT_OPCODE.SetDataBitsLowByte;
            GpioLowData = (byte)(PinData.SDAhiSCLlo | (GpioLowData & MaskGpio));
            GpioLowDir = (byte)(PinDirection.SDAinSCLout | (GpioLowDir & MaskGpio));
            toSend[idx++] = GpioLowData;
            toSend[idx++] = GpioLowDir;
        }

        // And ask it right away
        toSend[idx++] = (byte)FT_OPCODE.SendImmediate;
        Write(toSend);
        ReadInto(toRead);
        return toRead[0];
    }
}
