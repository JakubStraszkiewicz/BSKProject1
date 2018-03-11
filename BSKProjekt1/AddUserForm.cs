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

namespace BSKProject1
{
    public partial class AddUserForm : Form
    {
        private struct userKeys
        {
            public RSAParameters publicKey;
            public RSAParameters privateKey;
        }

        public AddUserForm()
        {
            InitializeComponent();
        }

        private void dodajUzytkownikaButton_Click(object sender, EventArgs e)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if(!hasUpperChar.IsMatch(hasloUzytkownikaTextBox.Text))
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
            if(hasloUzytkownikaTextBox.Text.Length < 8)
                MessageBox.Show("Hasło dostępu musi składać się z co najmniej: 8 znaków",
                    "Błędne hasło", MessageBoxButtons.OK, MessageBoxIcon.Error);
            createUser(nazwaUzytkownikaTextBox.Text,hasloUzytkownikaTextBox.Text);
        }

        private userKeys generateKey()
        {
            userKeys user;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                user.privateKey = rsa.ExportParameters(true);
                user.publicKey = rsa.ExportParameters(false);
            }
            return user;
        }

        private void savePublicKey(userKeys user)
        {
            new XDocument(
                new XElement("RSA",
                    new XElement("userName", nazwaUzytkownikaTextBox.Text),
                    new XElement("publicKey",
                        new XElement("D", user.publicKey.D),
                        new XElement("DP", user.publicKey.DP),
                        new XElement("DQ", user.publicKey.DQ),
                        new XElement("Exponent", user.publicKey.Exponent),
                        new XElement("InverseQ", user.publicKey.InverseQ),
                        new XElement("Modulus", user.publicKey.Modulus),
                        new XElement("P", user.publicKey.P),
                        new XElement("Q", user.publicKey.Q))
            )
        )
        .Save(nazwaUzytkownikaTextBox.Text + ".public");
        }
        private void savePrivateKey(userKeys user)
        { 
            new XDocument(
                new XElement("RSA",
                    new XElement("userName", nazwaUzytkownikaTextBox.Text),
                    new XElement("publicKey",
                        new XElement("D", user.privateKey.D),
                        new XElement("DP", user.privateKey.DP),
                        new XElement("DQ", user.privateKey.DQ),
                        new XElement("Exponent", user.privateKey.Exponent),
                        new XElement("InverseQ", user.privateKey.InverseQ),
                        new XElement("Modulus", user.privateKey.Modulus),
                        new XElement("P", user.privateKey.P),
                        new XElement("Q", user.privateKey.Q))
            )
        )
        .Save(nazwaUzytkownikaTextBox.Text + ".private");

            //TO DO 
            // Funkcja skrotu i zaszyfrowanie pliku w trybie ECB
        }




        // Dziala przyda sie pozniej kodowanie dokodowanie RSA

        /* private byte[] encrypt(string message, userKeys user)
         {
             byte[] encrypted;
             using (var rsa = new RSACryptoServiceProvider(2048))
             {
                 rsa.ImportParameters(user.publicKey);
                 encrypted = rsa.Encrypt(Encoding.UTF8.GetBytes(message), true);
             }
             return encrypted;
         }

         private string decrypt(byte[] message, userKeys user)
         {
             byte[] decrypted;
             using (var rsa = new RSACryptoServiceProvider(2048))
             {
                 rsa.ImportParameters(user.privateKey);
                 decrypted = rsa.Decrypt(message, true);
             }
             return Encoding.UTF8.GetString(decrypted);
         }*/

        private void createUser(string name, String password)
        {
            userKeys user = new userKeys();

            user = generateKey();
            savePublicKey(user);
            savePrivateKey(user);
            /* String abc = "to tekst jawny";
             byte[] encrypted = encrypt(abc, user);
             MessageBox.Show(abc+" "+encrypted,
                    "Błędne hasło", MessageBoxButtons.OK, MessageBoxIcon.Error);
             string decrypted = decrypt(encrypted, user);
             MessageBox.Show(abc + " " + decrypted,
                    "Błędne hasło", MessageBoxButtons.OK, MessageBoxIcon.Error);*/
        }
    }
}
