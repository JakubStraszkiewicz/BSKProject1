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
        private string privateKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "private");
        private string publicKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "public");
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
            DirectoryInfo privateKeyTargetDirectory = new DirectoryInfo(privateKeyPath);
            DirectoryInfo publicKeyTargetDirectory = new DirectoryInfo(publicKeyPath);
            if (!privateKeyTargetDirectory.Exists)
                privateKeyTargetDirectory.Create();
            if (!publicKeyTargetDirectory.Exists)
                publicKeyTargetDirectory.Create();
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
            DirectoryInfo directory = new DirectoryInfo(publicKeyPath);
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
            if(plikDeszyfrowaniaTextBox.Text == "")
                return;

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

        private bool isEncodingPossible()
        {
            if (plikSzyfrowaniaTextBox.Text == "")
                MessageBox.Show("Nie wybrano pliku do zaszyfrowania",
                   "Wybierz plik", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            if (lokalizacjaSzyfrowaniaTextBox.Text == "")
                MessageBox.Show("Nie wybrano docelowej lokalizacji",
                  "Wybierz lokalizacje", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            if (nazwaPlikuSzyfrowanegoTextBox.Text == "")
                MessageBox.Show("Nie wprowadzono nowej nazwy dla pliku zaszyfrowanego",
                  "Wpisz nazwe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            if (odbiorcyListView.SelectedItems.Count == 0)
                MessageBox.Show("Nie wybrano odbiorców wiadomosci",
                 "Wybierz odbiorców", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                return true;
            return false;
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

           /* using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                RSAParameters privateKey = rsa.ExportParameters(true);
                RSAParameters publicKey = rsa.ExportParameters(false);
                byte[] rsaEncoding = service.rsaEncoding(ASCIIEncoding.ASCII.GetBytes("wiadomosc"), publicKey);
                byte[] rsaDecoding = service.rsaDecoding(rsaEncoding, privateKey);
                Console.WriteLine(ASCIIEncoding.ASCII.GetString(rsaDecoding));
            }*/
            

            // loadRsaKey(users[0].name,false);
            if (!isEncodingPossible())
                return;

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
                createEndFile(users, aes, plikSzyfrowaniaTextBox.Text.Substring(plikSzyfrowaniaTextBox.Text.LastIndexOf('.') + 1), message);
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
            CryptoService service = new CryptoService();

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
                singleUser.publicKey = loadPublicKey(singleUser.name);
                singleUser.sessionKey = service.rsaEncoding(singleUser.sessionKey, singleUser.publicKey);
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

        public void writeElement(string element, object value, XmlTextWriter writer)
        {
            writer.WriteStartElement(element);
            if(value != null)
                writer.WriteValue(value);
            writer.WriteEndElement();
        }

        private bool isDecodingPossible()
        {
            if (plikDeszyfrowaniaTextBox.Text == "")
                MessageBox.Show("Nie wybrano pliku do zdeszyfrowania",
                   "Wybierz plik", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            if (lokalizacjaDeszyfrowaniaTextBox.Text == "")
                MessageBox.Show("Nie wybrano docelowej lokalizacji",
                  "Wybierz lokalizacje", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            if (nazwaPlikuDeszyfrowanegoTextBox.Text == "")
                MessageBox.Show("Nie wprowadzono nowej nazwy dla pliku zdeszyfrowanego",
                  "Wpisz nazwe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            if (deszyfratorListView.SelectedItems.Count == 0)
                MessageBox.Show("Nie wybrano użytkownika",
                 "Wybierz użytkownika", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            if (hasloTextBox.Text == "")
                MessageBox.Show("Nie wpisano hasła",
                 "Wpisz hasło", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                return true;
            return false;
        }

        private void deszyfrowanieButton_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedItems = deszyfratorListView.SelectedItems;
            int keySize = 0;
            byte[] iv = { };
            byte[] sessionKey = { };
            byte[] blockOfFile = { };
            byte[] encryptedFile = { };
            bool fileDecryptionBegin = false;
            string mode = "";
            string extension = "";

            if (!isDecodingPossible())
                return;

            List<byte[]> decryptedFile = new List<byte[]>();
            CryptoService service = new CryptoService();
            RSAParameters privatekey = new RSAParameters();

            deszyfrowanieProgressBar.Minimum = 1;
            deszyfrowanieProgressBar.Maximum = (int)(new FileInfo(plikDeszyfrowaniaTextBox.Text)).Length;

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
                                try
                                {
                                    byte[] hash = service.createSha512Hash(hasloTextBox.Text, 16);
                                    privatekey = loadPrivateKey(hash, item.Text);
                                    sessionKey = service.rsaDecoding(sessionKey, privatekey);
                                }
                                catch (System.Security.Cryptography.CryptographicException)
                                {
                                    return;
                                }
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
                        decryptedFile.Add(service.aesDecoding(sessionKey, mode , 128, blockOfFile, iv));
                        deszyfrowanieProgressBar.Step = ASCIIEncoding.Unicode.GetByteCount(line);
                        deszyfrowanieProgressBar.PerformStep();
                    }
                }                
            }

            using (FileStream stream = new FileStream(lokalizacjaDeszyfrowaniaTextBox.Text + "\\" + nazwaPlikuDeszyfrowanegoTextBox.Text + "." + extension,FileMode.Create))
            {
                foreach (byte[] line in decryptedFile)
                    stream.Write(line, 0, line.Length);
            }

        }

        private string takeValueFromNode(string node)
        {
            return node.Substring(node.IndexOf(">") + 1, node.LastIndexOf("<") - node.IndexOf(">") - 1);
        }

        private RSAParameters loadPrivateKey(byte[] password,string userName)
        {
            string line = "";
            string stringKey = "";
            byte[] encrypted = { };
            RSAParameters key = new RSAParameters();
            byte[] blockOfFile = { };
            CryptoService service = new CryptoService();
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.GenerateIV();
            using (System.IO.StreamReader fileReader = new System.IO.StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "private", userName + ".private1"), Encoding.UTF8))
            {
                while ((line = fileReader.ReadLine()) != null)
                {
                    blockOfFile = Convert.FromBase64String(line.Substring(0, line.Length));
                    encrypted = (service.aesDecoding(password, "ECB", 128, blockOfFile, aes.IV));
                    stringKey += ASCIIEncoding.ASCII.GetString(encrypted);
                }
                if (stringKey.Contains("<D>"))
                {
                    key.D = Convert.FromBase64String(stringKey.Substring(stringKey.IndexOf("<D>") + 3, stringKey.IndexOf("</D>") - stringKey.IndexOf("<D>") - 3));
                }
                if (stringKey.Contains("<DP>"))
                {
                    key.DP = Convert.FromBase64String(stringKey.Substring(stringKey.IndexOf("<DP>") + 4, stringKey.IndexOf("</DP>") - stringKey.IndexOf("<DP>") - 4));
                }
                if (stringKey.Contains("<DQ>"))
                {
                    key.DQ = Convert.FromBase64String(stringKey.Substring(stringKey.IndexOf("<DQ>") + 4, stringKey.IndexOf("</DQ>") - stringKey.IndexOf("<DQ>") - 4));
                }
                if (stringKey.Contains("<Exponent>"))
                {
                    key.Exponent = Convert.FromBase64String(stringKey.Substring(stringKey.IndexOf("<Exponent>") + 10, stringKey.IndexOf("</Exponent>") - stringKey.IndexOf("<Exponent>") - 10));
                }
                if (stringKey.Contains("<InverseQ>"))
                {
                    key.InverseQ = Convert.FromBase64String(stringKey.Substring(stringKey.IndexOf("<InverseQ>") + 10, stringKey.IndexOf("</InverseQ>") - stringKey.IndexOf("<InverseQ>") - 10));
                }
                if (stringKey.Contains("<Modulus>"))
                {
                    key.Modulus = Convert.FromBase64String(stringKey.Substring(stringKey.IndexOf("<Modulus>") + 9, stringKey.IndexOf("</Modulus>") - stringKey.IndexOf("<Modulus>") - 9));
                }
                if (stringKey.Contains("<P>"))
                {
                    key.P = Convert.FromBase64String(stringKey.Substring(stringKey.IndexOf("<P>") + 3, stringKey.IndexOf("</P>") - stringKey.IndexOf("<P>") - 3));
                }
                if (stringKey.Contains("<Q>"))
                {
                    key.Q = Convert.FromBase64String(stringKey.Substring(stringKey.IndexOf("<Q>") + 3, stringKey.IndexOf("</Q>") - stringKey.IndexOf("<Q>") - 3));
                }
            }
            return key;
        }

        private RSAParameters loadPublicKey(string userName)
         {
            string fileWithKey = userName +".public"; ;
            string line;
            RSAParameters key = new RSAParameters();

            using (StreamReader stream = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "public", fileWithKey)))
            {
                while(!(line = stream.ReadLine()).Contains("/RSA"))
                {
                    if(line.Contains("<D>"))
                    {
                        key.D = Convert.FromBase64String(takeValueFromNode(line));
                    }
                    if (line.Contains("<DP>"))
                    {
                        key.DP = Convert.FromBase64String(takeValueFromNode(line));
                    }
                    if (line.Contains("<DQ>"))
                    {
                        key.DQ = Convert.FromBase64String(takeValueFromNode(line));
                    }
                    if (line.Contains("<Exponent>"))
                    {
                        key.Exponent = Convert.FromBase64String(takeValueFromNode(line));
                    }
                    if (line.Contains("<InverseQ>"))
                    {
                        key.InverseQ = Convert.FromBase64String(takeValueFromNode(line));
                    }
                    if (line.Contains("<Modulus>"))
                    {
                        key.Modulus = Convert.FromBase64String(takeValueFromNode(line));
                    }
                    if (line.Contains("<P>"))
                    {
                        key.P = Convert.FromBase64String(takeValueFromNode(line));
                    }
                    if (line.Contains("<Q>"))
                    {
                        key.Q = Convert.FromBase64String(takeValueFromNode(line));
                    }
                }
            }
            return key;
        }
    }
}
