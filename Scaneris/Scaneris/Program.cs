using System;
using System.IO.Pipes;
class ScanerisA
{
    static void Main(string[] args)
    {
        string pipePav = "PipeA";
       //string pipeAtgal = "DuomenysA";

        string gautasPath = GautiFolderioPath(pipePav);

        if (!string.IsNullOrEmpty(gautasPath))
        {
            Console.WriteLine($"Gautas folderio path iš master: {gautasPath}");
            NuskaitytiFailus(gautasPath);
        }
        else
        { 
            Console.WriteLine("Nepavyko gauti Folderio path iš master");
            // T
        }
    }

    static string GautiFolderioPath(string pipePav)
    {
        using var Scaneris  = new NamedPipeClientStream(".", pipePav, PipeDirection.In); // NamedPipeServerStream tas kuris laukia kol prisijungs tas iš Mastrer su NamedPipeClientStream
        Console.WriteLine("Scaneris: Jungiasi prie master");
        Scaneris.Connect();
        Console.WriteLine("Scaneris: Prisijungta");

        using var skaitytuvas = new StreamReader(Scaneris);
        return skaitytuvas.ReadLine();
    }

    static void NuskaitytiFailus(string path)
    {
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Katalogas neegzistuoja: {path}");
            return;
        }

        string[] failai = Directory.GetFiles(path, "*.txt");

        Console.WriteLine($"Rasta tiek {failai.Length}, .txt failų:");

        foreach (string failas in failai)
        {
            Console.WriteLine($"- {Path.GetFileName(failas)}");
        }
    }

}