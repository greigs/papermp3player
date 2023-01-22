using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace mp3
{
    public class Program
    {
        public static void Main()
        {
            const int numParts = 291;
            // ffmpeg -rtbufsize 1k -i pipe: -f wav pipe:1 | ffplay -nodisp -
            const string bashScript = "start-pipe.sh";
            const string path = "/home/greig/filewatcherplayer/data/tmp/";

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