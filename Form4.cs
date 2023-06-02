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
    public partial class Form4 : Form
    {
        const int St_Size = 75;
        const int Sm_Size = 50;
        int default_size = St_Size;
        Form1 main_f;
        Graphics gr;
        Pen field_pen = new Pen(Color.Green);
        public Form4(Form1 f)
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
            Player Pl = new Player(point_creator(551, 386), sz, this, b);
            Wall doorWall = new Wall(point_creator(551, 161), sz);
            Wall[] doorWallarr = { doorWall };
            Wall[] fakeWarr = { new Wall(point_creator(176, 461), sz), new Wall(point_creator(251, 86), sz) };
            EPoint Ep = new EPoint(point_creator(551, 86), sz);
            ICollideable[] obstes = { Ep, new Spike(point_creator(476, 86), sz, Ep), doorWall, fakeWarr[0], fakeWarr[1],
                new GearPanel(point_creator(476, 161), sz, doorWallarr, "iron"), 
                new Gear(point_creator(326, 311), sz, "bronze"), new Spike(point_creator(401, 311), sz, Ep), 
                new Spike(point_creator(326, 386), sz, Ep, "imgs\\spike_hidden.png", true), 
                new Spike(point_creator(326, 236), sz, Ep, "imgs\\spike_hidden.png", true),
                new Spike(point_creator(251, 311), sz, Ep), new Spike(point_creator(251, 161), sz, Ep),
                new Spike(point_creator(251, 461), sz, Ep), new Spike(point_creator(251, 236), sz, Ep, "imgs\\spike_hidden.png", true),
                new Gear(point_creator(101, 161), sz, "iron"), new Wall(point_creator(101, 311), sz),
                new GearPanel(point_creator(101, 461), sz, fakeWarr, "bronze"), new Spike(point_creator(476, 461), sz, Ep)
            };
            Pl.obstacles = obstes;
            Pl.lim = 33;
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
                Pl.bounds = paint_field(7);
                this.Controls.Add(Pl.plbox);

                foreach (var item in obstes)
                {
                    this.Controls.Add(item.obsbox);
                }
            };

            Button quit_btn = new Button
            {
                Location = point_creator(5, 600),
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
                Form4 resLvlForm = new Form4(this.main_f);
                resLvlForm.Show();
                winscr.Close();
                this.Close();
            };

            next_btn.Click += (s, e) =>
            {
                Form5 nextLvlForm = new Form5(this.main_f);
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
                Form4 resLvlForm = new Form4(this.main_f);
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
            int b_y = start_value(this.Height) - 40;
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
            int[] b = { start_value(this.Width), start_value(this.Height) - 40,
                     start_value(this.Width) + sz * n, start_value(this.Height) + sz * n - 40};
            return b;

        }
    }
}

