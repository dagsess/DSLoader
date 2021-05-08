using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSLoader
{
    enum ObjectType { M, NGC, IC, USER }
    public partial class Form1 : Form
    {
        SerialInterface serialInterface;

        DataTable tableObjects = new DataTable();

        BindingSource bsObjects = new BindingSource();

        public Form1()
        {
            InitializeComponent();

            foreach (var portName in SerialPort.GetPortNames())
            {
                cbSerialPortNames.Items.Add(portName);
            }

            tableObjects.Columns.Add("order", typeof(int));
            //tableObjects.Columns[0].Unique = true;
            tableObjects.Columns.Add("objectType", typeof(ObjectType));
            tableObjects.Columns.Add("objectName", typeof(string));
            tableObjects.Columns.Add("raHour", typeof(int));
            tableObjects.Columns.Add("raMinute", typeof(double));

            bsObjects.DataSource = tableObjects;

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = bsObjects;
            dataGridView1.Columns[0].HeaderText = "#";
            dataGridView1.Columns[1].HeaderText = "Тип объекта";
            dataGridView1.Columns[2].HeaderText = "Имя/Номер";
            dataGridView1.Columns[3].HeaderText = "RA | часы";
            dataGridView1.Columns[3].DefaultCellStyle.Format = "d2";
            dataGridView1.Columns[4].HeaderText = "RA | минуты";
            dataGridView1.Columns[4].DefaultCellStyle.Format = "f1";

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
            dataGridView1.Columns[0].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;

            tableObjects.TableNewRow += TableObjects_TableNewRow;
        }

        private void TableObjects_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            e.Row["order"] = tableObjects.Rows.Count == 0? 0: tableObjects.AsEnumerable().Max(r=>(int)r["order"]) + 1;
        }

       
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            if(int.TryParse(comboBox2.Text, out int baudRate))
            {
                toolStripStatusLabel1.Text = $"Попытка открыть порт {cbSerialPortNames.Text}...";
                serialInterface = new SerialInterface(cbSerialPortNames.Text, baudRate);
                var result = await serialInterface.OpenInterface();

                if(result)
                {
                    toolStripStatusLabel1.Text = $"Устройство готово к принимать данные!";
                    button2.Enabled = true;
                }
                else
                {
                    toolStripStatusLabel1.Text = $"Ошибка при попытке открыть порт {cbSerialPortNames.Text}";
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var currentRow = (bsObjects.Current as DataRowView).Row;

            int oldOrder = (int)currentRow["order"];
            int newOrder = oldOrder - 1;

            int maxOrder = tableObjects.AsEnumerable().Max(r => (int)r["order"]);

            if (newOrder >= 0)
            {
                var rows = tableObjects.AsEnumerable().Where(r => (int)r["order"] == newOrder).ToArray();

                foreach (var r in rows)
                {
                    r["order"] = oldOrder;
                }
                currentRow["order"] = newOrder;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var currentRow = (bsObjects.Current as DataRowView).Row;

            int oldOrder = (int)currentRow["order"];
            int newOrder = oldOrder + 1;
            int maxOrder = tableObjects.AsEnumerable().Max(r => (int)r["order"]);


            if (newOrder <= maxOrder)
            {
                var rows = tableObjects.AsEnumerable().Where(r => (int)r["order"] == newOrder).ToArray();

                foreach (var r in rows)
                {
                    r["order"] = oldOrder;
                }
                currentRow["order"] = newOrder;
            }
        }
    }
}
