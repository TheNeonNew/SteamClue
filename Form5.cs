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
    public partial class Form5 : Form
    {
        const int St_Size = 75;
        const int Sm_Size = 50;
        int default_size = Sm_Size;
        Form1 main_f;
        Graphics gr;
        Pen field_pen = new Pen(Color.Green);
        public Form5(Form1 f)
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
            Player Pl = new Player(point_creator(131, 421), sz, this, b);
            EPoint Ep = new EPoint(point_creator(431, 121), sz);
            Wall[] inner_ring = { new Wall(point_creator(381, 71), sz), new Wall(point_creator(431, 71), sz),
                                  new Wall(point_creator(481, 71), sz), new Wall(point_creator(381, 121), sz),
                                  new Wall(point_creator(481, 121), sz), new Wall(point_creator(381, 171), sz),
                                  new Wall(point_creator(431, 171), sz), new Wall(point_creator(481, 171), sz)};
            BaseObstacle[] gold_ring = { new Wall(point_creator(431, 221), sz), new Wall(point_creator(481, 221), sz),
            new Spike(point_creator(131, 121), sz, Ep), new Spike(point_creator(281, 121), sz, Ep)};
            BaseObstacle[] bronze_ring = { new Wall(point_creator(281, 421), sz), new Spike(point_creator(481, 371), sz, Ep), 
                new Spike(point_creator(281, 171), sz, Ep, "imgs\\spike_hidden.png", true) };
            ICollideable[] obstes = { Ep, new GearPanel(point_creator(281, 71), sz, inner_ring, "gold"),
                                      new Gear(point_creator(481, 421), sz, "bronze"), new Gear(point_creator(131, 71), sz, "gold"),
                                      new Spike(point_creator(231, 71), sz, Ep), new Wall(point_creator(331, 321), sz),
                                      new Spike(point_creator(331, 71), sz, Ep), new Gear(point_creator(231, 171), sz, "iron"),
                                      new GearPanel(point_creator(231, 421), sz, gold_ring, "bronze"), new Wall(point_creator(381, 321), sz),
                                      new GearPanel(point_creator(231, 121), sz, bronze_ring, "iron"), new Spike(point_creator(331, 221), sz, Ep),
                                      new Spike(point_creator(231, 371), sz, Ep), new Spike(point_creator(181, 421), sz, Ep),
                                      new Wall(point_creator(181, 121), sz), new Wall(point_creator(231, 121), sz),
                                      new Spike(point_creator(181, 171), sz, Ep), new Spike(point_creator(231, 221), sz, Ep, "imgs\\spike_hidden.png", true),
                                      new Spike(point_creator(431, 421), sz, Ep,"imgs\\spike_hidden.png", true),
                                      new Spike(point_creator(381, 371), sz, Ep), new Spike(point_creator(431, 271), sz, Ep, "imgs\\spike_hidden.png", true),
                                      new Spike(point_creator(131, 271), sz, Ep, "imgs\\spike_hidden.png", true)
            };
            obstes = obstes.Concat(inner_ring).ToArray().Concat(gold_ring).ToArray().Concat(bronze_ring).ToArray();
     

            this.FormClosing += (s, e) =>
            {
                if (!(main_f.Visible) && !Ep.detectEnd) main_f.Close();
            };

            this.Click += (s, e) =>
            {
                Pl.bounds = paint_field(8);
                this.Controls.Add(Pl.plbox);

                foreach (var item in obstes)
                {
                    this.Controls.Add(item.obsbox);
                }
            };
            Pl.obstacles = obstes;
            Pl.lim = 45;
            Pl.ep = Ep;
            Label lim_box = new Label
            {
                Location = new Point(400, 25),
                Font = new Font("Arial", 14),
                Text = "Steps: " + Pl.lim.ToString(),
            };
            Label hint_box = new Label
            {
                Location = new Point(100, 500),
                Font = new Font("Arial", 24),
                AutoSize = true,
                Text = "Panels also can affect on a spike",
            };
            Button quit_btn = new Button
            {
                Location = point_creator(5, 640),
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

            Control[] ctrls = { quit_btn, hint_box, lim_box };
            this.Controls.AddRange(ctrls);
        }

        private void show_failscr()
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
                Form5 resLvlForm = new Form5(this.main_f);
                resLvlForm.Show();
                failscr.Close();
                this.Close();
            };
            failscr.Show();
        }

        private void show_winscr()
        {
            Form winscr = new Form();
            winscr.Size = new Size(600, 300);
            winscr.Text = "Win Screen";
            Label winlabel = new Label
            {
                Location = new Point(200, 50),
                Text = "You got the ending!",
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
                Form5 resLvlForm = new Form5(this.main_f);
                resLvlForm.Show();
                winscr.Close();
                this.Close();
            };

            next_btn.Click += (s, e) =>
            {
                Form6 nextLvlForm = new Form6(this.main_f);
                nextLvlForm.Show();
                winscr.Close();
                this.Close();

            };

            winscr.Show();
        }

        private int[] paint_field(int n, int sz = Sm_Size)
        {
            int b_x = 130;
            int b_y = 70;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    gr.DrawRectangle(field_pen, b_x, b_y, sz, sz);                    
                    b_x += sz;
                    
                }
                b_x = 130;
                b_y += sz;
            }
            int[] b = { 100, 70,
                     100 + sz * n, 70 + sz * n - 40};
            return b;

        }
    }
}
