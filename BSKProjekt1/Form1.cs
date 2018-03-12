using BSKProject1;
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

        private static string workSpace = AppDomain.CurrentDomain.BaseDirectory;

        public Form1()
        {
            InitializeComponent();
            trybSzyfrowaniaComboBox.SelectedIndex = 0;
            dlugoscKluczaComboBox.SelectedIndex = 2;
            dlugoscPodblokuComboBox.Enabled = false;
            dlugoscPodblokuComboBox.SelectedIndex = 4;
            DirectoryInfo directory = new DirectoryInfo(workSpace);
            List<ListViewItem> items = new List<ListViewItem>();
            FileInfo[] files = directory.GetFiles();
            for(int i=0;i<files.Length;i++)
            {
                if(files[i].Name.Contains(".public"))
                    items.Add(new ListViewItem(files[i].Name));
            }
            uzytkownicyListView.Items.AddRange(items.ToArray());
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
            try
            {
                FileInfo plikSzyfrowania = new FileInfo(fileDialog.FileName);
                plikSzyfrowaniaTextBox.Text = plikSzyfrowania.FullName;
            }catch(ArgumentException ex)
            {
                Console.WriteLine(ex);
            }
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

        private void dodajUzytkownikaButton_Click(object sender, EventArgs e)
        {
            AddUserForm form = new AddUserForm();
            form.Show();
        }
    }
}
