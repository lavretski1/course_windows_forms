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
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            checkBox1.Text = "I agree to the terms that you can take my \r\nkidney and sell my soul to the devil";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var textboxes = this.Controls.OfType<TextBox>();

            if (checkBox1.Checked == false) 
            {
                MessageBox.Show("You cannot use the app if you dont agree with terms and conditions");
                return;
            }


            foreach (var tb in textboxes) 
            {
                if (tb.Text == string.Empty) 
                {
                    MessageBox.Show("You need to fill every field to continue");
                    return;
                }
            }

            if (textBox1.Text.FirstOrDefault(c => !char.IsLetterOrDigit(c)) != default)
            {
                MessageBox.Show("Your username can only contain letters and digits");
                return;
            }

            if (textBox5.Text.FirstOrDefault(c => !char.IsLetterOrDigit(c)) != default)
            {
                MessageBox.Show("Your password can only contain letters and digits");
                return;
            }

            string result = RentContext.Register(textBox1.Text, textBox5.Text, textBox2.Text, textBox6.Text, textBox3.Text, textBox7.Text, textBox4.Text);

            MessageBox.Show(result);
            if (result == "Success")
            {
                this.Close();
            }
        }
    }
}
