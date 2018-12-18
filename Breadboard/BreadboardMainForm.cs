using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataStructures;


namespace Breadboard
{
    public partial class BreadboardMainForm : Form
    {
        public BreadboardMainForm()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            Console.WriteLine("S");
            fileDialog.Title = "Open RunLog File(s)";
            fileDialog.Filter = "RunLog files (*.clg)|*.clg|All files (*.*)|*.*";
            fileDialog.FilterIndex = 1;
            fileDialog.Multiselect = true;

            DialogResult result = fileDialog.ShowDialog();
            string[] fileNames = fileDialog.FileNames;

            /// MessageBox.Show("Thanks!");
            System.Diagnostics.Trace.WriteLine(fileNames[0]);

            if (result == DialogResult.OK)
            {
                loadFiles(fileNames);
            }
        }


        public void loadFiles(string[] fileNames)
        /// Load Runlog files
        {
            foreach (string fileName in fileNames)
            {
                try
                {
                    RunLog rlg = Common.loadBinaryObjectFromFile(fileName) as RunLog;
                    System.Diagnostics.Trace.WriteLine(rlg.RunTime);

                    writeRunLogVariablesToFile(rlg, "poo", "Q:\\roop\\crunls\\Snippets");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to open or read file " + fileName + " due to exception: " + ex.Message + ex.StackTrace);
                }
            }
        }

        public bool writeRunLogVariablesToFile(RunLog rlg, string filename, string path)
        /// write Variable List to File
        {
            bool writeSuccess = true;
            string varFileDirectory = path;
            string varFileExt = ".txt";
            string varFullFileName = varFileDirectory + "\\" + filename + varFileExt;

            if (path == "" || filename == "")
            {
                return false;
            }


            using (System.IO.StreamWriter file = new System.IO.StreamWriter(varFullFileName))
            {
                file.WriteLine(DateTime.Now);
                foreach (Variable var in rlg.RunSequence.Variables)
                {
                    file.WriteLine(var.VariableName + ' ' + Convert.ToString(var.VariableValue));
                }
                file.Close();
            }

            return true;

        }
    }
}