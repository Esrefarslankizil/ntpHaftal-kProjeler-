// ============================================
// 🛡️ EĞİTİM AMAÇLI BASİT ANTİVİRÜS PROGRAMI
// ============================================
// Bu program, virüs simülasyonu tarafından oluşturulan
// dosyaları tespit edip temizler.
// ============================================

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace AntivirusDemo
{
    class SimpleAntivirus
    {
        // Aranan virüs imzası
        private const string VIRUS_SIGNATURE = "DEMO_VIRUS_SIGNATURE_12345";
        
        private string scanDirectory;
        private string quarantineFolder;
        private string logFile;
        private List<string> infectedFiles = new List<string>();
        private int scannedCount = 0;

        public SimpleAntivirus()
        {
            // VirusDemo'nun çalıştığı klasörü tara
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            // VirusDemo klasörüne git
            string virusDemoPath = Path.Combine(Directory.GetParent(baseDir).Parent.Parent.Parent.FullName, "VirusDemo", "bin", "Debug");
            
            // En son oluşturulan klasörü bul (net8.0 gibi)
            if (Directory.Exists(virusDemoPath))
            {
                var dirs = Directory.GetDirectories(virusDemoPath);
                if (dirs.Length > 0)
                {
                    scanDirectory = Path.Combine(dirs[0], "infected_files");
                }
            }
            
            // Eğer bulunamazsa mevcut klasörde ara
            if (string.IsNullOrEmpty(scanDirectory) || !Directory.Exists(scanDirectory))
            {
                scanDirectory = Path.Combine(baseDir, "infected_files");
            }
            
            quarantineFolder = Path.Combine(baseDir, "quarantine");
            logFile = Path.Combine(baseDir, "antivirus_scan.log");
        }

        public void SetScanDirectory(string path)
        {
            scanDirectory = path;
        }

        public void CreateQuarantineFolder()
        {
            if (!Directory.Exists(quarantineFolder))
            {
                Directory.CreateDirectory(quarantineFolder);
            }
        }

        public void LogActivity(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            File.AppendAllText(logFile, $"[{timestamp}] {message}\n");
        }

        public bool ScanFile(string filepath)
        {
            try
            {
                string content = File.ReadAllText(filepath);
                scannedCount++;
                
                if (content.Contains(VIRUS_SIGNATURE))
                {
                    infectedFiles.Add(filepath);
                    LogActivity($"VİRÜS TESPİT EDİLDİ: {filepath}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogActivity($"Dosya okuma hatası: {filepath} - {ex.Message}");
                return false;
            }
        }

        public void QuarantineFile(string filepath)
        {
            try
            {
                CreateQuarantineFolder();
                
                string filename = Path.GetFileName(filepath);
                string quarantinePath = Path.Combine(quarantineFolder, filename + ".quarantined");
                
                File.Move(filepath, quarantinePath);
                LogActivity($"Karantinaya alındı: {filename}");
                
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"   🔒 Karantinaya alındı: {filename}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"   ❌ Karantina hatası: {ex.Message}");
                Console.ResetColor();
            }
        }

        public void DeleteFile(string filepath)
        {
            try
            {
                File.Delete(filepath);
                LogActivity($"Silindi: {filepath}");
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"   🗑️ Silindi: {Path.GetFileName(filepath)}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"   ❌ Silme hatası: {ex.Message}");
                Console.ResetColor();
            }
        }

        public void Scan()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("🔍 TARAMA BAŞLIYOR...");
            Console.WriteLine(new string('=', 50));
            Console.ResetColor();

            Console.WriteLine($"📂 Taranacak klasör: {scanDirectory}\n");

            if (!Directory.Exists(scanDirectory))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️ Hedef klasör bulunamadı!");
                Console.WriteLine("   Önce VirusDemo'yu çalıştırın.");
                Console.ResetColor();
                return;
            }

            string[] files = Directory.GetFiles(scanDirectory, "*.cs");
            
            if (files.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ Klasörde dosya bulunamadı. Sistem temiz!");
                Console.ResetColor();
                return;
            }

            foreach (string file in files)
            {
                Console.Write($"🔍 Taranıyor: {Path.GetFileName(file)}");
                Thread.Sleep(200); // Görsel efekt
                
                if (ScanFile(file))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" ⚠️ VİRÜS TESPİT EDİLDİ!");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" ✓ Temiz");
                    Console.ResetColor();
                }
            }

            // Log dosyasını da tara
            string virusLogPath = Path.Combine(Path.GetDirectoryName(scanDirectory), "virus_activity.log");
            if (File.Exists(virusLogPath))
            {
                infectedFiles.Add(virusLogPath);
            }

            ShowResults();
        }

        public void ShowResults()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("📊 TARAMA SONUÇLARI");
            Console.WriteLine(new string('=', 50));
            Console.ResetColor();

            Console.WriteLine($"📁 Taranan dosya sayısı: {scannedCount}");
            
            if (infectedFiles.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"🦠 Enfekte dosya sayısı: {infectedFiles.Count}");
                Console.ResetColor();
                
                Console.WriteLine("\n🦠 Enfekte dosyalar:");
                foreach (string file in infectedFiles)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"   • {Path.GetFileName(file)}");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ Hiçbir tehdit bulunamadı!");
                Console.ResetColor();
            }
        }

        public void CleanInfections(bool quarantine = false)
        {
            if (infectedFiles.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n✅ Temizlenecek dosya yok!");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("🧹 TEMİZLİK BAŞLIYOR...");
            Console.WriteLine(new string('=', 50));
            Console.ResetColor();

            foreach (string file in infectedFiles)
            {
                Console.WriteLine($"\n📄 İşleniyor: {Path.GetFileName(file)}");
                
                if (quarantine)
                {
                    QuarantineFile(file);
                }
                else
                {
                    DeleteFile(file);
                }
            }

            // infected_files klasörünü de sil
            if (Directory.Exists(scanDirectory))
            {
                try
                {
                    Directory.Delete(scanDirectory, true);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n🗑️ Enfekte klasör silindi: infected_files");
                    Console.ResetColor();
                }
                catch { }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("✅ TEMİZLİK TAMAMLANDI!");
            Console.WriteLine("🛡️ Sisteminiz artık güvende.");
            Console.WriteLine(new string('=', 50));
            Console.ResetColor();

            infectedFiles.Clear();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            SimpleAntivirus antivirus = new SimpleAntivirus();
            
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════╗
║       🛡️ EĞİTİM AMAÇLI ANTİVİRÜS PROGRAMI 🛡️                ║
║                                                              ║
║  Bu program virüs simülasyonunu tespit edip temizler.        ║
║                                                              ║
║  Özellikler:                                                 ║
║  • İmza tabanlı virüs tespiti                               ║
║  • Dosya tarama                                              ║
║  • Karantina veya silme seçenekleri                         ║
║  • Aktivite loglama                                          ║
╚══════════════════════════════════════════════════════════════╝
");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔════════════════════════════════╗");
                Console.WriteLine("║         ANA MENÜ               ║");
                Console.WriteLine("╠════════════════════════════════╣");
                Console.WriteLine("║  1. 🔍 Sistem Tara             ║");
                Console.WriteLine("║  2. 🗑️ Enfekte Dosyaları Sil   ║");
                Console.WriteLine("║  3. 🔒 Karantinaya Al          ║");
                Console.WriteLine("║  4. 📂 Özel Klasör Tara        ║");
                Console.WriteLine("║  5. ❌ Çıkış                   ║");
                Console.WriteLine("╚════════════════════════════════╝");
                Console.ResetColor();

                Console.Write("\nSeçiminiz (1-5): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        antivirus.Scan();
                        break;
                    case "2":
                        antivirus.Scan();
                        antivirus.CleanInfections(quarantine: false);
                        break;
                    case "3":
                        antivirus.Scan();
                        antivirus.CleanInfections(quarantine: true);
                        break;
                    case "4":
                        Console.Write("\nTaranacak klasör yolunu girin: ");
                        string path = Console.ReadLine();
                        if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                        {
                            antivirus.SetScanDirectory(path);
                            antivirus.Scan();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("❌ Geçersiz klasör yolu!");
                            Console.ResetColor();
                        }
                        break;
                    case "5":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\n👋 Güle güle! Sisteminizi güvende tutun.");
                        Console.ResetColor();
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Geçersiz seçim!");
                        Console.ResetColor();
                        break;
                }

                Console.WriteLine("\nDevam etmek için bir tuşa basın...");
                Console.ReadKey();
            }
        }
    }
}
