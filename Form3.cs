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
        Form1 main_f;
        Graphics gr;
        Pen field_pen = new Pen(Color.Green);

        public Form3(Form1 f)
        {
            InitializeComponent();
            gr = this.CreateGraphics();
            this.ShowIcon = false;
            main_f = f;

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

            Label lim_box = new Label
            {
                Location = new Point(400, 25),
                Font = new Font("Arial", 14),
                Text = "Steps: " + Pl.lim.ToString(),
            };

            this.FormClosing += (s, e) =>
            {
                if (!(main_f.Visible) && !Ep.detectEnd) main_f.Close();
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
            Pl.Ehandler += () => lim_box.Text = "Steps: " + Convert.ToString(Pl.lim);
            Ep.WinEvent += () =>
            {
                show_winscr();
                Ep.detectEnd = true;
            };
            Ep.FailEvent += () =>
            {
                show_failscr();
                Ep.detectEnd = true;
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
                Form3 resLvlForm = new Form3(this.main_f);
                resLvlForm.Show();
                winscr.Close();
                this.Close();
            };

            next_btn.Click += (s, e) =>
            {
                Form4 nextLvlForm = new Form4(this.main_f);
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
                Form3 resLvlForm = new Form3(this.main_f);
                resLvlForm.Show();
                failscr.Close();
                this.Close();
            };
            failscr.Show();
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
