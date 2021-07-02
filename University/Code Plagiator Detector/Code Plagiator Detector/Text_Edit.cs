using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Code_Plagiator_Detector
{
    class Text_Edit
    {
        public string Edit_Text(string text)
        {
            text = this.Space_Edit(text);
            text = this.Replace_To_New_Line(text);
            text = this.Delete_Bracket_Spaces(text);
            text = this.Delete_Empty_Lines(text);
            text = this.Add_Brackets(text);          
            text = this.Delete_Empty_Lines(text);
            text = this.Leveling_Tabs(text);
            return text;
        }
        public string Space_Edit(string txt)            //Deleting spaces that are in excess
        {
            Regex regex = new Regex("[ ]{2,}", RegexOptions.None);
            txt = regex.Replace(txt, " ");
            txt = this.Delete_Braket_Spaces(txt);
            txt = this.Delete_Tabs(txt);
            txt = this.Delete_Dot_Spaces(txt);
            txt = this.Delete_Bracket_Spaces(txt);
            return txt;
        }
        public string Delete_Empty_Lines(string txt)    //Delete empty rows
        {
            return Regex.Replace(txt, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
        }
        public string Replace_To_New_Line(string txt)   //
        {
            bool inbraket = false;
            string txt2 = "";
            for (int i = 0; i < txt.Length; i++)
            {
                if (txt[i] == '(')
                {
                    txt2 += txt[i];
                    inbraket = true;
                }
                else if (txt[i] == ')')
                {
                    txt2 += txt[i];
                    inbraket = false;
                }
                else
                {
                    if (!inbraket && txt[i] == ';')
                        txt2 += ";\n";
                    else
                        txt2 += txt[i];
                }
            }
            txt = Regex.Replace(txt2, @"; ", ";");
            return Regex.Replace(txt, @" ;", ";");
        }
        public string Delete_Tabs(string txt)
        {
            return Regex.Replace(txt, @"\t", "");
        }
        public string Delete_Braket_Spaces(string txt)
        {
            txt = Regex.Replace(txt, @" \(", "(");
            txt = Regex.Replace(txt, @"\( ", "(");
            txt = Regex.Replace(txt, @" \)", ")");
            txt = Regex.Replace(txt, @"\) ", ")");
            return txt;
        }
        public string Delete_Dot_Spaces(string txt)
        {
            txt = Regex.Replace(txt, @" ,", ",");
            return Regex.Replace(txt, @", ", ",");
        }
        public string Leveling_Tabs(string txt)
        {
            int tab_lvl = 0;
            string txt2 = "";
            for (int i = 0; i < txt.Length; i++)
            {
                if (txt[i] == '{')
                    tab_lvl++;
                else if (txt[i] == '}')
                {
                    tab_lvl--;
                    txt2 = txt2.Remove(txt2.Length - 1);
                }
                else if (txt[i] == '\n')
                {
                    txt2 += "\n";
                    for (int j = 0; j < tab_lvl; j++)
                        txt2 += "\t";
                    continue;
                }
                txt2 += txt[i];
            }
            return txt2;
        }
        public string Delete_Bracket_Spaces(string txt)
        {
            txt = Regex.Replace(txt, @" \{", "{");
            txt = Regex.Replace(txt, @"\{ ", "{");
            txt = Regex.Replace(txt, @" \}", "}");
            txt = Regex.Replace(txt, @"\} ", "}");
            return txt;
        }
        public string Add_Brackets(string txt)
        {
            string txt2 = Regex.Replace(txt, @"\)\r\n", ")");
            int count = 0;
            txt = txt2;
            for (int i=1;i<txt.Length;i++)
            {
                if(txt[i]!=';' && txt[i]!='\n' && txt[i-1]==')' && txt[i]!='\r' && txt[i]!='{')
                {
                    txt2 = txt2.Insert(i+count, "{");
                    count++;
                    for (int j = i + 1; j < txt.Length; j++)
                        if (txt[j] ==';')
                        {
                            txt2 = txt2.Insert(j + count+1, "}");
                            count++;
                            break;
                        }
                }
            }
            count = 0;
            txt = txt2;
            for (int i=1;i<txt.Length;i++)
            {
                if(txt[i]=='{' && txt[i-1]!='\n')
                {
                    txt2=txt2.Insert(i+count, "\n");
                    count++;
                }
            }
            count = 0;
            txt = txt2;
            for (int i = 1; i < txt.Length; i++)
            {
                if (txt[i] != '\n' && txt[i - 1] == '{')
                {
                    txt2 = txt2.Insert(i + count, "\n");
                    count++;
                }
            }
            count = 0;
            txt = txt2;
            for (int i = 1; i < txt.Length; i++)
            {
                if (txt[i] == '}' && txt[i - 1] != '\n')
                {
                    txt2 = txt2.Insert(i + count, "\n");
                    count++;
                }
            }
            count = 0;
            txt = txt2;
            for (int i = 1; i < txt.Length; i++)
            {
                if (txt[i] != '\n' && txt[i - 1] == '}')
                {
                    txt2 = txt2.Insert(i + count, "\n");
                    count++;
                }
            }      
            return txt2;
        }
    }
}
