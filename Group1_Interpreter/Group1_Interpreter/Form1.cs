using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Group1_Interpreter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save text file";
            saveFileDialog1.ShowDialog();

            try
            {
                if (saveFileDialog1.FileName != "")
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                    {
                        sw.Write(code.Rtf);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving file: " + ex.Message);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.Title = "Select text file";
            openFileDialog1.ShowDialog();

            try
            {
                if (openFileDialog1.FileName != "")
                {
                    using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                    {
                        string fileContent = sr.ReadToEnd();

                        // Convert RTF-formatted text to plain text
                        using (RichTextBox rtb = new RichTextBox())
                        {
                            rtb.Rtf = fileContent;
                            fileContent = rtb.Text;
                        }

                        fileContent = Regex.Replace(fileContent, @"[^\u0020-\u007E]", string.Empty); // Remove non-printable characters
                        loaded.UseCompatibleTextRendering = true; // Set UseCompatibleTextRendering property to true
                        loaded.Text = fileContent;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading file: " + ex.Message);
            }
        }
    }
}
