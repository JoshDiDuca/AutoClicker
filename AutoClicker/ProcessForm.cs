using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoClicker
{
    public partial class ProcessForm : Form
    {
        public String SelectedProcess { get; set; }
        public List<String> ProcessList = new List<String>();
        public ProcessForm()
        {
            InitializeComponent();
        }

        private void GetProcessList()
        {
            Process[] localAll = Process.GetProcesses();
            foreach (var item in localAll)
            {
                ProcessList.Add(item.ProcessName);
            }
            dataGridView1.DataSource = ProcessList;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectedProcess = dataGridView1.SelectedRows[0].Cells[0].ToString();
            this.Close();
        }

        private void ProcessForm_Load(object sender, EventArgs e)
        {
            GetProcessList();
        }
    }
}
