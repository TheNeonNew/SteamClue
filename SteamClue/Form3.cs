using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Classes;

namespace SteamClue_
{
    public partial class Form3 : Form
    {
        const int St_Size = 75;
        const int Sm_Size = 50;
        int default_size = Sm_Size;
        Graphics gr;
        Pen field_pen = new Pen(Color.Green);

        public Form3(Form1 main_f)
        {
            InitializeComponent();
            gr = this.CreateGraphics();

            Point point_creator(int x, int y)
            {
                Point p = new Point(x, y);
                return p;
            }

            Size sz = new Size(default_size - 2, default_size - 2);
            int[] b = new int[4];

            Player Pl = new Player(point_creator(401, 426), sz, this, b);
         
            EPoint Ep = new EPoint(point_creator(151, 176), sz);
            ICollideable[] obstes = { Ep, new Wall(point_creator(401, 326), sz), new Wall(point_creator(401, 226), sz), new Wall(point_creator(251, 176), sz), new Wall(point_creator(151, 226), sz),
                                      new Spike(point_creator(351, 376), sz, Ep,@"imgs\spike_hidden.png", true),
                                      new Spike(point_creator(251, 376), sz, Ep), new Spike(point_creator(251, 426), sz, Ep), new Spike(point_creator(301, 326), sz, Ep),
                                      new Spike(point_creator(251, 326), sz, Ep), new Spike(point_creator(201, 376), sz, Ep), new Spike(point_creator(351, 326), sz, Ep,@"imgs\spike_hidden.png", true),
                                      new Spike(point_creator(201, 326), sz, Ep, @"imgs\spike_hidden.png", true), new Spike(point_creator(301, 276), sz, Ep), new Spike(point_creator(351, 276), sz, Ep),
                                      new Spike(point_creator(401, 276), sz, Ep), new Spike(point_creator(351, 226), sz, Ep,@"imgs\spike_hidden.png", true), new Spike(point_creator(351, 176), sz, Ep),
                                      new Spike(point_creator(301, 176), sz, Ep), new Spike(point_creator(251, 276), sz, Ep), new Spike(point_creator(301, 226), sz, Ep),
                                      new Spike(point_creator(251, 276), sz, Ep), new Spike(point_creator(201, 276), sz, Ep), new Spike(point_creator(201, 226), sz, Ep), 
                                      new Spike(point_creator(251, 226), sz, Ep,@"imgs\spike_hidden.png", true)};
            Pl.obstacles = obstes;
            Pl.lim = 11;
            Pl.ep = Ep;

            Label lim_box = new Label();
            lim_box.Location = new Point(400, 25);
            lim_box.Text = Convert.ToString(Pl.lim);

            this.FormClosing += (s, e) =>
            {
                if (!(main_f.Visible) && !Ep.lvl_complete) main_f.Close();
            };

            this.Click += (s, e) =>
            {
                Pl.bounds = paint_field(6);
                this.Controls.Add(Pl.plbox);

                foreach (var item in obstes)
                {
                    this.Controls.Add(item.obsbox);
                }
            };

            Button quit_btn = new Button
            {
                Location = point_creator(5, 550),
                Size = new Size(75, 50),
                TabStop = false,
                Text = "QUIT!"
            };

            quit_btn.Click += (s, e) =>
            {
                main_f.Visible = true;
                this.Close();
            };
            Pl.Ehandler += () => lim_box.Text = Convert.ToString(Pl.lim);
            Ep.WinEvent += () => show_winscr();
            Ep.FailEvent += () => show_failscr();

            this.Controls.Add(quit_btn);
            this.Controls.Add(lim_box);

        }

        public void show_failscr()
        {
            MessageBox.Show("Level Failed!");
        }

        public void show_winscr()
        {
            MessageBox.Show("Level Completed!");
        }

        private int[] paint_field(int n, int sz = Sm_Size)
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
    }
}
