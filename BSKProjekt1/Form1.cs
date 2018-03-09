using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BSKProjekt1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            trybSzyfrowaniaComboBox.SelectedIndex = 0;
            dlugoscKluczaComboBox.SelectedIndex = 2;
            dlugoscPodblokuComboBox.Enabled = false;
            dlugoscPodblokuComboBox.SelectedIndex = 4;
        }

        private void trybSzyfrowaniaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(trybSzyfrowaniaComboBox.SelectedItem == "CFB" || trybSzyfrowaniaComboBox.SelectedItem == "OFB")
            {
                dlugoscPodblokuComboBox.Enabled = true;
            }
            else
            {
                dlugoscPodblokuComboBox.Enabled = false;
            }
        }

        private void pokazHasloButton_Click(object sender, EventArgs e)
        {
            if(hasloTextBox.PasswordChar == '*')
            {
                hasloTextBox.PasswordChar = '\0';
            }
            else
            {
                hasloTextBox.PasswordChar = '*';
            }
        }

        private void wyborPlikuSzyfrowaniaButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            FileInfo plikSzyfrowania = new FileInfo(fileDialog.FileName);
            plikSzyfrowaniaTextBox.Text = plikSzyfrowania.FullName;
        }

        private void lokalizacjaSzyfrowaniaButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.ShowDialog();
            String lokalizacjaSzyforwania = folderDialog.SelectedPath;
            lokalizacjaSzyfrowaniaTextBox.Text = lokalizacjaSzyforwania;
        }

        private void wyborPlikuDeszyfrowaniaButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            FileInfo plikDeszyfrowania = new FileInfo(fileDialog.FileName);
            plikDeszyfrowaniaTextBox.Text = plikDeszyfrowania.FullName;
        }

        private void lokalizacjaDeszyfrowaniaButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.ShowDialog();
            String lokalizacjaDeszyforwania = folderDialog.SelectedPath;
            lokalizacjaDeszyfrowaniaTextBox.Text = lokalizacjaDeszyforwania;
        }
    }
}
