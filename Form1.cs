using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace SteamClue_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            foreach (Control item in this.Controls)
            {
                if (item is Button)
                {
                    
                    item.MouseEnter += (s, e) => (item as Button).FlatStyle = FlatStyle.Standard;
                    item.MouseLeave += (s, e) => (item as Button).FlatStyle = FlatStyle.Popup;
                    item.BackgroundImageLayout = ImageLayout.Stretch;                                    
                }
            }
            button1.Click += (s, e) => {
                Form2 newForm2 = new Form2(this);
                newForm2.Show();
                this.Visible = false;
            };
            button2.Click += (s, e) => show_help();
            button3.Click += (s, e) => Application.Exit(); 
        }

        public void show_help()
        {
            Form help_form = new Form();
            help_form.Size = new Size(600, 300);
            Label help_text = new Label();
            help_text.Font = new Font("Courier New", 14);
            help_text.Size = new Size(help_form.Width - 2, help_form.Height - 2);
            string[] readHelp = File.ReadAllLines(@"help.txt");
            foreach (var line in readHelp)
            {
                help_text.Text += (line + "\n");
                
            }
            
            help_form.Text = @"Help";
            help_form.Controls.Add(help_text);
            help_form.Show();
        }
    }
}
