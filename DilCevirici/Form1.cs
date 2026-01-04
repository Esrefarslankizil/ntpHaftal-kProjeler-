using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DilCevirici
{
    public partial class Form1 : Form
    {
        private TextBox txtInput;
        private TextBox txtOutput;
        private ComboBox cmbSourceLang;
        private ComboBox cmbTargetLang;
        private Button btnTranslate;
        private Label lblSource;
        private Label lblTarget;

        public Form1()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Dil Çevirici (Basit)";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            int margin = 20;

            // Source Label
            lblSource = new Label { Text = "Kaynak Metin:", Top = margin, Left = margin, AutoSize = true };
            this.Controls.Add(lblSource);

            // Source Text
            txtInput = new TextBox { 
                Top = lblSource.Bottom + 5, 
                Left = margin, 
                Width = 540, 
                Height = 100, 
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            this.Controls.Add(txtInput);

            // Controls Panel
            int controlTop = txtInput.Bottom + 10;
            
            cmbSourceLang = new ComboBox { Top = controlTop, Left = margin, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSourceLang.Items.AddRange(new object[] { "Türkçe", "English", "Deutsch" });
            cmbSourceLang.SelectedIndex = 0;
            this.Controls.Add(cmbSourceLang);

            Label arrow = new Label { Text = "->", Top = controlTop + 3, Left = cmbSourceLang.Right + 10, AutoSize = true };
            this.Controls.Add(arrow);

            cmbTargetLang = new ComboBox { Top = controlTop, Left = arrow.Right + 10, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTargetLang.Items.AddRange(new object[] { "English", "Türkçe", "Deutsch" });
            cmbTargetLang.SelectedIndex = 0;
            this.Controls.Add(cmbTargetLang);

            btnTranslate = new Button { Text = "Çevir", Top = controlTop, Left = cmbTargetLang.Right + 20, Width = 100, BackColor = Color.LightBlue };
            btnTranslate.Click += BtnTranslate_Click;
            this.Controls.Add(btnTranslate);

            // Target Label
            lblTarget = new Label { Text = "Çeviri Sonucu:", Top = controlTop + 40, Left = margin, AutoSize = true };
            this.Controls.Add(lblTarget);

            // Target Text
            txtOutput = new TextBox { 
                Top = lblTarget.Bottom + 5, 
                Left = margin, 
                Width = 540, 
                Height = 100, 
                Multiline = true, 
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };
            this.Controls.Add(txtOutput);
        }

        private async void BtnTranslate_Click(object sender, EventArgs e)
        {
            string input = txtInput.Text;
            string source = cmbSourceLang.SelectedItem.ToString();
            string target = cmbTargetLang.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Lütfen çevrilecek metni giriniz.", "Uyarı");
                return;
            }

            btnTranslate.Enabled = false;
            btnTranslate.Text = "Çevriliyor...";

            try
            {
                string sourceCode = GetLanguageCode(source);
                string targetCode = GetLanguageCode(target);
                
                string result = await TranslateAsync(input, sourceCode, targetCode);
                txtOutput.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Çeviri hatası: {ex.Message}", "Hata");
            }
            finally
            {
                btnTranslate.Enabled = true;
                btnTranslate.Text = "Çevir";
            }
        }

        private string GetLanguageCode(string langName)
        {
            return langName switch
            {
                "Türkçe" => "tr",
                "English" => "en",
                "Deutsch" => "de",
                _ => "en"
            };
        }

        private async System.Threading.Tasks.Task<string> TranslateAsync(string text, string sourceCode, string targetCode)
        {
            // Google Translate 'gtx' endpoint (unofficial free endpoint)
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sourceCode}&tl={targetCode}&dt=t&q={Uri.EscapeDataString(text)}";

            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                string response = await client.GetStringAsync(url);
                // Response format is messy JSON array: [[["Translated","Original",null,null,1]],...]
                // We'll do a quick parsing hack to avoid complex JSON dependencies for this simple demo
                
                // Find the first string inside the nested arrays
                int firstQuote = response.IndexOf('"');
                if (firstQuote != -1)
                {
                    int secondQuote = response.IndexOf('"', firstQuote + 1);
                     if (secondQuote != -1)
                    {
                        // This usually captures the first sentence. 
                        // For multi-sentence, it might need better parsing, but for a simple demo this often works 
                        // or we can loop through matches.
                        
                        // A slightly better naive parse:
                        // The format is [[["Target","Source",...], ["Target2", "Source2"]]]
                        // We can verify this via simple json parsing if System.Text.Json is available (it is in .NET 8)
                        try
                        {
                            using (System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(response))
                            {
                                var root = doc.RootElement;
                                if (root.ValueKind == System.Text.Json.JsonValueKind.Array)
                                {
                                    var sentences = root[0]; // First element is array of sentences
                                    string fullText = "";
                                    foreach (var sentence in sentences.EnumerateArray())
                                    {
                                        fullText += sentence[0].GetString();
                                    }
                                    return fullText;
                                }
                            }
                        }
                        catch
                        {
                            // Fallback to simple substring if JSON parsing fails
                            return response.Substring(firstQuote + 1, secondQuote - firstQuote - 1);
                        }
                    }
                }
                return "Çeviri yapılamadı (Format Hatası).";
            }
        }

    }
}
