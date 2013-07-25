using NUnit.Framework;
using System;
using System.IO;

namespace Pixills.System.PePayload.Tests
{
    [TestFixture]
    public class PayloadTests
    {
        [Test]
        public void AppendTest()
        {
            /*
             * TODO : Add proper values for 
             * infile  : signed file without payload
             * outFile : file to be written
             * outFile : reference file. A working file wher payload already appended
             */
            var inFile = "";
            var outFile = "";
            var reference = "";
            var payload = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?></xml>";
            if (File.Exists(inFile))
            {
                using (var inputFile = File.OpenRead(inFile))
                {
                    using (var outputFile = new FileStream(outFile, FileMode.OpenOrCreate))
                    {
                        Payload.Append(inputFile, outputFile, payload);
                    }
                }
                var actual = File.ReadAllBytes(outFile);
                var expected = File.ReadAllBytes(reference);
                Assert.AreEqual(expected, actual);
            }

        }

        [Test]
        public void AppenTestOnUnsigened()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                /*
                 * TODO : Add proper values for 
                 * infile  : signed file without payload
                 * outFile : file to be written to
                 * outFile : reference file. A working file wher payload already appended
                 */
                var outFile = "";
                var unsignedFile = "";
                var payload = string.Empty;
                using (var inputFile = File.OpenRead(unsignedFile))
                {
                    using (var outputFile = new FileStream(outFile, FileMode.OpenOrCreate))
                    {
                        Payload.Append(inputFile, outputFile, payload);
                    }
                }
            });
        }
    }
}
