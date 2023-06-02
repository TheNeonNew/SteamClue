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

namespace SteamClue_
{
    public partial class Form6 : Form
    {
        const int St_Size = 100;
        Form1 main_f;
        int default_size = St_Size;

        Graphics gr;
        Pen field_pen = new Pen(Color.Green);

        public Form6(Form1 main_f)
        {
            InitializeComponent();
            gr = this.CreateGraphics();
            this.ShowIcon = false;

            Point point_creator(int x, int y)
            {
                Point p = new Point(x, y);
                return p;
            }

            Size sz = new Size(default_size - 2, default_size - 2);
            int[] b = new int[4];

            Player Pl = new Player(point_creator(251, 251), sz, this, b);
            EPoint Epf = new EPoint(point_creator(451, 251), sz);
            ICollideable[] obstes = { Epf, new Wall(point_creator(251, 151), sz),
                new Wall(point_creator(351, 151), sz), new Spike(point_creator(451, 151), sz, Epf),
                new Gear(point_creator(251, 51), sz, "iron"), new Gear(point_creator(351, 51), sz, "bronze"), new Gear(point_creator(451, 51), sz, "gold"),
            new Boiler(point_creator(351, 251), sz)};

            Pl.ep = Epf;
            this.main_f = main_f;
            Pl.obstacles = obstes;

            this.Click += (s, e) =>
            {
                Pl.bounds = paint_field(3);
                this.Controls.Add(Pl.plbox);

                foreach (var item in obstes)
                {
                    this.Controls.Add(item.obsbox);
                }
            }; // painting field & adding player

            this.FormClosing += (s, e) =>
            {
                if (!(main_f.Visible) && !Epf.detectEnd) main_f.Close();
            };

            Label lim_box = new Label
            {
                Location = new Point(550, 25),
                Font = new Font("Arial", 18),
                AutoSize = true,
                Text = "Steps: " + Pl.lim.ToString(),
            };

            Button quit_btn = new Button
            {
                Location = point_creator(5, 5),
                Size = new Size(75, 50),
                TabStop = false,
                Text = "QUIT!"
            };

            Pl.Ehandler += () => lim_box.Text = "Steps: " + Convert.ToString(Pl.lim);
            Epf.WinEvent += () =>
            {
                show_winscr();
                Epf.detectEnd = true;
            };
            Epf.FailEvent += () =>
            {
                show_failscr();
                Epf.detectEnd = true;
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
            Form wingamescr = new Form();
            wingamescr.Size = new Size(600, 300);
            wingamescr.Text = "Win Screen";
            Label wglabel = new Label
            {
                Location = new Point(200, 50),
                Text = "Game completed!",
                Size = new Size(250, 30),
                Font = new Font("Ariel", 18)
            };
            Button end_btn = new Button
            {
                Location = new Point(250, 100),
                Size = new Size(100, 100),
                BackgroundImage = Image.FromFile(@"imgs\thumb.png"),
                BackgroundImageLayout = ImageLayout.Stretch

            };
            end_btn.MouseEnter += (s, e) => end_btn.FlatStyle = FlatStyle.Standard;
            end_btn.MouseLeave += (s, e) => end_btn.FlatStyle = FlatStyle.Popup;
            end_btn.Click += (s, e) => {
                main_f.Visible = true;
                this.Close();
            };
            Control[] ctrls = { end_btn, wglabel };
            wingamescr.Controls.AddRange(ctrls);
            wingamescr.Show();
        }

        public void show_failscr()
        {
            return;
        }

        private int[] paint_field(int n, int sz = St_Size)
        {
            Func<int, int> start_value = (int V) => 250;
            int b_x = start_value(this.Width);
            int b_y = 50;
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
            int[] b = { start_value(this.Width), 50,
                     start_value(this.Width) + sz * n, 50 + sz * n };
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




