using System;
using System.IO.Pipes;
class ScanerisA
{
    static void Main(string[] args)
    {
        string pipePav = "PipeA";
        string pipeAtgal = "DuomenysA";

        string gautasPath = GautiFolderioPath(pipePav);

        if (!string.IsNullOrEmpty(gautasPath))
        {
            Console.WriteLine($"Gautas folderio path iš master: {gautasPath}");
            NuskaitytiFailus(gautasPath, pipeAtgal);
        }
        else
        { 
            Console.WriteLine("Nepavyko gauti Folderio path iš master");
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

    static void NuskaitytiFailus(string path, string pipeAtgal)
    {
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Katalogas neegzistuoja: {path}");
            return;
        }

        string[] failai = Directory.GetFiles(path, "*.txt");

        Console.WriteLine($"Rasta tiek {failai.Length}, .txt failų:");

       
        using var siuntimasAtgal = IssiuntimasImaster(pipeAtgal);
        using var scaneris = new StreamWriter(siuntimasAtgal) { AutoFlush = true };

        foreach (string failas in failai)
        {
            string tekstas = File.ReadAllText(failas);
            var zodziuStatistika = SkaiciuotiZodzius(tekstas);

            scaneris.WriteLine($"\nKnygos {Path.GetFileName(failas)} statistika: ");
            foreach (var pora  in zodziuStatistika)
            {
                scaneris.WriteLine($"{pora.Key}:{pora.Value}");
            }
        }

        scaneris.WriteLine("\nVISOS KNYGOS BUVO NUSKENUOTOS");
    }

    static NamedPipeClientStream IssiuntimasImaster(string pipePav)
    {
        var siuntimasMasteriui = new NamedPipeClientStream(".", pipePav, PipeDirection.Out);
        Console.WriteLine("Scaneris: Jungiamasi prie Master rezultato pipe:");
        siuntimasMasteriui.Connect();
        Console.WriteLine($"Prisijungta prie {pipePav}");
        return siuntimasMasteriui;
    }
    static Dictionary<string, int> SkaiciuotiZodzius(string tekstas)
    {
        var rezultatas = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var zodziai = tekstas.Split(new[] { ' ', '\r', '\n', '\t', '.', ',', ';', ':', '!', '?', '(', ')', '[', ']', '{', '}', '"', '\'' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var zodis in zodziai)
        {
            if(rezultatas.ContainsKey(zodis))
                rezultatas[zodis]++;
            else
                rezultatas[zodis] = 1;
        }
        return rezultatas;
    }
}