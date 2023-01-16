using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace SharpEditor
{
    public partial class Form1 : Form
    {
        public List<string> links = new List<string>();
        public string openFile = String.Empty;
        public bool fileIsChanged = false;
        public bool fileIsOpen = false;
        public Form1()
        {
            InitializeComponent();
            links.Add("System.Core.dll");
            richTextBox1.Text = "System.Core.dll";
            autocompleteMenu1.Items = File.ReadAllLines("cs-reserv-list.dicr");
            сохранитьToolStripMenuItem.Enabled = false;

            fastColoredTextBox1.Language = FastColoredTextBoxNS.Language.Custom;
            autocompleteMenu1.Enabled = false;
            режимToolStripMenuItem.Image = обычныйТекстToolStripMenuItem.Image;
            компиляцияToolStripMenuItem.Enabled = false;
        }

        private void fastColoredTextBox1_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            string text = fastColoredTextBox1.Text;
            string[] lines = text.Split('\n');
            
            label1.Text = "Строк: " + lines.Length.ToString();
            label2.Text = "Символов: " + text.Length.ToString();
        }

        private void открытьМенеджерСсылокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
        }

        // закрыть менеджер ссылок
        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filter = null;

            switch (fastColoredTextBox1.Language)
            {
                case FastColoredTextBoxNS.Language.Custom:
                    filter = "Text File (*.txt)|*.txt|" +
                        "Sharp Editor File(*.sef)| *.sef"; break;
                case FastColoredTextBoxNS.Language.CSharp:
                    filter = "CSharp source code (*.cs)|*.cs"; break;
                case FastColoredTextBoxNS.Language.HTML:
                    filter = "HTML file (*.html)|*.thml"; break;
                case FastColoredTextBoxNS.Language.JSON:
                    filter = "JSON file (*.json)|*.json"; break;
                case FastColoredTextBoxNS.Language.XML:
                    filter = "XML File (*.xml)|*.xml"; break;
            }

            openFileDialog1.Filter = filter;


            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            fastColoredTextBox1.Text = File.ReadAllText(openFileDialog1.FileName);
            openFile = openFileDialog1.FileName;

            сохранитьToolStripMenuItem.Enabled = true;
            fileIsOpen = true;
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
               File.WriteAllText(openFile, fastColoredTextBox1.Text);
                fileIsChanged = false;
            }
            catch(Exception)
            {
                MessageBox.Show("Ошибка", "Не удалось сохранить файл", MessageBoxButtons.OK);
            }          
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filter = null;

            switch (fastColoredTextBox1.Language)
            {
                case FastColoredTextBoxNS.Language.Custom:
                    filter = "Text File (*.txt)|*.txt | " +
                        "Sharp Editor File(*.sef)| *.sef"; break;
                case FastColoredTextBoxNS.Language.CSharp: 
                    filter = "CSharp source code (*.cs)|*.cs"; break;
                case FastColoredTextBoxNS.Language.HTML:
                    filter = "HTML file (*.html)|*.thml"; break;
                case FastColoredTextBoxNS.Language.JSON:
                    filter = "JSON file (*.json)|*.json"; break;
                case FastColoredTextBoxNS.Language.XML:
                    filter = "XML File (*.xml)|*.xml"; break;
            }

            saveFileDialog1.Filter = filter;

            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            File.WriteAllText(saveFileDialog1.FileName, fastColoredTextBox1.Text);
            fileIsChanged = false;
        }

        // добавить динамическую библиотеку
        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.Filter = "Dynamic link library (*.dll)|*.dll";

            if (openFileDialog2.ShowDialog() == DialogResult.Cancel)
                return;

            links.Add(openFileDialog2.FileName);
            richTextBox1.Text += '\n' + openFileDialog2.FileName;
        }

        private void скомпилироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.ReadAllText(@"files/run-app-file.cs") != fastColoredTextBox1.Text)
            {
                File.WriteAllText(@"run-app-file.cs", fastColoredTextBox1.Text);
            }
            string sourceName = @"run-app-file.cs";
            FileInfo sourceFile = new FileInfo(sourceName);
            CodeDomProvider provider = null;

            provider = CodeDomProvider.CreateProvider("CSharp");

            if (provider != null)
            {
                String exeName = String.Format(@"{0}\{1}.exe", System.Environment.CurrentDirectory,
                    sourceFile.Name.Replace(".", "_"));

                CompilerParameters cp = new CompilerParameters(links.ToArray(),
                    fastColoredTextBox1.Text);

                cp.GenerateExecutable = true;
                cp.OutputAssembly = exeName;

                cp.GenerateInMemory = false;
                cp.TreatWarningsAsErrors = false;

                CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceName);

                if (cr.Errors.Count > 0)
                {
                    foreach (CompilerError ce in cr.Errors)
                    {
                        MessageBox.Show(ce.ToString(), "Ошибка компиляции",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                    MessageBox.Show("Компиляция прошла успешно");
            }
        }

        private void обычныйТекстToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = fastColoredTextBox1.Text;
            fastColoredTextBox1.Language = FastColoredTextBoxNS.Language.Custom;
           
            autocompleteMenu1.Enabled = false;
            fastColoredTextBox1.SelectAll();
            fastColoredTextBox1.Cut();
            fastColoredTextBox1.Text = text;
            режимToolStripMenuItem.Image = обычныйТекстToolStripMenuItem.Image;

            компиляцияToolStripMenuItem.Enabled = false;
        }

        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = fastColoredTextBox1.Text;

            fastColoredTextBox1.Language = FastColoredTextBoxNS.Language.CSharp;
            autocompleteMenu1.Enabled = true;
            fastColoredTextBox1.SelectAll();
            fastColoredTextBox1.Cut();
            fastColoredTextBox1.Text = text;
            режимToolStripMenuItem.Image = cToolStripMenuItem.Image;

            компиляцияToolStripMenuItem.Enabled = true;
        }


        private void hTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = fastColoredTextBox1.Text;

            fastColoredTextBox1.Language = FastColoredTextBoxNS.Language.HTML;
            autocompleteMenu1.Enabled = true;
            fastColoredTextBox1.SelectAll();
            fastColoredTextBox1.Cut();
            fastColoredTextBox1.Text = text;
            режимToolStripMenuItem.Image = hTMLToolStripMenuItem.Image;

            компиляцияToolStripMenuItem.Enabled = false;
        }

        private void cSSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = fastColoredTextBox1.Text;

            fastColoredTextBox1.Language = FastColoredTextBoxNS.Language.XML;
            autocompleteMenu1.Enabled = true;
            fastColoredTextBox1.SelectAll();
            fastColoredTextBox1.Cut();
            fastColoredTextBox1.Text = text;
            режимToolStripMenuItem.Image = xMLToolStripMenuItem.Image;

            компиляцияToolStripMenuItem.Enabled = false;
        }

        private void jSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = fastColoredTextBox1.Text;

            fastColoredTextBox1.Language = FastColoredTextBoxNS.Language.JSON;
            autocompleteMenu1.Enabled = true;
            fastColoredTextBox1.SelectAll();
            fastColoredTextBox1.Cut();
            fastColoredTextBox1.Text = text;
            режимToolStripMenuItem.Image = jSONToolStripMenuItem.Image;

            компиляцияToolStripMenuItem.Enabled = false;
        }

        private void скомпилироватьИЗапуститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.ReadAllText(@"files/run-app-file.cs") != fastColoredTextBox1.Text)
            {
                File.WriteAllText(@"run-app-file.cs", fastColoredTextBox1.Text);
            }
            string sourceName = @"run-app-file.cs";
            FileInfo sourceFile = new FileInfo(sourceName);
            CodeDomProvider provider = null;

            provider = CodeDomProvider.CreateProvider("CSharp");

            if (provider != null)
            {
                String exeName = String.Format(@"{0}\{1}.exe", System.Environment.CurrentDirectory,
                    sourceFile.Name.Replace(".", "_"));

                CompilerParameters cp = new CompilerParameters(links.ToArray(),
                    fastColoredTextBox1.Text);

                cp.GenerateExecutable = true;
                cp.OutputAssembly = exeName;

                cp.GenerateInMemory = false;
                cp.TreatWarningsAsErrors = false;

                CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceName);

                if (cr.Errors.Count > 0)
                    foreach (CompilerError ce in cr.Errors)
                    {
                        MessageBox.Show(ce.ToString(), "Ошибка компиляции",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
                    }

                if (cr.Errors.Count == 0)
                {
                    Process.Start(@"run-app-file_cs.exe");
                }
            }
        }

        private void запуститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(@"run-app-file_cs.exe");
        }

        private void шрифтToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog1.ShowDialog();
            fastColoredTextBox1.Font = fontDialog1.Font;
        }

        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fastColoredTextBox1.TextLength > 0)
            {
                fastColoredTextBox1.Cut();
            }
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fastColoredTextBox1.TextLength > 0)
            {
                fastColoredTextBox1.Copy();
            }
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fastColoredTextBox1.TextLength > 0)
            {
                fastColoredTextBox1.Paste();
            }
        }

        private void выделитьВсёToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fastColoredTextBox1.TextLength > 0)
            {
                fastColoredTextBox1.SelectAll();
            }
        }

        private void датаИВремяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Text += DateTime.Now;
        }

        private void фонToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            fastColoredTextBox1.BackColor = colorDialog1.Color;
        }

        private void fastColoredTextBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                fastColoredTextBox1.ContextMenuStrip = contextMenuStrip1;
            }
        }

        private void копироватьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (fastColoredTextBox1.TextLength > 0)
            {
                fastColoredTextBox1.Copy();
            }
        }

        private void вырезатьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (fastColoredTextBox1.TextLength > 0)
            {
                fastColoredTextBox1.Cut();
            }
        }

        private void вставитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (fastColoredTextBox1.TextLength > 0)
            {
                fastColoredTextBox1.Paste();
            }
        }

        private void выделитьВсёToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (fastColoredTextBox1.TextLength > 0)
            {
                fastColoredTextBox1.SelectAll();
            }
        }

        // Если файл отредактирован
        private void fastColoredTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            fileIsChanged = true;
        }

        // Если файл отредактирован
        private void fastColoredTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            fileIsChanged = true;
        }

        private void заменаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel3.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string saved_text = fastColoredTextBox1.Text;
            string new_text = null;

            string replace_what = textBox1.Text;
            string replace_with = textBox2.Text;

            if (checkBox1.Checked)
                new_text = Regex.Replace(saved_text, replace_what, replace_with,
                    RegexOptions.Singleline);
            else
                new_text = Regex.Replace(saved_text, replace_what, replace_with);

            fastColoredTextBox1.Text = new_text;

            panel3.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string saved_text = fastColoredTextBox1.Text;
            string new_text = null;

            string replace_what = textBox1.Text;
            string replace_with = textBox2.Text;

            if (checkBox1.Checked)
                new_text = Regex.Replace(saved_text, replace_what, replace_with,
                    RegexOptions.IgnoreCase);
            else
                new_text = Regex.Replace(saved_text, replace_what, replace_with);

            fastColoredTextBox1.Text = new_text;

            panel3.Hide();
        }

        // Закрыть меню замены
        private void button5_Click(object sender, EventArgs e)
        {
            panel3.Hide();
        }

        // Спросить про сохранение файла при закрытии
        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if (fileIsChanged)
            {
                DialogResult dialogResult = MessageBox.Show("Сохранить изменения перед закрытием приложения?", 
                    "SharpEditor", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        if (fileIsOpen)
                            File.WriteAllText(openFile, fastColoredTextBox1.Text);
                        else
                        {
                            string filter = null;

                            switch (fastColoredTextBox1.Language)
                            {
                                case FastColoredTextBoxNS.Language.Custom:
                                    filter = "Text File (*.txt)|*.txt | " +
                                        "Sharp Editor File(*.sef)| *.sef"; break;
                                case FastColoredTextBoxNS.Language.CSharp:
                                    filter = "CSharp source code (*.cs)|*.cs"; break;
                                case FastColoredTextBoxNS.Language.HTML:
                                    filter = "HTML file (*.html)|*.thml"; break;
                                case FastColoredTextBoxNS.Language.JSON:
                                    filter = "JSON file (*.json)|*.json"; break;
                                case FastColoredTextBoxNS.Language.XML:
                                    filter = "XML File (*.xml)|*.xml"; break;
                            }

                            saveFileDialog1.Filter = filter;

                            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                                return;

                            File.WriteAllText(saveFileDialog1.FileName, fastColoredTextBox1.Text);
                        }

                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Ошибка", "Не удалось сохранить файл",
                            MessageBoxButtons.OK);
                    }
                }
            }

        }

        private void темаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.WhiteSmoke;
            fastColoredTextBox1.BackColor = Color.WhiteSmoke;
            fastColoredTextBox1.ForeColor = Color.Black;
            fastColoredTextBox1.IndentBackColor = Color.Bisque;
            fastColoredTextBox1.LineNumberColor = Color.Orange;
            fastColoredTextBox1.ServiceLinesColor = Color.DarkGoldenrod;
            menuStrip1.BackColor = SystemColors.ActiveCaption;
            label1.ForeColor = Color.Black;
            label2.ForeColor = Color.Black;
            panel1.BackColor = SystemColors.ActiveCaption;
            panel1.ForeColor = Color.WhiteSmoke;
            panel2.ForeColor = Color.WhiteSmoke;
            panel3.ForeColor = Color.Black;
            panel3.BackColor = SystemColors.ControlLight;
            button3.BackColor = Color.WhiteSmoke;
            button4.BackColor = Color.WhiteSmoke;

            menuStrip1.ForeColor = Color.Black;

            правкаToolStripMenuItem.BackColor = Color.WhiteSmoke;
            вырезатьToolStripMenuItem.BackColor = Color.WhiteSmoke;
            заменаToolStripMenuItem.BackColor = Color.WhiteSmoke;
            вставитьToolStripMenuItem.BackColor = Color.WhiteSmoke;
            копироватьToolStripMenuItem.BackColor = Color.WhiteSmoke;
            датаИВремяToolStripMenuItem.BackColor = Color.WhiteSmoke;
            выделитьВсёToolStripMenuItem.BackColor = Color.WhiteSmoke;
            шрифтToolStripMenuItem.BackColor = Color.WhiteSmoke;
            фонToolStripMenuItem.BackColor = Color.WhiteSmoke;
            обычныйТекстToolStripMenuItem.BackColor = Color.WhiteSmoke;
            hTMLToolStripMenuItem.BackColor = Color.WhiteSmoke;
            xMLToolStripMenuItem.BackColor = Color.WhiteSmoke;
            jSONToolStripMenuItem.BackColor = Color.WhiteSmoke;
            cToolStripMenuItem.BackColor = Color.WhiteSmoke;
            открытьToolStripMenuItem.BackColor = Color.WhiteSmoke;
            сохранитьToolStripMenuItem.BackColor = Color.WhiteSmoke;
            сохранитьКакToolStripMenuItem.BackColor = Color.WhiteSmoke;
            копироватьToolStripMenuItem1.BackColor = Color.WhiteSmoke;
            выделитьВсёToolStripMenuItem1.BackColor = Color.WhiteSmoke;
            вырезатьToolStripMenuItem1.BackColor = Color.WhiteSmoke;
            выделитьВсёToolStripMenuItem1.BackColor = Color.WhiteSmoke;
            вставитьToolStripMenuItem1.BackColor = Color.WhiteSmoke;
            открытьМенеджерСсылокToolStripMenuItem.BackColor = Color.WhiteSmoke;
            скомпилироватьToolStripMenuItem.BackColor = Color.WhiteSmoke;
            скомпилироватьИЗапуститьToolStripMenuItem.BackColor = Color.WhiteSmoke;
            запуститьToolStripMenuItem.BackColor = Color.WhiteSmoke;

            правкаToolStripMenuItem.ForeColor = Color.Black;
            вырезатьToolStripMenuItem.ForeColor = Color.Black;
            заменаToolStripMenuItem.ForeColor = Color.Black;
            вставитьToolStripMenuItem.ForeColor = Color.Black;
            копироватьToolStripMenuItem.ForeColor = Color.Black;
            датаИВремяToolStripMenuItem.ForeColor = Color.Black;
            выделитьВсёToolStripMenuItem.ForeColor = Color.Black;
            шрифтToolStripMenuItem.ForeColor = Color.Black;
            фонToolStripMenuItem.ForeColor = Color.Black;
            обычныйТекстToolStripMenuItem.ForeColor = Color.Black;
            hTMLToolStripMenuItem.ForeColor = Color.Black;
            xMLToolStripMenuItem.ForeColor = Color.Black;
            jSONToolStripMenuItem.ForeColor = Color.Black;
            cToolStripMenuItem.ForeColor = Color.Black;
            открытьToolStripMenuItem.ForeColor = Color.Black;
            сохранитьToolStripMenuItem.ForeColor = Color.Black;
            сохранитьКакToolStripMenuItem.ForeColor = Color.Black;
            копироватьToolStripMenuItem1.ForeColor = Color.Black;
            выделитьВсёToolStripMenuItem1.ForeColor = Color.Black;
            вырезатьToolStripMenuItem1.ForeColor = Color.Black;
            выделитьВсёToolStripMenuItem1.ForeColor = Color.Black;
            вставитьToolStripMenuItem1.ForeColor = Color.Black;
            открытьМенеджерСсылокToolStripMenuItem.ForeColor = Color.Black;
            скомпилироватьToolStripMenuItem.ForeColor = Color.Black;
            скомпилироватьИЗапуститьToolStripMenuItem.ForeColor = Color.Black;
            запуститьToolStripMenuItem.ForeColor = Color.Black;

            panel2.BackColor = SystemColors.ActiveCaption;
            richTextBox1.BackColor = Color.WhiteSmoke;
            richTextBox1.ForeColor = Color.Black;
        }

        private void темноToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(255, 40, 40, 40);
            fastColoredTextBox1.BackColor = Color.FromArgb(255, 50, 50, 50);
            fastColoredTextBox1.ForeColor = Color.White;
            fastColoredTextBox1.IndentBackColor = Color.DarkSlateBlue;
            fastColoredTextBox1.LineNumberColor = Color.WhiteSmoke;
            fastColoredTextBox1.ServiceLinesColor = Color.WhiteSmoke;
            menuStrip1.BackColor = Color.FromArgb(255, 40, 40, 40);
            label1.ForeColor = Color.White;
            label2.ForeColor = Color.White;
            panel1.BackColor = Color.FromArgb(255, 40, 40, 40);
            panel1.ForeColor = Color.FromArgb(255, 40, 40, 40);
            panel2.ForeColor = Color.FromArgb(255, 40, 40, 40);
            panel3.ForeColor = Color.White;
            panel3.BackColor = Color.FromArgb(255, 40, 40, 40);
            button3.BackColor = Color.FromArgb(255, 40, 40, 40);
            button4.BackColor = Color.FromArgb(255, 40, 40, 40);

            menuStrip1.ForeColor = Color.White;

            правкаToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            вырезатьToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            заменаToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            вставитьToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            копироватьToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            датаИВремяToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            выделитьВсёToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            шрифтToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            фонToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            обычныйТекстToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            hTMLToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            xMLToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            jSONToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            cToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            открытьToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            сохранитьToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            сохранитьКакToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            копироватьToolStripMenuItem1.BackColor = Color.FromArgb(255, 40, 40, 40);
            выделитьВсёToolStripMenuItem1.BackColor = Color.FromArgb(255, 40, 40, 40);
            вырезатьToolStripMenuItem1.BackColor = Color.FromArgb(255, 40, 40, 40);
            выделитьВсёToolStripMenuItem1.BackColor = Color.FromArgb(255, 40, 40, 40);
            вставитьToolStripMenuItem1.BackColor = Color.FromArgb(255, 40, 40, 40);
            открытьМенеджерСсылокToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            скомпилироватьToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            скомпилироватьИЗапуститьToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);
            запуститьToolStripMenuItem.BackColor = Color.FromArgb(255, 40, 40, 40);

            правкаToolStripMenuItem.ForeColor = Color.White;
            вырезатьToolStripMenuItem.ForeColor = Color.White;
            заменаToolStripMenuItem.ForeColor = Color.White;
            вставитьToolStripMenuItem.ForeColor = Color.White;
            копироватьToolStripMenuItem.ForeColor = Color.White;
            датаИВремяToolStripMenuItem.ForeColor = Color.White;
            выделитьВсёToolStripMenuItem.ForeColor = Color.White;
            шрифтToolStripMenuItem.ForeColor = Color.White;
            фонToolStripMenuItem.ForeColor = Color.White;
            обычныйТекстToolStripMenuItem.ForeColor = Color.White;
            hTMLToolStripMenuItem.ForeColor = Color.White;
            xMLToolStripMenuItem.ForeColor = Color.White;
            jSONToolStripMenuItem.ForeColor = Color.White;
            cToolStripMenuItem.ForeColor = Color.White;
            открытьToolStripMenuItem.ForeColor = Color.White;
            сохранитьToolStripMenuItem.ForeColor = Color.White;
            сохранитьКакToolStripMenuItem.ForeColor = Color.White;
            копироватьToolStripMenuItem1.ForeColor = Color.White;
            выделитьВсёToolStripMenuItem1.ForeColor = Color.White;
            вырезатьToolStripMenuItem1.ForeColor = Color.White;
            выделитьВсёToolStripMenuItem1.ForeColor = Color.White;
            вставитьToolStripMenuItem1.ForeColor = Color.White;
            открытьМенеджерСсылокToolStripMenuItem.ForeColor = Color.White;
            скомпилироватьToolStripMenuItem.ForeColor = Color.White;
            скомпилироватьИЗапуститьToolStripMenuItem.ForeColor = Color.White;
            запуститьToolStripMenuItem.ForeColor = Color.White;

            panel2.BackColor = Color.Gray;
            richTextBox1.BackColor = Color.FromArgb(255, 40, 40, 40);
            richTextBox1.ForeColor = Color.White;
        }
    }
}
