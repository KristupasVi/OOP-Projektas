namespace ConsoleApp1
{
    using System;
    using System.Diagnostics;
    using System.IO.Pipes;
    using System.Text;
    using System.Threading;

    internal class Master
    {
        static void Main(string[] args)
        {

            string pipe1Pav = "PipeA";
            //string pipe2Pav = "PipeB";
            string folderio1Path = @"C:\Users\krist\OneDrive\Desktop\Uzrasai";
            //string folderio2Path = @"C:\Users\krist\OneDrive\Desktop\Pasakos";

            SiuntimasScaneriui(pipe1Pav, folderio1Path);
          //  SiuntimasScaneriui(pipe2Pav, folderio2Path);
        }
      
        static void SiuntimasScaneriui(string pipePav, string folderioPath)
        {
            using var serveris = new NamedPipeServerStream(pipePav, PipeDirection.Out); //  NamedPipeClientStream tas kuris jungiasi prie serverio
            Console.WriteLine("Master, laukia Scanerio prisijungimo...");
            serveris.WaitForConnection();
            Console.WriteLine("Master: Prisijungė Scaneris");

            using var write = new StreamWriter(serveris) { AutoFlush = true };
            write.WriteLine(folderioPath);

            Console.WriteLine("Master: Išsiuntė folderio path");
        }
    }
}
