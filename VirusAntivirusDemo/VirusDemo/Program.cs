// ============================================
// 🦠 EĞİTİM AMAÇLI BASİT VİRÜS SİMÜLASYONU
// ============================================
// DİKKAT: Bu tamamen zararsız bir eğitim aracıdır!
// Gerçek bir virüs DEĞİLDİR. Sadece konseptleri gösterir.
// ============================================

using System;
using System.IO;
using System.Threading;

namespace VirusDemo
{
    class SimpleVirus
    {
        // Virüs İmzası - Antivirüs bu imzayı arayacak
        public const string VIRUS_SIGNATURE = "DEMO_VIRUS_SIGNATURE_12345";
        
        private string targetFolder;
        private string logFile;
        private int infectionCount = 0;
        private Random random = new Random();

        public SimpleVirus()
        {
            // Proje klasöründe çalış
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            targetFolder = Path.Combine(baseDir, "infected_files");
            logFile = Path.Combine(baseDir, "virus_activity.log");
        }

        public void CreateTargetFolder()
        {
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"📁 Hedef klasör oluşturuldu: {targetFolder}");
                Console.ResetColor();
            }
        }

        public void LogActivity(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            File.AppendAllText(logFile, $"[{timestamp}] {message}\n");
        }

        public void InfectFile(string filename)
        {
            string filepath = Path.Combine(targetFolder, filename);
            
            string content = $@"
// ============================================
// {VIRUS_SIGNATURE}
// ============================================
// Bu dosya 'enfekte' edildi!
// Enfeksiyon tarihi: {DateTime.Now}
// Dosya adı: {filename}
// ============================================

using System;

class InfectedProgram
{{
    static void Main()
    {{
        Console.WriteLine(""Bu dosya virüs simülasyonu tarafından oluşturuldu!"");
        Console.WriteLine(""Endişelenmeyin, tamamen zararsız bir demo."");
    }}
}}
";
            
            File.WriteAllText(filepath, content);
            
            infectionCount++;
            LogActivity($"Dosya enfekte edildi: {filename}");
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"🦠 Enfekte edildi: {filename}");
            Console.ResetColor();
        }

        public void Replicate()
        {
            string copyName = $"virus_copy_{random.Next(1000, 9999)}.cs";
            string filepath = Path.Combine(targetFolder, copyName);
            
            string replicaContent = $@"
// ============================================
// {VIRUS_SIGNATURE}
// ============================================
// VİRÜS KENDİNİ KOPYALADI!
// Kopya oluşturma tarihi: {DateTime.Now}
// ============================================

using System;

class VirusCopy
{{
    static void Main()
    {{
        Console.WriteLine(""Ben bir virüs kopyasıyım! 🦠"");
        Console.WriteLine(""Ama endişelenme, sadece bir demoyum."");
    }}
}}
";
            
            File.WriteAllText(filepath, replicaContent);
            
            infectionCount++;
            LogActivity($"Virüs kendini kopyaladı: {copyName}");
            
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"🔄 Virüs kendini kopyaladı: {copyName}");
            Console.ResetColor();
        }

        public void Spread(int count = 5)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("🦠 VİRÜS AKTİVİTESİ BAŞLIYOR...");
            Console.WriteLine(new string('=', 50) + "\n");
            Console.ResetColor();

            CreateTargetFolder();

            for (int i = 0; i < count; i++)
            {
                string filename = $"infected_file_{i + 1}.cs";
                InfectFile(filename);
                Thread.Sleep(300); // Görsel efekt için yavaşlat
            }

            // Kendini kopyala
            Replicate();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine($"✅ Virüs aktivitesi tamamlandı!");
            Console.WriteLine($"📊 Toplam enfekte dosya: {infectionCount}");
            Console.WriteLine($"📂 Enfekte klasör: {targetFolder}");
            Console.WriteLine(new string('=', 50));
            Console.ResetColor();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════╗
║       🦠 EĞİTİM AMAÇLI VİRÜS SİMÜLASYONU 🦠                 ║
║                                                              ║
║  DİKKAT: Bu program tamamen zararsızdır!                     ║
║  Sadece virüslerin nasıl çalıştığını gösterir.               ║
║                                                              ║
║  Bu virüs şunları yapacak:                                   ║
║  • 'infected_files' klasörü oluşturacak                      ║
║  • İçine sahte 'enfekte' dosyalar koyacak                    ║
║  • Kendini kopyalayacak                                      ║
║  • Tüm aktiviteleri log dosyasına yazacak                    ║
╚══════════════════════════════════════════════════════════════╝
");
            Console.ResetColor();

            Console.Write("Devam etmek için ENTER'a basın...");
            Console.ReadLine();

            SimpleVirus virus = new SimpleVirus();
            virus.Spread(5);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n💡 Şimdi 'AntivirusDemo' projesini çalıştırarak virüsü temizleyebilirsiniz!");
            Console.ResetColor();
            
            Console.WriteLine("\nÇıkmak için bir tuşa basın...");
            Console.ReadKey();
        }
    }
}
