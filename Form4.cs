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
            main_f = f;

            Point point_creator(int x, int y)
            {
                Point p = new Point(x, y);
                return p;
            }

            Size sz = new Size(default_size - 2, default_size - 2);
            int[] b = new int[4];
            Player Pl = new Player(point_creator(401, 386), sz, this, b);

            EPoint Ep = new EPoint(point_creator(151, 176), sz);
            ICollideable[] obstes = { };
            Pl.obstacles = obstes;
            Pl.lim = 50;
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
            MessageBox.Show("1");
        }
        public void show_failscr()
        {
            MessageBox.Show("0");
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
