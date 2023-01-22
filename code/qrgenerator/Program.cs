using System;
using System.IO;

namespace Net.Codecrete.QrCodeGenerator.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Must provide path to file!");
            }
            const int maxBytes = 2953;
            var bytes = File.ReadAllBytes(args[0]);
            var j = 0;
            for (var i = 0; i < bytes.Length - maxBytes; i += maxBytes)
            {
                var data = (new ReadOnlySpan<byte>(bytes, i, maxBytes)).ToArray();
                var output = QrCode.EncodeBinary(data, QrCode.Ecc.Low);
                output.SaveAsPng(j + ".png", 1, 2);
                j++;
            }
        }
    }
}
