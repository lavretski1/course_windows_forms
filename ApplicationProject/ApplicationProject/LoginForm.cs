using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApplicationProject
{
    public partial class LoginForm : Form
    {
        public bool Register { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public LoginForm()
        {
            InitializeComponent();
            label3.ForeColor = Color.OrangeRed;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox2.Text;
            string password = textBox1.Text;

            if (username == string.Empty || password == string.Empty) 
            {
                label3.Text = "Please enter username and password";
                return;
            }

            Username = username;
            Password = password;

            this.Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label3.Text = string.Empty;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label3.Text = string.Empty;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Register = true;

            this.Close();
        }
    }
}
