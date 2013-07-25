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
            using (var br = new BinaryReader(file))
            {
                targetStream.SetLength(0);
                using (var bw = new BinaryWriter(targetStream))
                {
                    var mz = br.ReadBytes(2);
                    if (!mz.SequenceEqual(new byte[] { 0x4d, 0x5a }))
                        throw new FormatException("The file has no MZ header");

                    // skip DOS stub and proceed to PE header
                    br.BaseStream.Seek(0x3c, SeekOrigin.Begin);
                    var baseOffset = br.ReadByte();
                    br.BaseStream.Seek(baseOffset, SeekOrigin.Begin);
                    var peMagic = br.ReadBytes(4);
                    if (!peMagic.SequenceEqual(new byte[] { 0x50, 0x45, 0x0, 0x0 }))
                        throw new FormatException("The file has no PE header");

                    var payloadBytes = Encoding.UTF8.GetBytes(payload);

                    // Skip Coff file header
                    br.ReadBytes(20);
                    var magic = br.ReadUInt16();
                    var isPePlus = magic == 0x20B;

                    // Skip optional header
                    br.ReadBytes(isPePlus ? 142 : 126);

                    // Read address and size of the certificate table
                    var address = br.ReadUInt32();
                    var sizePosition = br.BaseStream.Position;
                    var size = br.ReadUInt32();
                    if(address == 0)
                        throw new InvalidOperationException("The file contains no certificate");

                    var newSize = (uint)(size + payloadBytes.Length);
                    var padding =  8 - (newSize % 8);
                    newSize += padding;

                    // Copy the first part to the output stream
                    br.BaseStream.Seek(0, SeekOrigin.Begin);
                    
                    var firstChunkbytes = new byte[sizePosition];
                    br.Read(firstChunkbytes, 0, firstChunkbytes.Length);
                    bw.Write(firstChunkbytes, 0, firstChunkbytes.Length);
                    bw.Write(newSize);

                    // reread the size
                    br.ReadUInt32();
                    var secondChunkposition = br.BaseStream.Position;

                    // Copy the second part to the output stream
                    var secondChunkbytes = new byte[address - secondChunkposition];
                    br.Read(secondChunkbytes, 0, secondChunkbytes.Length);
                    bw.Write(secondChunkbytes, 0, secondChunkbytes.Length);
                    
                    
                    // Skip the length field in the certificate table header
                    br.ReadUInt32();
                    var certTablebytes = new byte[size - 4];
                    br.Read(certTablebytes, 0, certTablebytes.Length);
                    // Write the new length to the certificate table header
                    bw.Write(newSize);
                    bw.Write(certTablebytes, 0, certTablebytes.Length);

                    // Copy the payload to output stream
                    bw.Write(payloadBytes, 0, payloadBytes.Length);

                    // Padd with zeros
                    bw.Write(new byte[(int)padding], 0, (int)padding);
                }
            }
        }
    }
}
