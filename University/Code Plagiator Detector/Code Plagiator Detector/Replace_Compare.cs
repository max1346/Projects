using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Code_Plagiator_Detector
{
    class Replace_Compare
    {
        int[] vars = new int[5];
        string final_text;
        public Replace_Compare(string text)
        {
            text = this.Replace_Vars(text, "int");
            text = this.Replace_Vars(text, "float");
            text = this.Replace_Vars(text, "char");
            text = this.Replace_Vars(text, "bool");
            text = this.Replace_Vars(text, "double");
            final_text = text;
        }
        public string Get_text()
        {
            return final_text;
        }
        public string Replace_Vars(string text,string data_type_var)
        {
            List<string> list = new List<string>();
            string[] sentences = text.Split('\n');
            foreach (string sentence in sentences)
            {
                if (sentence.Contains(data_type_var) && !sentence.Contains("("))
                {
                    string[] words = sentence.Split(' ',',');
                    foreach(string word in words)
                    {
                        if (!word.Contains(data_type_var) && !word.Contains('\n'))
                        {
                            string txt = Regex.Replace(word, @"(\s+|@|&|'|\(|\)|<|>|#|;)", "");
                            if (txt.Contains("="))
                            {
                                string word_with_value = "";
                                for (int j = 0; j < word.Length; j++)
                                    if (word[j] != '=') word_with_value += word[j];
                                    else break;                             
                                list.Add(word_with_value);
                            }
                            else list.Add(txt);
                        }
                    }
                }
            }
            int i = 0;
            foreach (string elem in list)
            {
                text = text.Replace(" " + elem + ";", " var_" + data_type_var + "_" + (i + 1).ToString() + ";");  //vars with no value
                text = text.Replace(" " + elem + ",", " var_" + data_type_var + "_" + (i + 1).ToString() + ",");
                text = text.Replace("," + elem + ",", ",var_" + data_type_var + "_" + (i + 1).ToString() + ",");
                text = text.Replace("," + elem + ";", ",var_" + data_type_var + "_" + (i + 1).ToString() + ";");

                text = text.Replace(" " + elem + "=", " var_" + data_type_var + "_" + (i + 1).ToString() + "=");  //vars with value
                text = text.Replace("," + elem + "=", ",var_" + data_type_var + "_" + (i + 1).ToString() + "=");

                text = text.Replace("(" + elem + ")", "(var_" + data_type_var + "_" + (i + 1).ToString() + ")");    //vars in if

                text = text.Replace("<" + elem + ";", "<var_" + data_type_var + "_" + (i + 1).ToString() + ";");    //vars in loop
                i++;
            }
            if (String.Compare(data_type_var, "int") == 0) vars[0] = list.Count;
            if (String.Compare(data_type_var, "float") == 0) vars[1] = list.Count;
            if (String.Compare(data_type_var, "char") == 0) vars[2] = list.Count;
            if (String.Compare(data_type_var, "bool") == 0) vars[3] = list.Count;
            if (String.Compare(data_type_var, "double") == 0) vars[4] = list.Count;
            return text;
        }
    }
}
