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
    public partial class RoomTypes : Form
    {
        private static RentContext _context;
        private bool _closedProperly = false;
        private DataGridViewCellCancelEventHandler _cellValidatingHandler;
        private object[] _defaultValues;
        public RoomTypes(RentContext context, Func<RentContext, IBindingList> getEntries, DataGridViewCellCancelEventHandler cellValidatingHandler, object[] defaultValues, bool readOnly = false, Action<DataGridView, RentContext> action = null)
        {
            InitializeComponent();
            if(readOnly) 
            {
                dataGridView1.ReadOnly = false;
                button1.Text = "Close";
            }
            ControlBox = false;
            _defaultValues = defaultValues;
            _cellValidatingHandler = cellValidatingHandler;
            dataGridView1.AutoSize = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            StartPosition = FormStartPosition.CenterScreen;
            _context = context;
            dataGridView1.DataSource = getEntries.Invoke(context);
            dataGridView1.Columns[0].ReadOnly = true;
            this.Load += (s, e) =>
            {
                action?.Invoke(dataGridView1, _context);
                dataGridView1.RowsAdded += dataGridView1_RowsAdded;
            };
            this.FormClosing += (s, e) =>
            {
                try
                {
                    _context.SaveChanges();
                }
                catch
                {
                    MessageBox.Show("Validation didnt pass, data wont be saved");
                    e.Cancel = false;
                }
            };
            dataGridView1.DataError += (s, e) =>
            {
                e.ThrowException = false;
                var data = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                MessageBox.Show($"Inserted data didnt pass type validation");
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _context.SaveChanges();
            this.Close();
        }

        private void dataGridView1_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dataGridView1.Rows.Count == 1) return;
            _cellValidatingHandler.Invoke(sender, e);
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (e.RowIndex == 0) return;
            var row = dataGridView1.Rows[e.RowIndex -1].Cells;

            for (int i = 0; i < row.Count; i++)
            {
                row[i].Value = _defaultValues[i];
            }
        }
    }
}
