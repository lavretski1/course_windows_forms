using ApplicationProject.DAL;
using ApplicationProject.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace ApplicationProject
{
    public partial class MainWindow : Form
    {
        private RentContext _context;

        public MainWindow(RentContext context, bool forAdmin)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            _context = context;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            if (!forAdmin)
            {
                var columns = dataGridView1.Columns;
                foreach (DataGridViewColumn col in columns)
                {
                    col.ReadOnly = true;
                }
                dataGridView1.DataSource = _context.Rooms
                    .Where(r => r.Active == true)
                    .ToList();
                panel2.Visible = false;
                dataGridView1.Columns[5].Visible = false;
                comboBox1.Enabled = false;
                dataGridView1.ReadOnly = true;
            }
            else
            {
                _context.Rooms.Load();
                dataGridView1.DataSource = _context.Rooms.Local.ToBindingList();
                button6.Visible = false;
            }
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.SelectionChanged += (s, e) =>
            {
                var cells = dataGridView1.SelectedCells;

                if (cells.Count == 0) 
                {
                    comboBox1.SelectedIndex = -1;
                    return;
                }

                var room = cells[0].OwningRow.DataBoundItem as Room;

                if (room == null) 
                {
                    comboBox1.SelectedIndex = -1;
                    return;
                }

                List<SelectibleRoomtype> roomtypes = _context.RoomTypes
                    .AsNoTracking()
                    .Select(rt => new SelectibleRoomtype { Id = rt.Id, Name = rt.Name, Pricing = rt.Pricing })
                    .ToList();

                comboBox1.Items.Clear();

                comboBox1.Items.AddRange(roomtypes.ToArray());

                int index;

                var roomObject = _context.Rooms
                    .AsNoTracking()
                    .Where(r => r.Id == room.Id)
                    .Include(r => r.Type)
                    .FirstOrDefault();
                if (roomObject == null)
                {
                    index = -1;
                }
                else 
                {
                    index = roomtypes.FindIndex(rt => rt.Id == roomObject.Type.Id);
                }

                comboBox1.SelectedIndex = index;
            };
            comboBox1.SelectedIndexChanged += (s, e) =>
            {
                if (comboBox1.SelectedIndex == -1) return;

                var cells = dataGridView1.SelectedCells;

                if (cells.Count == 0)
                {
                    comboBox1.SelectedIndex = -1;
                    return;
                }

                var room = cells[0].OwningRow.DataBoundItem as Room;

                if (room is null) 
                {
                    return;
                }

                room.Type = comboBox1.SelectedItem as RoomType;

                label3.Text = room.Type.Pricing.ToString();
            };
            this.FormClosing += (s, e) =>
            {
                if (forAdmin)
                {
                    try
                    {
                        _context.SaveChanges();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Validation didnt pass, data wont be saved");
                        e.Cancel = false;
                    }
                }
            };
            dataGridView1.DataError += (s, e) =>
            {
                e.ThrowException = false;
                var data = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                MessageBox.Show($"Inserted data didnt pass type validation");
            };
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EditUsers editor = new EditUsers( _context);
            editor.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EditUsers editor = new EditUsers(_context,false);
            editor.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new RoomTypes(_context, (c) => {
                    c.RoomTypes.Load();
                    return c.RoomTypes.Local.ToBindingList();
                },
                (sender, e) => {
                    var row = (sender as DataGridView).Rows[e.RowIndex];
                
                    if ((row.Cells[1].Value as string) == string.Empty) 
                    {
                        MessageBox.Show("Room type name shouldnt be empty");
                        e.Cancel = true;
                    }

                    float price = (float)row.Cells[2].Value;
                    if (price <= 0) 
                    {
                        MessageBox.Show("Price should be more than 0");
                        e.Cancel = true;
                    }
                },
                new object[] { 0, "New Type", 10 }
            ).ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dataGridView1.Rows.Count == 1) return;
            var row = dataGridView1.Rows[e.RowIndex];

            int number = (int)row.Cells[1].Value;
            if (number < 0)
            {
                MessageBox.Show("Room number should be positive");
                e.Cancel = true;
            }

            float area = (float)row.Cells[2].Value;
            if (area <= 0)
            {
                MessageBox.Show("Area should be more than 0");
                e.Cancel = true;
            }

            if ((row.Cells[3].Value as string) == string.Empty)
            {
                MessageBox.Show("Room address shouldnt be empty");
                e.Cancel = true;
            }
        }


        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (e.RowIndex == 0) return;
            var row = dataGridView1.Rows[e.RowIndex - 1];

            var room = row.DataBoundItem as Room;

            room.Type = _context.RoomTypes.FirstOrDefault();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var room = dataGridView1.SelectedRows[0].DataBoundItem as Room;
        }
    }
    class SelectibleTenant : Tenant 
    {
        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }

    class SelectibleRoom : Room
    {
        public override string ToString()
        {
            return $"{Number} ({Id})";
        }
    }

    class SelectibleRoomtype : RoomType 
    {
        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}