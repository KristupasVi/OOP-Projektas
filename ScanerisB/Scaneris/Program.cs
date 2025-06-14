using System;
using System.Diagnostics;
using System.IO.Pipes;
class ScanerisB
{
    static List<string> duomenysAtgal = new List<string>();
    static bool baigeSkaityma = false;
    static void Main(string[] args)
    {
        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)2;

        string pipePav = "PipeB";
        string pipeAtgal = "DuomenysB";

        string gautasPath = GautiFolderioPath(pipePav);

        if (!string.IsNullOrEmpty(gautasPath))
        {
            Console.WriteLine($"Gautas folderio path iš master: {gautasPath}");
            Thread skaitymoT = new Thread(() => NuskaitytiFailus(gautasPath));
            Thread siuntimoT = new Thread(() => IssiuntimasImaster(pipeAtgal));

            skaitymoT.Start();
            siuntimoT.Start();

            skaitymoT.Join();
            siuntimoT.Join();
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

    static void NuskaitytiFailus(string path)
    {
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Katalogas neegzistuoja: {path}");
            baigeSkaityma = true;
            return;
        }

        string[] failai = Directory.GetFiles(path, "*.txt");

        Console.WriteLine($"Rasta tiek {failai.Length}, .txt failų:");

        foreach (string failas in failai)
        {
            string tekstas = File.ReadAllText(failas);
            var zodziuStatistika = SkaiciuotiZodzius(tekstas);

            duomenysAtgal.Add($"\nKnygos {Path.GetFileName(failas)} statistika: ");
            foreach (var pora  in zodziuStatistika)
            {
                duomenysAtgal.Add($"{pora.Key}:{pora.Value}");
            }
        }

        duomenysAtgal.Add("\nVISOS KNYGOS BUVO NUSKENUOTOS");
        baigeSkaityma = true;
    }

    static void IssiuntimasImaster (String pipePav)
    {
        var siuntimasMasteriui = new NamedPipeClientStream(".", pipePav, PipeDirection.Out);
        Console.WriteLine("Scaneris: Jungiamasi prie Master rezultato pipe:");
        siuntimasMasteriui.Connect();
        Console.WriteLine($"Prisijungta prie {pipePav}");

        using var scaneris = new StreamWriter(siuntimasMasteriui) { AutoFlush = true };

        while (!baigeSkaityma || duomenysAtgal.Count > 0)
        {
            if (duomenysAtgal.Count > 0)
            {
                string eilute = duomenysAtgal[0];
                duomenysAtgal.RemoveAt(0);
                scaneris.WriteLine(eilute);
            }
        }
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