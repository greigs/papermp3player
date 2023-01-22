using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace mp3
{
    public class Program
    {

        // https://github.com/manuelbl/QrCodeGenerator
        // Net.Codecrete.QrCodeGenerator
        // Demo-Winforms 
        // QrCodeControl.cs
        // private QrCode CreateQrCode()
        // {
        //
        //     const int maxBytes = 2953;
        //     var bytes = File.ReadAllBytes("input.m4a");
        //     var j = 0;
        //     for (var i = 0; i < bytes.Length - maxBytes; i += maxBytes)
        //     {
        //         var data = (new ReadOnlySpan<byte>(bytes, i, maxBytes)).ToArray();
        //         var output = QrCode.EncodeBinary(data, QrCode.Ecc.Low);
        //         output.SaveAsPng(j + ".png", 1, 2);
        //         j++;
        //     }
        //
        //     return QrCode.EncodeSegments(new List<QrSegment>(){ QrSegment.MakeBytes(Encoding.UTF8.GetBytes(_textData.ToCharArray()))}, QrCode.Ecc.Low);
        //
        // }

        public static void Main()
        {
            const int numParts = 291;
            // ffmpeg -rtbufsize 1k -i pipe: -f wav pipe:1 | ffplay -nodisp -
            const string bashScript = "start-pipe.sh";
            const string path = "/home/greig/papermp3/input_spt/";

            using (Process myProcess = new Process())
            {
                myProcess.StartInfo.FileName = "bash";
                myProcess.StartInfo.Arguments = bashScript;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.RedirectStandardInput = true;
                myProcess.Start();

                Stream outputStream = myProcess.StandardInput.BaseStream;

                for (int i = 0; i <= numParts - 1; i++)
                {
                    FileSystemWatcher fsw = new FileSystemWatcher(path);

                    WaitForChangedResult fileSystemChange;
                    do
                    {
                        fileSystemChange = fsw.WaitForChanged(WatcherChangeTypes.Created, 60000);
                    } while (fileSystemChange.Name != $"input_{i}.spt");
                    Console.WriteLine(i);
                    var inputFromQrCode = File.OpenRead($"{path}input_{i}.spt");
                    var bufferSize = (int)inputFromQrCode.Length;
                    var newBuffer = new byte[bufferSize];
                    inputFromQrCode.Read(newBuffer, 0, bufferSize);
                    outputStream.Write(newBuffer, 0, newBuffer.Length);
                    inputFromQrCode.Close();
                }
            }
        }
    }
}