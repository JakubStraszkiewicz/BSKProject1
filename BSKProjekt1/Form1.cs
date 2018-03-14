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
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Xml;

namespace BSKProjekt1
{
    public partial class Form1 : Form
    {
        private struct User
        {
            public string name;
            public string sessionKey;
        }

        private static string workSpace = AppDomain.CurrentDomain.BaseDirectory;
        private List<User> users = new List<User>();

        public Form1()
        {
            InitializeComponent();
            trybSzyfrowaniaComboBox.SelectedIndex = 0;
            dlugoscKluczaComboBox.SelectedIndex = 2;
            dlugoscPodblokuComboBox.Enabled = false;
            dlugoscPodblokuComboBox.SelectedIndex = 4;
            uzytkownicyListView.View = View.List;
            odbiorcyListView.View = View.List;
            uzytkownicylistViewUpdate();
        }

        public bool checkTextExistListView(string text)
        {
            return (uzytkownicyListView.FindItemWithText(text) == null) &&
                    (odbiorcyListView.FindItemWithText(text) == null);
        }

        public void uzytkownicylistViewUpdate()
        {

            uzytkownicyListView.Items.Clear();
            DirectoryInfo directory = new DirectoryInfo(workSpace);
            List<ListViewItem> items = new List<ListViewItem>();
            FileInfo[] files = directory.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.Contains(".public") &&
                    checkTextExistListView(files[i].Name.Split('.')[0]))
                        items.Add(new ListViewItem(files[i].Name.Split('.')[0]));
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
            AddUserForm form = new AddUserForm(this);
            form.Show();
        }

        private byte[] generateSessionKey()
        {

            DateTime thisTime = DateTime.Now;
            byte[] sessionKey = new byte[Convert.ToInt32(dlugoscKluczaComboBox.SelectedItem) / 8];
            Random random = new Random(thisTime.Hour + thisTime.Minute + thisTime.Second + thisTime.Millisecond);
            random.NextBytes(sessionKey);
            return sessionKey;
        }

        private void dodajOdbiorceButton_Click(object sender, EventArgs e)
        {
            User user = new User();
            List<ListViewItem> items = new List<ListViewItem>();
            ListView.SelectedListViewItemCollection selectedItems = uzytkownicyListView.SelectedItems;
            foreach(ListViewItem item in selectedItems)
            {
                uzytkownicyListView.Items.Remove(item);
                items.Add(item);
            }
            odbiorcyListView.Items.AddRange(items.ToArray());
        }

        private void usunOdbiorceButton_Click(object sender, EventArgs e)
        {
            List<ListViewItem> items = new List<ListViewItem>();
            ListView.SelectedListViewItemCollection selectedItems = odbiorcyListView.SelectedItems;
            foreach (ListViewItem item in selectedItems)
            {
                odbiorcyListView.Items.Remove(item);
                items.Add(item);
            }
            uzytkownicyListView.Items.AddRange(items.ToArray());
        }

        private void szyforwanieButton_Click(object sender, EventArgs e)
        {
            User user = new User();
            users.Clear();
            List<ListViewItem> items = new List<ListViewItem>();
            ListView.ListViewItemCollection selectedItems = odbiorcyListView.Items;
            foreach (ListViewItem item in selectedItems)
            {
                user.name = item.Text;
                user.sessionKey = Encoding.ASCII.GetString(generateSessionKey());
                users.Add(user);
            }
            CryptoService service = new CryptoService();
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.GenerateIV();
            /*string encoding = service.aesEncoding(user.sessionKey, "ECB", 128, "wiadomosc oby dzialalo",aes.IV);
            string decoding = service.aesDecoding(user.sessionKey, "ECB", 128, encoding,aes.IV);
            Console.WriteLine(decoding);*/
            try
            {
                createEndFile(users, aes, plikSzyfrowaniaTextBox.Text.Split('.')[1]);
            }
            catch(IndexOutOfRangeException ex)
            {
                MessageBox.Show("Nie wybrano pliku do zaszyfrowania",
                    "Brak pliku", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
        }

        private void createEndFile(List<User> users, AesCryptoServiceProvider aes, string extension)
        {
            XmlTextWriter writer = new XmlTextWriter("code.xml", Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;

            writer.WriteStartElement("EncryptedFileHeader");

            writeElement("Algorithm", "AES", writer);
            writeElement("KeySize", dlugoscKluczaComboBox.Text, writer);
            writeElement("BlockSize", "128", writer);
            writeElement("Subblock", dlugoscPodblokuComboBox.Text, writer);
            writeElement("Mode", trybSzyfrowaniaComboBox.Text, writer);
            writeElement("IV", aes.IV, writer);
            writeElement("Extension", extension, writer);

            writer.WriteStartElement("ApprovedUsers");

            foreach (User singleUser in users)
            {
                writer.WriteStartElement("User");
                writeElement("Name", singleUser.name, writer);
                writeElement("sessionKey", ASCIIEncoding.ASCII.GetBytes(singleUser.sessionKey), writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        private void writeElement(string element, object value, XmlTextWriter writer)
        {
            writer.WriteStartElement(element);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }
    }
}
