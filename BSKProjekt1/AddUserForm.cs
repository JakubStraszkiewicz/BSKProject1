using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.IO;
using BSKProjekt1;
using System.Xml;

namespace BSKProject1
{
    public partial class AddUserForm : Form
    {
        private Form1 form;
        private string privateKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory , "private");
        private string publicKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "public");

        public AddUserForm(Form1 form)
        {
            InitializeComponent();
            this.form = form;
        }

        private void dodajUzytkownikaButton_Click(object sender, EventArgs e)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasUpperChar.IsMatch(hasloUzytkownikaTextBox.Text))
                MessageBox.Show("Hasło dostępu musi składać się z co najmniej: jednej dużej litery",
                    "Błędne hasło", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            if (!hasLowerChar.IsMatch(hasloUzytkownikaTextBox.Text))
                MessageBox.Show("Hasło dostępu musi składać się z co najmniej: jednej małej litery",
                   "Błędne hasło", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            if (!hasNumber.IsMatch(hasloUzytkownikaTextBox.Text))
                MessageBox.Show("Hasło dostępu musi składać się z co najmniej: jednej cyfry",
                    "Błędne hasło", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            if (!hasSymbols.IsMatch(hasloUzytkownikaTextBox.Text))
                MessageBox.Show("Hasło dostępu musi składać się z co najmniej: jednego znaku specjalnego",
                    "Błędne hasło", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            if (hasloUzytkownikaTextBox.Text.Length < 8)
                MessageBox.Show("Hasło dostępu musi składać się z co najmniej: 8 znaków",
                    "Błędne hasło", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            if (!form.checkTextExistListView(nazwaUzytkownikaTextBox.Text))
                MessageBox.Show("Uzytkownik o podanej nazwie istnieje",
                    "Błędna nazwa", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                createUser(nazwaUzytkownikaTextBox.Text, hasloUzytkownikaTextBox.Text);
                this.Close();
                form.uzytkownicylistViewUpdate();
            }
        }

        private User generateKey(User user)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                user.privateKey = rsa.ExportParameters(true);
                user.publicKey = rsa.ExportParameters(false);
            }
            return user;
        }

        private void savePublicKey(User user)
        {
            XmlTextWriter writer = new XmlTextWriter(Path.Combine(publicKeyPath, user.name + ".public"), Encoding.ASCII);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;

            writer.WriteStartElement("RSA");
            form.writeElement("D", user.publicKey.D, writer);
            form.writeElement("DP", user.publicKey.DP, writer);
            form.writeElement("DQ", user.publicKey.DQ, writer);
            form.writeElement("Exponent", user.publicKey.Exponent, writer);
            form.writeElement("InverseQ", user.publicKey.InverseQ, writer);
            form.writeElement("Modulus", user.publicKey.Modulus, writer);
            form.writeElement("P", user.publicKey.P, writer);
            form.writeElement("Q", user.publicKey.Q, writer);
            writer.WriteEndElement();
            writer.Close();

        }

        private void savePrivateKey(User user)
        {
            XmlTextWriter writer = new XmlTextWriter(Path.Combine(privateKeyPath, user.name + ".private"), Encoding.ASCII);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;

            writer.WriteStartElement("RSA");
            form.writeElement("D", user.privateKey.D, writer);
            form.writeElement("DP", user.privateKey.DP, writer);
            form.writeElement("DQ", user.privateKey.DQ, writer);
            form.writeElement("Exponent", user.privateKey.Exponent, writer);
            form.writeElement("InverseQ", user.privateKey.InverseQ, writer);
            form.writeElement("Modulus", user.privateKey.Modulus, writer);
            form.writeElement("P", user.privateKey.P, writer);
            form.writeElement("Q", user.privateKey.Q, writer);
            writer.WriteEndElement();
            writer.Close();

            String path = Path.Combine(privateKeyPath, nazwaUzytkownikaTextBox.Text + ".private");
            CryptoService service = new CryptoService();
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();

            byte[] file = File.ReadAllBytes(path);
            List<byte[]> message = new List<byte[]>();
            byte[] encryptedMessage = { };
            byte[] blockOfFile = { };
            int readedByte = 0;
            int sizeOfBlock = 256;
            aes.GenerateIV();

            while (readedByte < file.Length - sizeOfBlock)
            {
                blockOfFile = file.Skip(readedByte).Take(sizeOfBlock).ToArray<byte>();
                readedByte += sizeOfBlock;
                message.Add(service.aesEncoding(service.createSha512Hash(hasloUzytkownikaTextBox.Text, 16), "ECB", 128, blockOfFile, aes.IV));
            }

            blockOfFile = file.Skip(readedByte).Take(file.Length - readedByte).ToArray<byte>();
            message.Add(service.aesEncoding(service.createSha512Hash(hasloUzytkownikaTextBox.Text, 16), "ECB", 128, blockOfFile, aes.IV));

            XmlTextWriter writer2 = new XmlTextWriter(path, Encoding.UTF8);
            foreach (byte[] block in message)
            {
                writer2.WriteBase64(block, 0, block.Length);
                writer2.WriteWhitespace("\n");
            }
            writer2.Close();
        }

        private int lengthFromBitsToBytes(int lengthInBits)
        {
            return lengthInBits / 8;
        }

        private void createUser(string name, String password)
        {
            User user = new User();
            user.name = name;
            user = generateKey(user);
            savePublicKey(user);
            savePrivateKey(user);
        }
    }
}
