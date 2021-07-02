using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Code_Plagiator_Detector
{
    class Scan
    {
        private List<string> files = new List<string>();
        private List<string> folder = new List<string>();
        private List<string> useless_files = new List<string>();
        string text_final;
        int index = 0;
        int[] plagiarism = new int[2] {0,0};
        bool[] sentence_plagiat;
        public Scan(List<string> files, List<string> folder)
        {            
            foreach (string file in files)
                if (file.Contains(".c") || file.Contains(".cpp"))
                    this.files.Add(file);
                else
                    useless_files.Add(file);
            ////////////////////////////////////////////////////////////////////
            foreach (string path in folder)        
                if (File.Exists(path))
                    ProcessFile(path);              
                else if (Directory.Exists(path))
                    ProcessDirectory(path);                
                else
                    Console.WriteLine("{0} is not a valid file or directory.", path);
            ////////////////////////////////////////////////////////////////////
        }
        public string Get_text()
        {
            return text_final;
        }
        public bool[] Get_bool()
        {
            return sentence_plagiat;
        }
        public void Forward()
        {
            if (index < files.Count-1) index++;
            this.Blind_Brutus();
        }
        public void Backward()
        {
            if (index > 0 ) index--;
            this.Blind_Brutus();
        }
        ~Scan()
        {
            files.Clear();
            folder.Clear();
            useless_files.Clear();
        }
        public void ProcessDirectory(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }
        public void ProcessFile(string path)
        {
            if (path.Contains(".c") || path.Contains(".cpp"))
                folder.Add(path);
        }
        public bool Check_if_exists()
        {
            if (files.Any() && folder.Any())
                return true;
            return false;
        }
        public void reset_plagiarism()
        {
            plagiarism[0] = 0;
            plagiarism[1] = 0;
        }
        public void Blind_Brutus()
        {
            this.reset_plagiarism();
            Code_Plagiator_Detector.Replace_Compare txt_file,txt_folder;
            string text_file = "";
            string text_folder = "";

            text_file = Text_Edit(files.ElementAt(index));
            txt_file = new Replace_Compare(text_file);
            sentence_plagiat = new bool[txt_file.Get_text().Split('\n').Length];
            Console.WriteLine("Textul dupa Text Edit \n"+text_file);

            int i, n = txt_file.Get_text().Split('\n').Length;
            for (i = 0; i < n; i++)
                sentence_plagiat[i] = false;

            foreach (string file_folder in folder)
            {
                text_folder = Text_Edit(file_folder);
                txt_folder  = new Replace_Compare(text_folder);
                string[] sentences = txt_file.Get_text().Split('\n');
                i = 0;
                foreach (string sentence in sentences)
                {
                    string[] sentences_folders = txt_folder.Get_text().Split('\n');
                    foreach (string sentence_folder in sentences_folders)
                    {
                        if (String.Compare(sentence, sentence_folder) ==0 && !sentence.Contains("{") && !sentence.Contains("}"))
                        {
                            sentence_plagiat[i] = true;
                        }
                    }
                    i++;
                }
            }
            plagiarism[0] = n;
            for (i = 0; i < n; i++)
                if(sentence_plagiat[i] == true) plagiarism[1]++;
            text_final = txt_file.Get_text();
            string txt = DateTime.Now.ToString() + "\n" + files.ElementAt(index) + "\n" + plagiarism[0] + "\n" + plagiarism[1];
            if (!File.Exists("hi_lendar.txt"))File.WriteAllText("hi_lendar.txt", txt + Environment.NewLine);
            else
            {
                File.AppendAllText(@"hi_lendar.txt", txt + Environment.NewLine);
            }
            Console.WriteLine("Textul final : \n"+text_final);
        }  
        public int[] Get_plagiarism()
        {
            return plagiarism;
        }
        public string Text_Edit(string file)
        {
            string text = System.IO.File.ReadAllText(@file);
            Code_Plagiator_Detector.Text_Edit txt = new Text_Edit();
            text = txt.Edit_Text(text);
            return text;
        }
        public void Junk(ListBox temp)
        {
            if (useless_files.Any())
            {
                string msg = "Useless files :\n";
                foreach (string file in useless_files)
                    msg += file + "\n";
                msg += "Do you want to delete them ?\n";
                DialogResult dialogResult = MessageBox.Show(msg, "Useless Files", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {                    
                    foreach (string to_delete in useless_files)
                    {
                        for (int n = temp.Items.Count - 1; n >= 0; --n)
                        {
                            string removelistitem = to_delete;
                            if (temp.Items[n].ToString().Contains(removelistitem))
                            {
                                temp.Items.RemoveAt(n);
                            }
                        }
                    }                    
                }
            }
        }
    }
}
