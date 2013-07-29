using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Pixills.System.PePayload
{
    public class Payload
    {
        public static void Append(Stream file, Stream targetStream, string payload)
        {
            targetStream.SetLength(0);
            using (var reader = new BinaryReader(file))
            using (var writer = new BinaryWriter(targetStream))
            {
                var mz = reader.ReadBytes(2);
                if (!mz.SequenceEqual(new byte[] { 0x4d, 0x5a }))
                    throw new FormatException("The file has no MZ header");

                // skip DOS stub and proceed to PE header
                reader.BaseStream.Seek(0x3c, SeekOrigin.Begin);
                var baseOffset = reader.ReadByte();
                reader.BaseStream.Seek(baseOffset, SeekOrigin.Begin);
                var peMagic = reader.ReadBytes(4);
                if (!peMagic.SequenceEqual(new byte[] { 0x50, 0x45, 0x0, 0x0 }))
                    throw new FormatException("The file has no PE header");

                var payloadBytes = Encoding.UTF8.GetBytes(payload);

                // Skip Coff file header
                reader.ReadBytes(20);
                var magic = reader.ReadUInt16();
                var isPePlus = magic == 0x20B;

                // Skip optional header
                reader.ReadBytes(isPePlus ? 142 : 126);

                // Read address and size of the certificate table
                var address = reader.ReadUInt32();
                var sizePosition = reader.BaseStream.Position;
                var size = reader.ReadUInt32();
                if (address == 0)
                    throw new InvalidOperationException("The file does not contain any certificate");

                var newSize = (uint)(size + payloadBytes.Length);
                var padding = 8 - (newSize % 8);
                newSize += padding;

                // Copy the first part to the output stream
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                var firstChunkbytes = new byte[sizePosition];
                reader.Read(firstChunkbytes, 0, firstChunkbytes.Length);
                writer.Write(firstChunkbytes, 0, firstChunkbytes.Length);
                writer.Write(newSize);

                // reread the size
                reader.ReadUInt32();
                var secondChunkposition = reader.BaseStream.Position;

                // Copy the second part to the output stream
                var secondChunkbytes = new byte[address - secondChunkposition];
                reader.Read(secondChunkbytes, 0, secondChunkbytes.Length);
                writer.Write(secondChunkbytes, 0, secondChunkbytes.Length);


                // Skip the length field in the certificate table header
                reader.ReadUInt32();
                var certTablebytes = new byte[size - 4];
                reader.Read(certTablebytes, 0, certTablebytes.Length);
                // Write the new length to the certificate table header
                writer.Write(newSize);
                writer.Write(certTablebytes, 0, certTablebytes.Length);

                // Copy the payload to output stream
                writer.Write(payloadBytes, 0, payloadBytes.Length);

                // Padd with zeros
                writer.Write(new byte[(int)padding], 0, (int)padding);
            }
        }
    }
}
