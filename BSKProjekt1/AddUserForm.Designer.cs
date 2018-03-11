namespace BSKProject1
{
    partial class AddUserForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.nazwaUzytkownikaTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.hasloUzytkownikaTextBox = new System.Windows.Forms.TextBox();
            this.dodajUzytkownikaButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(112, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nazwa";
            // 
            // nazwaUzytkownikaTextBox
            // 
            this.nazwaUzytkownikaTextBox.Location = new System.Drawing.Point(44, 56);
            this.nazwaUzytkownikaTextBox.Name = "nazwaUzytkownikaTextBox";
            this.nazwaUzytkownikaTextBox.Size = new System.Drawing.Size(189, 22);
            this.nazwaUzytkownikaTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(113, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Hasło";
            // 
            // hasloUzytkownikaTextBox
            // 
            this.hasloUzytkownikaTextBox.Location = new System.Drawing.Point(44, 121);
            this.hasloUzytkownikaTextBox.Name = "hasloUzytkownikaTextBox";
            this.hasloUzytkownikaTextBox.PasswordChar = '*';
            this.hasloUzytkownikaTextBox.Size = new System.Drawing.Size(189, 22);
            this.hasloUzytkownikaTextBox.TabIndex = 3;
            // 
            // dodajUzytkownikaButton
            // 
            this.dodajUzytkownikaButton.Location = new System.Drawing.Point(97, 172);
            this.dodajUzytkownikaButton.Name = "dodajUzytkownikaButton";
            this.dodajUzytkownikaButton.Size = new System.Drawing.Size(75, 31);
            this.dodajUzytkownikaButton.TabIndex = 4;
            this.dodajUzytkownikaButton.Text = "Dodaj";
            this.dodajUzytkownikaButton.UseVisualStyleBackColor = true;
            this.dodajUzytkownikaButton.Click += new System.EventHandler(this.dodajUzytkownikaButton_Click);
            // 
            // AddUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 222);
            this.Controls.Add(this.dodajUzytkownikaButton);
            this.Controls.Add(this.hasloUzytkownikaTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nazwaUzytkownikaTextBox);
            this.Controls.Add(this.label1);
            this.Name = "AddUserForm";
            this.Text = "AddUserForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox nazwaUzytkownikaTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox hasloUzytkownikaTextBox;
        private System.Windows.Forms.Button dodajUzytkownikaButton;
    }
}