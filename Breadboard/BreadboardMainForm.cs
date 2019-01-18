using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Permissions;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DataStructures;


namespace Breadboard
{


    public partial class BreadboardMainForm : Form
    {
        public static Dictionary<string, string> BreadboardSettings = new Dictionary<string, string>();
        public static FileSystemWatcher watcher = new FileSystemWatcher();
        public BreadboardMainForm()
        {
            InitializeComponent();

            string settingsFileName = "./BreadboardSettings.set";
            //addEventLogText("Settings loaded");
            if (File.Exists(settingsFileName))
            {
                BreadboardSettings = ReadDictionary(settingsFileName);
                if (BreadboardSettings.ContainsKey("SnippetFolder"))
                {
                    textBox1.Text = BreadboardSettings["SnippetFolder"];
                }
                if (BreadboardSettings.ContainsKey("RunLogFolder"))
                {
                    textBox2.Text = BreadboardSettings["RunLogFolder"];
                }
                //MessageBox.Show("Settings were loaded from the file 'BreadboardSettings.set' in the directory with the Breadboard executable.");
            }
            else
            {
                MessageBox.Show("Settings were not automatically loaded. Save new settings to enable automatic loading of settings.");
            }




        }

        // Define the event handlers.
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            addEventLogText("Reading file... ");
            while (IsFileLocked(e.FullPath)) { }// Do nothing while file is being written by cicero
            Debug.WriteLine("File is probably unlocked");
            // If the file is ready to read,
            if (!IsFileLocked(e.FullPath))
            {
                Debug.WriteLine("File is defo unlocked");
                // Specify what is done when a file is changed, created, or deleted.
                Debug.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
                RunLog rlg = Common.loadBinaryObjectFromFile(e.FullPath) as RunLog;
                if (BreadboardSettings.ContainsKey("SnippetFolder"))
                {
                    string destination = BreadboardSettings["SnippetFolder"];
                    writeVariablesToFile(rlg, destination);
                }
                else
                {
                    MessageBox.Show("Please enter a Snippet Folder");
                }
            }

        }

        // Manually read runlogs
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            Debug.WriteLine("S");
            fileDialog.Title = "Open RunLog File(s)";
            fileDialog.Filter = "RunLog files (*.clg)|*.clg|All files (*.*)|*.*";
            fileDialog.FilterIndex = 1;
            fileDialog.Multiselect = true;

            DialogResult result = fileDialog.ShowDialog();
            string[] fileNames = fileDialog.FileNames;


