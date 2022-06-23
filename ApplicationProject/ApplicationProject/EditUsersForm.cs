using ApplicationProject.DAL;
using Microsoft.EntityFrameworkCore;
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
    public partial class EditUsers : Form
    {
        private enum States 
        {
            NothingHappened,
            UserSelected,
            UserModified,
            DataInserted
        }

        private States _state = States.NothingHappened;
        private bool _userMode;
        private RentContext _context;

        public EditUsers(RentContext context, bool userMode = true)
        {
            InitializeComponent();
            _context = context;
            _userMode = userMode;
            UpdateData();
            if (!userMode)
            {
                panel1.Visible = false;
                textBoxId.Enabled = true;
            }
            else 
            {
                button1.Visible = false;
            }
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedIndex == -1)
            {
                if (_state == States.UserSelected || _state == States.UserModified)
                {
                    _state = States.NothingHappened;

                    textBoxId.Text = string.Empty;
                    textBoxName.Text = string.Empty;
                    textBoxAddress.Text = string.Empty;
                    textBoxBank.Text = string.Empty;
                    textBoxCharacteristic.Text = string.Empty;
                    textBoxHead.Text = string.Empty;

                    button1.Enabled = false;
                    button2.Enabled = false;
                    button3.Enabled = false;
                    button4.Enabled = false;
                }
            }
            else {
                _state = States.UserSelected;

                User selectedUser = listView.SelectedItem as User;

                textBoxId.Text = selectedUser.Username;
                textBoxName.Text = selectedUser.Name ?? string.Empty;
                textBoxAddress.Text = selectedUser.LegalAddress ?? string.Empty;
                textBoxBank.Text = selectedUser.BankName ?? string.Empty;
                textBoxCharacteristic.Text = selectedUser.Characteristic ?? string.Empty;
                textBoxHead.Text = selectedUser.Head ?? string.Empty;
                if (selectedUser.IsActive != null) {
                    if (selectedUser.IsActive == true)
                    {
                        button3.Text = "Remove access";
                    }
                    else 
                    {
                        button3.Text = "Return access";
                    }
                }
            }
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = true;
            button4.Enabled = true;
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            if (_state == States.NothingHappened)
            {
                _state = States.DataInserted;
                button1.Enabled = true;
            }
            else if (_state == States.UserSelected) 
            {
                _state = States.UserModified;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            User selectedUser = listView.SelectedItem as User;

            _context.ToggleAccess(selectedUser.Username);
            if (selectedUser.IsActive == true)
            {
                MessageBox.Show("Access removed");
                selectedUser.IsActive = false;
                button3.Text = "Return access";
            }
            else 
            {
                MessageBox.Show("Access returned");
                selectedUser.IsActive = true;
                button3.Text = "Remove access";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBoxId.Text.Any(c => !char.IsLetterOrDigit(c))) 
            {
                MessageBox.Show("Username can contain only digits or letters");
                return;
            }

            if (textBoxId.Text == string.Empty) 
            {
                MessageBox.Show("Specify username to create user");
                return;
            }

            EnterPasswordForm passwordForm = new EnterPasswordForm(textBoxId.Text, _context);
            passwordForm.ShowDialog();

            _state = States.NothingHappened;

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;

            UpdateData();
        }

        private void UpdateData() 
        {
            listView.SelectedIndex = -1;
            if (_userMode)
            {
                listView.Items.Clear();
                var users = _context.Tenants
                .AsNoTracking()
                .Select(t =>
                    new User
                    {
                        Id = t.Id,
                        Username = t.Username,
                        BankName = t.BankName,
                        Characteristic = t.Characteristic,
                        Name = t.Name,
                        Head = t.Head,
                        LegalAddress = t.LegalAddress,
                        IsActive = t.HasAccess
                    })
                .ToList();
                listView.Items.AddRange(users.ToArray() as object[]);
            }
            else 
            {
                listView.Items.Clear();
                var users = _context.Admins
                .AsNoTracking()
                .Select(a =>
                    new User
                    {
                        Id = a.Id,
                        Username = a.Username,
                    })
                .ToList();
                listView.Items.AddRange(users.ToArray() as object[]);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            User selectedUser = listView.SelectedItem as User;

            _context.DeleteUser(selectedUser.Username);

            MessageBox.Show("User with its data deleted");

            _state = States.NothingHappened;

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;

            UpdateData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            User selectedUser = listView.SelectedItem as User;

            var tenant = _context.Tenants.Find(selectedUser.Id);

            var textBoxes = panel1.Controls.OfType<TextBox>();

            foreach (var tb in textBoxes)
            {
                if (tb.Text == string.Empty)
                {
                    MessageBox.Show("You need to fill every field to continue");
                    return;
                }
            }

            tenant.Name = textBoxName.Text;
            tenant.BankName = textBoxBank.Text;
            tenant.Characteristic = textBoxCharacteristic.Text;
            tenant.Head = textBoxHead.Text;
            tenant.LegalAddress = textBoxAddress.Text;

            _context.SaveChanges();

            _state = States.NothingHappened;

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;

            UpdateData();
        }
    }

    public class User
    {
        private const string Admin = "Admin";
        public int Id { get; set; }
        public string Username { get; set; }
        public string? Name { get; set; }
        public string? LegalAddress { get; set; }
        public string? BankName { get; set; }
        public string? Head { get; set; }
        public string? Characteristic { get; set; }
        public bool? IsActive { get; set; }

        public override string ToString()
        {
            return $"Name: {Name ?? Admin} ({Id}) Username: {Username}";
        }
    }
}
