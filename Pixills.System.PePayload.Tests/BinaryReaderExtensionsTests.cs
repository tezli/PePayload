using NUnit.Framework;
using System.IO;

namespace AppenPayload.Tests
{
    [TestFixture]
    public class BinaryReaderExtensionsTests
    {
        [Test]
        public void ReadAsBigEndianTest()
        {
            uint actual;
            const uint input = 0xd0c0b0a0;
            const uint expected = 0xa0b0c0d0;
            using (var bw = new BinaryWriter(new MemoryStream()))
            {
                bw.Write(input);
                using (var br = new BinaryReader(bw.BaseStream))
                {
                    bw.BaseStream.Seek(0, SeekOrigin.Begin);
                    actual = br.ReadAsBigEndian();
                }
            }
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WriteAsBigEndianTest()
        {
            uint actual;
            const uint input = 0xd0c0b0a0;
            const uint expected = 0xa0b0c0d0;
            using (var bw = new BinaryWriter(new MemoryStream()))
            {
                bw.WriteAsBigEndian(input);
                using (var br = new BinaryReader(bw.BaseStream))
                {
                    bw.BaseStream.Seek(0, SeekOrigin.Begin);
                    actual = br.ReadUInt32();
                }
            }
            Assert.AreEqual(expected, actual);
        }
    }
}
