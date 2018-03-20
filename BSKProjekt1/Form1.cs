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

        private static string workSpace = AppDomain.CurrentDomain.BaseDirectory;
        private List<User> users = new List<User>();
        private static int sizeOfBlock = 1000000;

        public Form1()
        {
            InitializeComponent();
            trybSzyfrowaniaComboBox.SelectedIndex = 0;
            dlugoscPodblokuComboBox.SelectedIndex = 4;
            dlugoscKluczaComboBox.SelectedIndex = 2;
            dlugoscPodblokuComboBox.Enabled = false;
            uzytkownicyListView.View = View.List;
            deszyfratorListView.View = View.List;
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
            plikDeszyfrowaniaTextBox.Text = fileDialog.FileName;
            using (StreamReader stream = new StreamReader(plikDeszyfrowaniaTextBox.Text))
            {
                string line = "";
                do
                {
                    line = stream.ReadLine();
                    if(line.Contains("Name"))
                    {
                        deszyfratorListView.Items.Add(line.Substring(line.IndexOf(">") + 1,line.LastIndexOf("<") - line.IndexOf(">") - 1));
                    }
                } while (!line.Contains("</ApprovedUsers>"));
            }
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

        private byte[] mergeArray(byte[] array1, byte[] array2)
        {
            int length = array1.Length + array2.Length;
            byte[] sum = new byte[length];
            array1.CopyTo(sum, 0);
            array2.CopyTo(sum, array1.Length);
            return sum;
        }

        private void szyforwanieButton_Click(object sender, EventArgs e)
        {
            users.Clear();
            List<ListViewItem> items = new List<ListViewItem>();
            ListView.ListViewItemCollection selectedItems = odbiorcyListView.Items;
            byte[] sessionKey = generateSessionKey();
            foreach (ListViewItem item in selectedItems)
            {
                User user = new User();
                user.name = item.Text;
                user.sessionKey = sessionKey;
                users.Add(user);
            }

            CryptoService service = new CryptoService();
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.GenerateIV();

            List<byte[]> message = new List<byte[]>();
            byte[] blockOfFile = { };
            int readedByte = 0;
            byte[] file = File.ReadAllBytes(plikSzyfrowaniaTextBox.Text);
            szyfrowanieProgressBar.Minimum = 1;
            szyfrowanieProgressBar.Maximum = file.Length;
            szyfrowanieProgressBar.Step = sizeOfBlock;
            while(readedByte < file.Length- sizeOfBlock)
            {
                blockOfFile = file.Skip(readedByte).Take(sizeOfBlock).ToArray<byte>();
                readedByte += sizeOfBlock;
                message.Add(service.aesEncoding(sessionKey, trybSzyfrowaniaComboBox.Text, 128, blockOfFile, aes.IV));
                szyfrowanieProgressBar.PerformStep();
            }

            blockOfFile = file.Skip(readedByte).Take(file.Length-readedByte).ToArray<byte>();
            message.Add(service.aesEncoding(sessionKey, trybSzyfrowaniaComboBox.Text, 128, blockOfFile, aes.IV));
            szyfrowanieProgressBar.PerformStep();
            
          /*  byte[] message1 = ASCIIEncoding.ASCII.GetBytes("wiadomosc oby dz");
            byte[] message2 = ASCIIEncoding.ASCII.GetBytes("ialalo");
            byte[] encoding = service.aesEncoding(sessionKey, "ECB", 128, message1,aes.IV);
            byte[] encoding2 = service.aesEncoding(sessionKey, "ECB", 128, message2, aes.IV);
            encoding = mergeArray(encoding, encoding2);
            int encodingbyte = 0;
            byte[] blockOfFile1 = { };
            byte[] decryptedBlock = { };
            byte[] endfile = { };
            while(encodingbyte < encoding.Length)
            {
                blockOfFile1 = encoding.Skip(encodingbyte).Take(32).ToArray<byte>();
                encodingbyte += 32;
                decryptedBlock = service.aesDecoding(sessionKey, trybSzyfrowaniaComboBox.Text, 128, blockOfFile1, aes.IV);
                endfile = mergeArray(endfile, decryptedBlock);

            }*/
            //byte[] decoding = service.aesDecoding(sessionKey, "ECB", 128, encoding,aes.IV);
           // Console.WriteLine(ASCIIEncoding.ASCII.GetString(endfile));
            try
            {
                createEndFile(users, aes, plikSzyfrowaniaTextBox.Text.Split('.')[1], message);
            }
            catch (IndexOutOfRangeException ex)
            {
                MessageBox.Show("Nie wybrano pliku do zaszyfrowania",
                    "Brak pliku", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
        }

        private void createEndFile(List<User> users, AesCryptoServiceProvider aes, string extension,List<byte[]> message)
        {
            XmlTextWriter writer = new XmlTextWriter(lokalizacjaSzyfrowaniaTextBox.Text + "\\" + 
                                        nazwaPlikuSzyfrowanegoTextBox.Text + ".xml", Encoding.UTF8);
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
                writeElement("sessionKey", singleUser.sessionKey, writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteStartElement("EnFile");
            writer.WriteWhitespace("\n");
            foreach (byte[] block in message)
            {
                writer.WriteBase64(block, 0, block.Length);
                writer.WriteWhitespace("\n");
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
        
        private string takeValueFromNode(string node)
        {
            return node.Substring(node.IndexOf(">") + 1, node.LastIndexOf("<") - node.IndexOf(">") - 1);
        }

        private void deszyfrowanieButton_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedItems = deszyfratorListView.SelectedItems;
            int keySize;
            int decryptedByte = 0;
            byte[] sessionKey = { };
            byte[] iv = { };
            byte[] encryptedFile = { };
            byte[] blockOfFile = { };
            bool fileDecryptionBegin = false;
            string extension = "";
            string mode = "";
            List<byte[]> decryptedFile = new List<byte[]>();
            CryptoService service = new CryptoService();

            deszyfrowanieProgressBar.Minimum = 1;
            deszyfrowanieProgressBar.Maximum = encryptedFile.Length;
            deszyfrowanieProgressBar.Step = 2 * sizeOfBlock;

            using (StreamReader stream = new StreamReader(plikDeszyfrowaniaTextBox.Text, Encoding.UTF8))
            {
                string line = "";
                while((line = stream.ReadLine()) != null)
                {
                    
                    if (line.Contains("<Name>"))
                    {
                        foreach(ListViewItem item in selectedItems)
                        {
                            if(item.Text == takeValueFromNode(line))
                            {
                                line = stream.ReadLine();
                                sessionKey = Convert.FromBase64String(takeValueFromNode(line));
                            }
                        }                  
                    }
                    if (line.Contains("<Extension>"))
                    {
                        extension = takeValueFromNode(line);
                    }
                    if (line.Contains("<KeySize>"))
                    {
                        keySize = Convert.ToInt32(takeValueFromNode(line));
                    }
                    if (line.Contains("<IV>"))
                    {
                        iv = Convert.FromBase64String(takeValueFromNode(line));
                    }
                    if (line.Contains("<Mode>"))
                    {
                        mode = takeValueFromNode(line);
                    }
                    if (line.Contains("</EnFile"))
                    {
                        fileDecryptionBegin = false;
                    }
                    else
                   if (line.Contains("EnFile"))
                    {
                        fileDecryptionBegin = true;
                    }
                    else
                   if (fileDecryptionBegin)
                    {
                        blockOfFile = Convert.FromBase64String(line.Substring(0, line.Length));
                        decryptedFile.Add(service.aesDecoding(sessionKey, trybSzyfrowaniaComboBox.Text, 128, blockOfFile, iv));
                        deszyfrowanieProgressBar.PerformStep();
                    }
                }
                

            }


            /* XmlDocument document = new XmlDocument();
             document.Load(plikDeszyfrowaniaTextBox.Text);
             foreach (ListViewItem item in selectedItems)
             {

                 XmlNodeList exponentList = document.GetElementsByTagName("sessionKey");
                 foreach(XmlNode node in exponentList)
                 {
                     if (node.PreviousSibling.FirstChild.Value == item.Text)
                         sessionKey = Convert.FromBase64String(node.InnerText);
                 }
             }

             XmlNodeList extensionNode = document.GetElementsByTagName("Extension");
             foreach (XmlNode node in extensionNode)
             {
                 extension = node.InnerText;
             }

             XmlNodeList keySizeNode = document.GetElementsByTagName("KeySize");
             foreach (XmlNode node in keySizeNode)
             {
                 keySize = Convert.ToInt32(node.InnerText);
             }

             XmlNodeList fileNode = document.GetElementsByTagName("File");
             foreach (XmlNode node in fileNode)
             {
                 encryptedFile = Convert.FromBase64String(node.InnerText);
             }

             XmlNodeList ivNode = document.GetElementsByTagName("IV");
             foreach (XmlNode node in ivNode)
             {
                 iv = Convert.FromBase64String(node.InnerText);
             }

             XmlNodeList modeNode = document.GetElementsByTagName("Mode");
             foreach (XmlNode node in modeNode)
             {
                 mode = node.InnerText;
             }*/

            // byte[] blockOfFile = { };
            /* byte[] endFile = { };
             byte[] decryptedBlock = { };

             while (decryptedByte < encryptedFile.Length)
             {
                 blockOfFile = encryptedFile.Skip(decryptedByte).Take(2 * sizeOfBlock).ToArray<byte>();
                 decryptedByte += 2 * sizeOfBlock;
                 decryptedBlock = service.aesDecoding(sessionKey, trybSzyfrowaniaComboBox.Text, 128, blockOfFile, iv);
                 endFile = mergeArray(endFile, decryptedBlock);
                 deszyfrowanieProgressBar.PerformStep();
             }*/

            /*blockOfFile = encryptedFile.Skip(decryptedByte).Take(encryptedFile.Length- decryptedByte).ToArray<byte>();
            decryptedBlock = service.aesDecoding(sessionKey, trybSzyfrowaniaComboBox.Text, 128, blockOfFile,iv);
            endFile = mergeArray(endFile, decryptedBlock);*/
            //Console.WriteLine(ASCIIEncoding.ASCII.GetString(endFile));
            using (FileStream stream = new FileStream(lokalizacjaDeszyfrowaniaTextBox.Text + "\\" + nazwaPlikuDeszyfrowanegoTextBox.Text + "." + extension,FileMode.Create))
            {
                foreach (byte[] line in decryptedFile)
                    stream.Write(line, 0, line.Length);
            }
               // File.WriteAllBytes(lokalizacjaDeszyfrowaniaTextBox.Text + "\\" + nazwaPlikuDeszyfrowanegoTextBox.Text + "." + extension, );

        }

        /* private void loadPublicKey(string userName)
         {
             XmlDocument document = new XmlDocument();
             document.Load(userName + ".public");
             XmlNodeList exponentList = document.GetElementsByTagName("Exponent");
             foreach(XmlNode node in exponentList)
             {
                 Convert.ToByte(node.LastChild.Value);
             }
         }*/
    }
}
