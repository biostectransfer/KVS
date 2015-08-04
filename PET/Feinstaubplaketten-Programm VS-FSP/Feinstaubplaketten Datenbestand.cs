using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace PET
{
    public partial class Feinstaubplaketten_Datenbestand : Form
    {
        private SqlDataAdapter da;
        private SqlConnection conn;
        BindingSource bsource = new BindingSource();
        DataSet ds = null;
        public Feinstaubplaketten_Datenbestand()
        {
            InitializeComponent();
        }

        private void Feinstaubplaketten_Datenbestand_Load(object sender, EventArgs e)
        {
            string rowValue;
            string[] cellValue;
            string CSVPath;
            if (String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["DatatablePath"]))
            {
                CSVPath = Application.StartupPath + @"\LogDatenbank.csv";
            }
            else
            {
                CSVPath = System.Configuration.ConfigurationManager.AppSettings["DatatablePath"];
            }
            if (System.IO.File.Exists(CSVPath))
            {
                try
                {
                    System.IO.StreamReader streamReader = new StreamReader(CSVPath, Encoding.Default);

                    rowValue = streamReader.ReadLine();
                    cellValue = rowValue.Split(';');
                    for (int i = 0; i <= cellValue.Count() - 1; i++)
                    {
                        DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                        column.Name = cellValue[i];
                        column.HeaderText = cellValue[i];
                        dgv.Columns.Add(column);
                    }
                    while (streamReader.Peek() != -1)
                    {
                        rowValue = streamReader.ReadLine();
                        cellValue = rowValue.Split(';');
                        dgv.Rows.Add(cellValue);
                    }

                    streamReader.Close();
                }
                catch
                {
                    MessageBox.Show("Die Datei wird gerade verwendet und/oder ist gesperrt");
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Datei existiert nicht");
            }

            dgv.AutoResizeColumns();
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //Die Verbindung zur DB wird hergestellt und die Daten werden in das GridView geladen END------------
        }
    }
}