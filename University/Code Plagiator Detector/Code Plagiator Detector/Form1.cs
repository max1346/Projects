using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Code_Plagiator_Detector;
using System.IO;

namespace Code_Plagiator_Detector
{
    public partial class Form1 : Form
    {
        private bool isCollapsed,window_status=true;
        int mov, movX, movY;
        Code_Plagiator_Detector.Scan scan;
        //Panel 1 //////////////////////////////////////////////////////////////////////////
        public Form1()
        {
            InitializeComponent();
            timer1.Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            panel_dashboard.Visible = true;
            if (File.Exists("default_folder.txt"))
            {
                string text = File.ReadAllText("default_folder.txt");
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                    listBox3.Items.Add(lines[i]);
            }
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle,
                System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(120)))), ((int)(((byte)(138))))), ButtonBorderStyle.Solid);
        }
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            mov = 0;
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if(window_status)
            {
                mov = 1;
                movX = e.X;
                movY = e.Y;
            }
        }
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mov == 1)
            {
                this.SetDesktopLocation(MousePosition.X - movX, MousePosition.Y - movY);
            }
        }
        //Exit Button ///////////////////////////////////////////////////////////////////////////
        private void Exit_Button_Click(object sender, EventArgs e)
        {
            string txt="";
            int i=0;
            foreach (var item in listBox3.Items)
            {
                if(i+1<listBox3.Items.Count) txt += item.ToString() + Environment.NewLine;
                else txt += item.ToString();
                i++;
            }
            File.WriteAllText("default_folder.txt", txt);
            Application.Exit();
        }
        //Zoom Button ///////////////////////////////////////////////////////////////////////////
        private void Zoom_Button_Click(object sender, EventArgs e)
        {
            if (window_status)
            {
                window_status = false;
                this.WindowState = FormWindowState.Maximized;
                this.Refresh();
            }
            else
            {
                window_status = true;
                this.WindowState = FormWindowState.Normal;
                this.Refresh();
            }
        }
        //Minimize Button ///////////////////////////////////////////////////////////////////////////
        private void Minimize_Button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        //Panel DashBoard///////////////////////////////////////////////////////////////////////////
        private void panel_dashboard_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle,
                System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(120)))), ((int)(((byte)(138))))), ButtonBorderStyle.Solid);
        }
        //List Box 1///////////////////////////////////////////////////////////////////////////
        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string file in files)        
                listBox1.Items.Add(file);
        }
        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ListBox.SelectedObjectCollection selectedItems = new ListBox.SelectedObjectCollection(listBox1);
                selectedItems = listBox1.SelectedItems;
                if (listBox1.SelectedIndex != -1)                
                    for (int i = selectedItems.Count - 1; i >= 0; i--)
                        listBox1.Items.Remove(selectedItems[i]);            
                else
                    MessageBox.Show("Empty Box for Files.");
            }
        }
        //List Box 2///////////////////////////////////////////////////////////////////////////        
        private void listBox2_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
        private void listBox2_DragDrop(object sender, DragEventArgs e)
        {
            string[] files2 = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string file in files2)
                listBox2.Items.Add(file);
        }
        private void listBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ListBox.SelectedObjectCollection selectedItems = new ListBox.SelectedObjectCollection(listBox2);
                selectedItems = listBox2.SelectedItems;
                if (listBox2.SelectedIndex != -1)               
                    for (int i = selectedItems.Count - 1; i >= 0; i--)
                        listBox2.Items.Remove(selectedItems[i]);               
                else
                    MessageBox.Show("Empty Box for Folders.");
            }
        }
        //Scan Button ///////////////////////////////////////////////////////////////////////////
        private void Scan_Button_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) MessageBox.Show("Empty box for Files");
            else 
            { 
                if(listBox2.Items.Count==0 && listBox3.Items.Count == 0 && !checkBox1.Checked) MessageBox.Show("Empty box for Folders");
                else
                {
                    var files = new List<string>();
                    foreach (var item in listBox1.Items)            
                        files.Add(item.ToString());

                    var folders = new List<string>();
                    foreach (var item in listBox2.Items)
                        folders.Add(item.ToString());

                    if(checkBox1.Checked)
                    {
                        foreach (var item in listBox3.Items)
                            folders.Add(item.ToString());
                    }
                    scan = new Scan(files,folders);
                    if (scan.Check_if_exists())
                    {
                        scan.Junk(listBox1);
                        panel_dashboard.Visible = false;
                        panel_dashboard_scanned.Visible = true;
                        textBox1.Clear();
                        scan.Blind_Brutus();
                        this.Display_text(scan.Get_text(),scan.Get_bool());
                        this.display_chart();                     
                    }
                    else MessageBox.Show("No files to scan.\nCheck if u done everything corect.");
                }
            }
        }
        private void display_chart()
        {
            int[] x = scan.Get_plagiarism();
            chart2.Series["s1"].Points.Clear();
            chart2.Series["s1"].Points.AddXY("Plagiat " + (100 / x[0] * x[1]) + "%", (100 / x[0] * x[1]));
            chart2.Series["s1"].Points.AddXY("Ne plagiat " + (100 - (100 / x[0] * x[1])) + "%", (100 - (100 / x[0] * x[1])));
        }
        private void Display_text(string text, bool[] sentence_plagiat)
        {
            string[] lines = text.Split('\n');
            if (lines.Length == sentence_plagiat.Length)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    textBox1.AppendText(lines[i]);
                    textBox1.AppendText(Environment.NewLine);
                }
            }
        }
        //Menu Button ///////////////////////////////////////////////////////////////////////////
        private void button7_Click(object sender, EventArgs e)
        {
            panel_menu.BringToFront();
            timer1.Start();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isCollapsed)
            {
                panel_menu.Width += 5;
                if(panel_menu.Width == 135)
                {
                    panel_menu.Height += 10;
                    if (panel_menu.Size == panel_menu.MaximumSize)
                    {
                        timer1.Stop();
                        isCollapsed = false;                       
                    }
                }
            }
            else
            {
                panel_menu.Height -= 10;
                if (panel_menu.Height == 36)
                {
                    panel_menu.Width -= 5;
                    if (panel_menu.Size == panel_menu.MinimumSize)
                    {
                        timer1.Stop();
                        isCollapsed = true;
                    }
                }
            }
            this.Refresh();
        }
        //HELP Button ///////////////////////////////////////////////////////////////////////////
        private void button10_MouseHover(object sender, EventArgs e)
        {
            Help.ShowPopup(textBox1, "Menu", new Point(this.Location.X, this.Location.Y));
            Help.ShowPopup(listBox1, "Select files to scan", new Point(this.Location.X+100, this.Location.Y+100));
        }
        //Main Page Panel ///////////////////////////////////////////////////////////////////////////        
        private void MainPage_Button_Click(object sender, EventArgs e)
        {
            panel_dashboard.Visible = true;
            panel_dashboard_scanned.Visible = false;
            panel_dashboard_scanned_details.Visible = false;
            Panel_Hi_lendar.Visible = false;
            Panel_Account.Visible = false;
            Store_Panel.Visible = false;
            Setting_Panel.Visible = false;
            About_Panel.Visible = false;

            chart2.Series["s1"].Points.Clear();
            chart2.Series["s1"].Points.AddXY("Variabilile", "40");
            chart2.Series["s1"].Points.AddXY("Ciclu", "40");
            chart2.Series["s1"].Points.AddXY("Comentarii", "20");
        }
        private void panel_menu_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle,
                System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(120)))), ((int)(((byte)(138))))), ButtonBorderStyle.Solid);
        }
        private void button1_Click(object sender, EventArgs e) //bakc button
        {
            panel_dashboard.Visible = true;
            panel_dashboard_scanned.Visible = false;
            panel_dashboard_scanned_details.Visible = false;
            Panel_Hi_lendar.Visible = false;
            Panel_Account.Visible = false;
            Store_Panel.Visible = false;
            Setting_Panel.Visible = false;
            About_Panel.Visible = false;
        }
        //Scaned Panel ///////////////////////////////////////////////////////////////////////////
        private void panel_dashboard_scanned_backward_Click(object sender, EventArgs e)
        {
            scan.Backward();
            textBox1.Clear();
            this.Display_text(scan.Get_text(),scan.Get_bool());
            this.display_chart();
        }
        private void panel_dashboard_scanned_forward_Click(object sender, EventArgs e)
        {
            scan.Forward();
            textBox1.Clear();
            this.Display_text(scan.Get_text(),scan.Get_bool());
            this.display_chart();
        }
        private void panel_dashboard_scanned_details_button_Click(object sender, EventArgs e)
        {
            panel_dashboard.Visible = false;
            panel_dashboard_scanned.Visible = false;
            panel_dashboard_scanned_details.Visible = true;
            Panel_Hi_lendar.Visible = false;
            Panel_Account.Visible = false;
            Store_Panel.Visible = false;
            Setting_Panel.Visible = false;
            About_Panel.Visible = false;
        }
        private void panel_dashboard_scanned_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle,
                System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(120)))), ((int)(((byte)(138))))), ButtonBorderStyle.Solid);
        }  
        //Scaned Details Panel ///////////////////////////////////////////////////////////////////////////
        private void panel_dashboard_scanned_details_back_button_Click(object sender, EventArgs e)
        {
            panel_dashboard.Visible = false;
            panel_dashboard_scanned.Visible = true;
            panel_dashboard_scanned_details.Visible = false;
            Panel_Hi_lendar.Visible = false;
            Panel_Account.Visible = false;
            Store_Panel.Visible = false;
            Setting_Panel.Visible = false;
            About_Panel.Visible = false;
        }
        private void panel_dashboard_scanned_details_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle,
                   System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(120)))), ((int)(((byte)(138))))), ButtonBorderStyle.Solid);
        }
        //Hi_Lendar Panel ///////////////////////////////////////////////////////////////////////////
        private void HiLendar_Button_Click(object sender, EventArgs e)
        {
            panel_dashboard.Visible = false;
            panel_dashboard_scanned.Visible = false;
            panel_dashboard_scanned_details.Visible = false;
            Panel_Hi_lendar.Visible = true;
            Panel_Account.Visible = false;
            Store_Panel.Visible = false;
            Setting_Panel.Visible = false;
            About_Panel.Visible = false;

            string text = File.ReadAllText("hi_lendar.txt");
            string[] lines = text.Split('\n');


            for(int i=0;i+4<lines.Length;i+=4)
            {
                Button button = new Button();
                button.Text = lines[i] + "\n" + lines[i + 1] + "\n" + lines[i + 2] + " " + lines[i + 3];
                button.Name = lines[i] + "\n" + lines[i + 1] + "\n" + lines[i + 2] + " " + lines[i + 3];
                button.Size = new System.Drawing.Size(290, 70);
                button.Click += new EventHandler(this.displat_chart);
                button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                button.Font = new System.Drawing.Font("Century Gothic", 10F);
                button.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
                button.Margin = new System.Windows.Forms.Padding(5);
                button.RightToLeft = System.Windows.Forms.RightToLeft.No;
                button.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                button.UseVisualStyleBackColor = true;
                flowLayoutPanel1.Controls.Add(button);
            }

            chart1.Series["s1"].Points.Clear();
            chart1.Series["s1"].Points.AddXY("Variabilile","30");
            chart1.Series["s1"].Points.AddXY("Ciclu", "30");
            chart1.Series["s1"].Points.AddXY("Comentarii", "30");
        }
        private void displat_chart(object sender, System.EventArgs e)
        {
            Button btn = sender as Button;
            string[] words = btn.Name.ToString().Split('\n',' ');
            int value;
            int[] x = new int[2] { 1, 1 };
            int i = 0;
            foreach (string word in words)
            {
                if(Int32.TryParse(word, out value))
                {
                    x[i] = value;
                    i++;
                }
            }
            chart1.Series["s1"].Points.Clear();
            chart1.Series["s1"].Points.AddXY("Plagiat " + (100 / x[0] * x[1]) + "%", (100 / x[0] * x[1]));
            chart1.Series["s1"].Points.AddXY("Ne plagiat " + (100 - (100 / x[0] * x[1])) + "%", (100 - (100 / x[0] * x[1])));
        }
        private void Panel_Hi_lendar_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle,
                System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(120)))), ((int)(((byte)(138))))), ButtonBorderStyle.Solid);
        }
        //Account Panel ///////////////////////////////////////////////////////////////////////////
        private void Account_Button_Click(object sender, EventArgs e)
        {
            panel_dashboard.Visible = false;
            panel_dashboard_scanned.Visible = false;
            panel_dashboard_scanned_details.Visible = false;
            Panel_Hi_lendar.Visible = false;
            Panel_Account.Visible = true;
            Store_Panel.Visible = false;
            Setting_Panel.Visible = false;
            About_Panel.Visible = false;
        }
        private void Panel_Account_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle,
                System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(120)))), ((int)(((byte)(138))))), ButtonBorderStyle.Solid);
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle,
                System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(120)))), ((int)(((byte)(138))))), ButtonBorderStyle.Solid);
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle,
                System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(120)))), ((int)(((byte)(138))))), ButtonBorderStyle.Solid);
        }
        //Store Panel ///////////////////////////////////////////////////////////////////////////
        private void Store_Button_Click(object sender, EventArgs e)
        {
            panel_dashboard.Visible = false;
            panel_dashboard_scanned.Visible = false;
            panel_dashboard_scanned_details.Visible = false;
            Panel_Hi_lendar.Visible = false;
            Panel_Account.Visible = false;
            Store_Panel.Visible = true;
            Setting_Panel.Visible = false;
            About_Panel.Visible = false;
        }
        private void Store_Panel_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle,
                System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(120)))), ((int)(((byte)(138))))), ButtonBorderStyle.Solid);
        }
        //Settings Panel ///////////////////////////////////////////////////////////////////////////
        private void Setting_Button_Click(object sender, EventArgs e)
        {
            panel_dashboard.Visible = false;
            panel_dashboard_scanned.Visible = false;
            panel_dashboard_scanned_details.Visible = false;
            Panel_Hi_lendar.Visible = false;
            Panel_Account.Visible = false;
            Store_Panel.Visible = false;
            Setting_Panel.Visible = true;
            About_Panel.Visible = false;
        }
        private void Setting_Panel_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle,
                System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(120)))), ((int)(((byte)(138))))), ButtonBorderStyle.Solid);
        }
        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle,
                System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(120)))), ((int)(((byte)(138))))), ButtonBorderStyle.Solid);
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
             radioButton2.Checked = false;
             radioButton3.Checked = false;
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            
            radioButton1.Checked = false;
            radioButton3.Checked = false;    
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Button button = new Button();
            button.Tag = 5;
            flowLayoutPanel1.Controls.Add(button);
        }
        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle,
                System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(120)))), ((int)(((byte)(138))))), ButtonBorderStyle.Solid);
        }
        private void listBox3_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
        private void listBox3_DragDrop(object sender, DragEventArgs e)
        {
            string[] files2 = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string file in files2)
                listBox3.Items.Add(file);
        }
        private void listBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ListBox.SelectedObjectCollection selectedItems = new ListBox.SelectedObjectCollection(listBox3);
                selectedItems = listBox3.SelectedItems;
                if (listBox3.SelectedIndex != -1)
                    for (int i = selectedItems.Count - 1; i >= 0; i--)
                        listBox3.Items.Remove(selectedItems[i]);
                else
                    MessageBox.Show("Empty Box for Folders.");
            }
        }
        //About Us Panel ///////////////////////////////////////////////////////////////////////////
        private void About_Button_Click(object sender, EventArgs e)
        {
            panel_dashboard.Visible = false;
            panel_dashboard_scanned.Visible = false;
            panel_dashboard_scanned_details.Visible = false;
            Panel_Hi_lendar.Visible = false;
            Panel_Account.Visible = false;
            Store_Panel.Visible = false;
            Setting_Panel.Visible = false;
            About_Panel.Visible = true;
        }
        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle,
                System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(120)))), ((int)(((byte)(138))))), ButtonBorderStyle.Solid);
        }
    }
}
