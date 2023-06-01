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
            Wall[] inner_ring = {new Wall(point_creator(381, 71), sz), new Wall(point_creator(431, 71), sz),
                                 new Wall(point_creator(481, 71), sz), new Wall(point_creator(381, 121), sz),
                                 new Wall(point_creator(481, 121), sz), new Wall(point_creator(381, 171), sz),
                                 new Wall(point_creator(431, 171), sz), new Wall(point_creator(481, 171), sz)};
            Wall[] gold_ring = { new Wall(point_creator(231, 121), sz), new Wall(point_creator(331, 121), sz)};
            ICollideable[] obstes = { Ep, new GearPanel(point_creator(481, 421), sz, inner_ring, "gold"),
                                      new Gear(point_creator(331, 71), sz, "gold")};
            obstes = obstes.Concat(inner_ring).ToArray().Concat(gold_ring).ToArray();

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
            Pl.lim = 50;
            Pl.ep = Ep;
            Label lim_box = new Label
            {
                Location = new Point(400, 25),
                Font = new Font("Arial", 14),
                Text = "Steps: " + Pl.lim.ToString(),
            };
            Button quit_btn = new Button
            {
                Location = point_creator(5, 750),
                Size = new Size(75, 50),
                TabStop = false,
                Text = "QUIT!"
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

        private void show_failscr()
        {
            throw new NotImplementedException();
        }

        private void show_winscr()
        {
            throw new NotImplementedException();
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