            if (result == DialogResult.OK)
            {
                loadAndWrite(fileNames, "Q:\\roop\\crunls\\Snippets");
                System.Diagnostics.Trace.WriteLine(fileNames[0]);
            }
            else
            {
                addEventLogText("No runlogs were selected");
            }
        }

        // load files from filenames, and write snippets to destination
        private void loadAndWrite(string[] fileNames, string destination)
        /// Load Runlog files
        {
            foreach (string fileName in fileNames)
            {
                try
                {
                    RunLog rlg = Common.loadBinaryObjectFromFile(fileName) as RunLog;
                    Debug.WriteLine(rlg.RunTime);
                    writeVariablesToFile(rlg, destination);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to open or read file " + fileName + " due to exception: " + ex.Message + ex.StackTrace);
                }
            }
        }

        // Write Runlog to file
        private bool writeVariablesToFile(RunLog rlg, string path)
        /// write Variable List to File
        {

            DateTime runEndTime = rlg.RunTime.AddSeconds(rlg.RunSequence.SequenceDuration);
            string filename = runEndTime.ToString("yyyy-MM-dd");
            Debug.WriteLine(filename);
            string varFullFileName = path + "\\" + filename + ".txt";

            if (path == "")
            {
                return false;
            }

            try //writing to file
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(varFullFileName, append:true))
                {
                    file.Write(runEndTime.ToString("MM-dd-yyyy_HH_mm_ss\t"));
                    file.Write("SequenceDuration" + ";"+ rlg.RunSequence.SequenceDuration + ",");
                    try { file.Write("SequenceModeName" + ";" + rlg.RunSequence.CurrentMode.ModeName + ","); }
                    catch { }
                    foreach (Variable var in rlg.RunSequence.Variables)
                    {
                        file.Write(var.VariableName + ';' + Convert.ToString(var.VariableValue) + ",");
                    }
                    file.WriteLine("");
                    file.Close();
                }

                addEventLogText("New snippet: " + runEndTime.ToString("MM-dd-yyyy_HH_mm_ss\t"));
                return true;
            }
            catch (Exception ex)
            {
                addEventLogText("ERROR:" + "Unable to open or read file " + filename + " due to exception: " + ex.Message + ex.StackTrace);
                Debug.WriteLine("Unable to open or read file " + filename+" due to exception: " + ex.Message + ex.StackTrace);
                return false;
            }


        }

        private void BreadboardMainForm_Load(object sender, EventArgs e)
        {
            addEventLogText("Welcome to Breadboard.");
        }

        private void rlgfolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
                BreadboardSettings["RunLogFolder"] = folderBrowserDialog1.SelectedPath;
            }
        }

        private void snippetfolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                BreadboardSettings["SnippetFolder"] = folderBrowserDialog1.SelectedPath;
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            BreadboardSettings["RunLogFolder"] = textBox2.Text;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            BreadboardSettings["SnippetFolder"] = textBox1.Text;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            string fileName = "./BreadboardSettings.set";
            BreadboardSettings["SnippetFolder"] = textBox1.Text;
            BreadboardSettings["RunLogFolder"] = textBox2.Text;
            WriteDictionary(BreadboardSettings, fileName);
            addEventLogText("Settings saved.");
            MessageBox.Show("Settings saved as 'BreadboardSettings.set'.");
        }

        static void WriteDictionary(Dictionary<string, string> dictionary, string file)
        {
            using (FileStream fs = File.OpenWrite(file))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                // Put count.
                writer.Write(dictionary.Count);
                // Write pairs.
                foreach (var pair in dictionary)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }
            }
        }

        static Dictionary<string, string> ReadDictionary(string file)
        {
            var result = new Dictionary<string, string>();
            using (FileStream fs = File.OpenRead(file))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // Get count.
                int count = reader.ReadInt32();
                // Read in all pairs.
                for (int i = 0; i < count; i++)
                {
                    string key = reader.ReadString();
                    string value = reader.ReadString();
                    result[key] = value;
                }
            }
            return result;
        }

        // START button
        private void button4_Click(object sender, EventArgs e)
        {
            
            if (BreadboardSettings.ContainsKey("RunLogFolder") && BreadboardSettings.ContainsKey("SnippetFolder") && Directory.Exists(BreadboardSettings["RunLogFolder"]) && Directory.Exists(BreadboardSettings["SnippetFolder"]))
            {
                
                addEventLogText("Starting up");
                label5.Visible = false;
                label4.Visible = true;
                // Create a new FileSystemWatcher and set its properties.
                watcher.Path = BreadboardSettings["RunLogFolder"];
                /* Watch for changes in LastAccess and LastWrite times, and
                   the renaming of files or directories. */
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                // Only watch Runlog files.
                watcher.Filter = "*.clg";

                // Add event handlers.
                watcher.Created += new FileSystemEventHandler(OnCreated);

                // Begin watching.
                watcher.EnableRaisingEvents = true;
                addEventLogText("Running");
            }
            else
            {
                MessageBox.Show("Check your folders!");
                addEventLogText("ERROR: folders invalid.");
            }
            

        }

        // Check if runfile is locked
        private bool IsFileLocked(string fileName)
        {
            FileStream stream = null;
            FileInfo file = new FileInfo(fileName);
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }


        // Add message to eventlog
        public void addEventLogText(string messagetext)
        {
            eventLogTextBox.Invoke((Action)delegate
            {
                eventLogTextBox.AppendText(DateTime.Now.ToString() + " " + messagetext + "\r\n"); ;
            });
        }


    }
}