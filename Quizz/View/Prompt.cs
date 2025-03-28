using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Quizz.Controller;

namespace Quizz
{
    public partial class Prompt : Form
    {
        private Label lblPrompt;
        private TextBox txtInput;

        public string InputText => txtInput.Text;  // Ajout de la propriété

        public Prompt(string text, string caption)
        {
            InitializeComponent();
            lblPrompt.Text = text;
            this.Text = caption;
            ShowDialog();
        }

        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 200,
                Text = caption
            };

            Label lbl = new Label() { Left = 20, Top = 20, Text = text };
            TextBox inputBox = new TextBox() { Left = 20, Top = 50, Width = 350 };
            Button confirmButton = new Button() { Text = "OK", Left = 280, Top = 100, Width = 80 };

            confirmButton.Click += (sender, e) => { prompt.DialogResult = DialogResult.OK; prompt.Close(); };

            prompt.Controls.Add(lbl);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmButton);
            prompt.StartPosition = FormStartPosition.CenterParent;

            return (prompt.ShowDialog() == DialogResult.OK) ? inputBox.Text : string.Empty;
        }
    }
}

