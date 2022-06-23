using ApplicationProject.DAL;
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
    public partial class EnterPasswordForm : Form
    {
        private readonly string _username;
        private readonly RentContext _context;

        public EnterPasswordForm(string username, RentContext context)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            _username = username;
            _context = context;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Any(c => !char.IsLetterOrDigit(c)))
            {
                MessageBox.Show("Username can contain only digits or letters");
                return;
            }

            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show("Specify username to create user");
                return;
            }

            _context.CreateAdmin(_username, textBox1.Text);

            MessageBox.Show("Admin added");

            this.Close();
        }
    }
}
