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
using Classes;
// First level -> all-worked;

namespace SteamClue_
{
    public partial class Form2 : Form
    {
        const int St_Size = 75;
        const int Sm_Size = 50;
        Form1 main_f;
        int field_size = 4;
        int default_size = St_Size;

        Graphics gr;
        Pen field_pen = new Pen(Color.Green);

        public Form2(Form1 main_f)
        {
            InitializeComponent();
            gr = this.CreateGraphics();

            Point point_creator(int x, int y) {
                Point p = new Point(x, y);
                return p;
            }
            
            Size sz = new Size(default_size - 2, default_size - 2);
            int[] b = new int[4];

            Player Pl = new Player(point_creator(151, 400), sz, this, b);
            EPoint Ep1 = new EPoint(point_creator(376, 175), sz);
            ICollideable[] obstes = { Ep1, new Wall(point_creator(151, 325), sz), 
                new Wall(point_creator(301, 400), sz)};
            //obstes.Select(ob => ob.EndingScrCallable ? ob.main_f = this : null);

            Pl.ep = Ep1;
            this.main_f = main_f;
            Pl.obstacles = obstes;

            this.Click += (s, e) =>
            {
                Pl.bounds = paint_field(field_size);
                this.Controls.Add(Pl.plbox);
                
                foreach (var item in obstes)
                {
                    this.Controls.Add(item.obsbox);
                }
            }; // painting field & adding player

            this.FormClosing += (s, e) =>
            {
                if (!(main_f.Visible) && !Ep1.detectEnd) main_f.Close();
            };

            Label lim_box = new Label
            {
                Location = new Point(400, 25),
                Font = new Font("Arial", 14),
                Text = "Steps: " + Pl.lim.ToString(),
            };
            
            Button quit_btn = new Button
            {
                Location = point_creator(5, 550),
                Size = new Size(75, 50),
                TabStop = false,
                Text = "QUIT!"
            };

            Pl.Ehandler += () => lim_box.Text = "Steps: " + Convert.ToString(Pl.lim);
            Ep1.WinEvent += () =>
            {
                show_winscr();
                Ep1.detectEnd = true;
            };
            Ep1.FailEvent += () =>
            {
                show_failscr();
                Ep1.detectEnd = true;
            };
            quit_btn.Click += (s, e) =>
            {
                main_f.Visible = true;
                this.Close();
            };

            this.Controls.Add(quit_btn);
            this.Controls.Add(lim_box);
            
        }

        public void show_winscr()
        {
            Form winscr = new Form();
            winscr.Size = new Size(600, 300);
            winscr.Text = "Win Screen";
            Label winlabel = new Label
            {
                Location = new Point(200, 50),
                Text = "Level Completed!",
                Size = new Size(250, 30),
                Font = new Font("Ariel", 18)
            };
            Button restart_btn = new Button
            {
                Location = new Point(100, 100),
                Size = new Size(100, 100),
                BackgroundImage = Image.FromFile(@"imgs\restart_btn.png"),
                BackgroundImageLayout = ImageLayout.Stretch

        };
            Button next_btn = new Button
            {
                Location = new Point(400, 100),
                Size = new Size(100, 100),
                BackgroundImage = Image.FromFile(@"imgs\next_lvl_btn.png"),
                BackgroundImageLayout = ImageLayout.Stretch
        };
            Control[] ctrls = { restart_btn, next_btn, winlabel };
            winscr.Controls.AddRange(ctrls);
            restart_btn.Click += (s, e) =>
            {
                Form2 resLvlForm = new Form2(this.main_f);
                resLvlForm.Show();
                winscr.Close();
                this.Close();
            };

            next_btn.Click += (s, e) =>
            {
                Form3 nextLvlForm = new Form3(this.main_f);
                nextLvlForm.Show();
                winscr.Close();
                this.Close();

            };

            winscr.Show();
        }

        public void show_failscr()
        {
            Form failscr = new Form();
            failscr.Size = new Size(600, 300);
            failscr.Text = "Win Screen";
            Label faillabel = new Label
            {
                Location = new Point(200, 50),
                Text = "Level Failed!",
                Size = new Size(250, 30),
                Font = new Font("Ariel", 18)
            };
            Button restart_btn = new Button
            {
                Location = new Point(250, 100),
                Size = new Size(100, 100),
                BackgroundImage = Image.FromFile(@"imgs\restart_btn.png"),
                BackgroundImageLayout = ImageLayout.Stretch

            };
            Control[] ctrls = { restart_btn, faillabel };
            failscr.Controls.AddRange(ctrls);
            restart_btn.Click += (s, e) =>
            {
                Form2 resLvlForm = new Form2(this.main_f);
                resLvlForm.Show();
                failscr.Close();
                this.Close();
            };
            failscr.Show();          
        }

        private int[] paint_field(int n, int sz = St_Size)
        {
            Func<int, int> start_value = (int V) => V / 2 - sz * (n / 2) - Convert.ToInt32(field_pen.Width);
            int b_x = start_value(this.Width);
            int b_y = start_value(this.Height);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    gr.DrawRectangle(field_pen, b_x, b_y, sz, sz);
                    b_x += sz;
                }
                b_x = start_value(this.Width);
                b_y += sz;
            }
            int[] b = { start_value(this.Width), start_value(this.Height),
                     start_value(this.Width) + sz * n, start_value(this.Height) + sz * n };
            return b;

        }
        public void next_level(Form1 f)
        {
            Form3 newForm3 = new Form3(f);
            newForm3.Show();
            this.Close();
        }
    }
}



