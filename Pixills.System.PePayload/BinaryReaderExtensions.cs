using System;
using System.IO;
using System.Threading;

namespace AppenPayload
{
    public static class BinaryReaderExtensions
    {
        public static void WriteAsBigEndian(this BinaryWriter writer, uint value)
        {
            var bigEndianArray = new[]
            {
                (byte)(value >> 24 & 0xff),
                (byte)((value >> 16) & 0xff), 
                (byte)((value >> 8) & 0xff), 
                (byte)((value) & 0xff)
            };
            writer.Write(bigEndianArray);
        }

        public static uint ReadAsBigEndian(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}
