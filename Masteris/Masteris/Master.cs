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

            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)4;
            string pipe1Pav = "PipeA";
            string folderio1Path = @"C:\Users\krist\OneDrive\Desktop\Uzrasai";
            string rezultatas1 = "DuomenysA";
            string pipe2Pav = "PipeB";
            string folderio2Path = @"C:\Users\krist\OneDrive\Desktop\Pasakos";
            string rezultatas2 = "DuomenysB";

            Thread siuntimas1 = new(() => SiuntimasScaneriui(pipe1Pav, folderio1Path));
            Thread gavimasAtgal1 = new(() => GautiRezultatai(rezultatas1));

            Thread siuntimas2 = new(() => SiuntimasScaneriui(pipe2Pav, folderio2Path));
            Thread gavimasAtgal2 = new(() => GautiRezultatai(rezultatas2));


            siuntimas1.Start();
            gavimasAtgal1.Start();
            siuntimas2.Start();
            gavimasAtgal2.Start();

            siuntimas1.Join();
            gavimasAtgal1.Join();
            siuntimas2.Join();
            gavimasAtgal2.Join();
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
