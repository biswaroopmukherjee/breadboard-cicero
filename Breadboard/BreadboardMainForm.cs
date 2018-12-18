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

            System.Diagnostics.Trace.WriteLine(fileNames[0]);

            if (result == DialogResult.OK)
            {
                loadAndWrite(fileNames, "Q:\\roop\\crunls\\Snippets");
            }
        }


        public void loadAndWrite(string[] fileNames, string destination)
        /// Load Runlog files
        {
            foreach (string fileName in fileNames)
            {
                try
                {
                    RunLog rlg = Common.loadBinaryObjectFromFile(fileName) as RunLog;
                    Console.WriteLine(rlg.RunTime);
                    writeVariablesToFile(rlg, destination);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to open or read file " + fileName + " due to exception: " + ex.Message + ex.StackTrace);
                }
            }
        }

        public bool writeVariablesToFile(RunLog rlg, string path)
        /// write Variable List to File
        {

            DateTime runEndTime = rlg.RunTime.AddSeconds(rlg.RunSequence.SequenceDuration);
            string filename = runEndTime.ToString("yyyy-MM-dd");
            string varFullFileName = path + "\\" + filename + ".txt";

            if (path == "")
            {
                return false;
            }

            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(varFullFileName))
                {
                    file.Write(runEndTime.ToString("MM-dd-yyyy_HH_mm_ss\t"));
                    file.Write("SequenceDuration" + ";"+ rlg.RunSequence.SequenceDuration + ",");
                    file.Write("SequenceModeName" + ";" + rlg.RunSequence.CurrentMode.ModeName + ",");
                    foreach (Variable var in rlg.RunSequence.Variables)
                    {
                        file.Write(var.VariableName + ';' + Convert.ToString(var.VariableValue) + ",");
                    }
                    file.WriteLine("");
                    file.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to open or read file " + filename+" due to exception: " + ex.Message + ex.StackTrace);
                return false;
            }


        }
    }
}