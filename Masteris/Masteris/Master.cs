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
            string folderio1Path = @"C:\Users\krist\OneDrive\Desktop\Uzrasai";
            string rezultatas1 = "DuomenysA";
            string pipe2Pav = "PipeB";
            string folderio2Path = @"C:\Users\krist\OneDrive\Desktop\Pasakos";
            string rezultatas2 = "DuomenysB";

            SiuntimasScaneriui(pipe1Pav, folderio1Path);
            GautiRezultatai(rezultatas1);
            SiuntimasScaneriui(pipe2Pav, folderio2Path);
            GautiRezultatai(rezultatas2);

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
        static void GautiRezultatai(string pipePav)
        {
            using var infoGavimas = new NamedPipeServerStream(pipePav, PipeDirection.In);
            Console.WriteLine("Master: Laukia rezultatų iš scanerio...");
            infoGavimas.WaitForConnection();
            Console.WriteLine("Master: Prisijunge Scanris");

            using var skaitytuvas = new StreamReader(infoGavimas);
            string? line;
            while ((line = skaitytuvas.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine($"Rezultatai gauti iš {pipePav}");
        }
    }


}
