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
// First level -> all-worked;

namespace SteamClue_
{
    public partial class Form2 : Form
    {
        const int St_Size = 75;
        const int Sm_Size = 50;
        int field_size = 4;
        int default_size = St_Size;

        Graphics gr;
        Pen field_pen = new Pen(Color.Green);

        public Form2(Form1 f)
        {
            InitializeComponent();
            gr = this.CreateGraphics();

            Point p_s = new Point(151, 400);
            Size sz = new Size(default_size - 2, default_size - 2);
            int[] b = new int[4];

            Player Pl = new Player(p_s, sz, this, b);

            this.Click += (s, e) =>
            {
                Pl.bounds = paint_field(field_size);
                this.Controls.Add(Pl.plbox);
            };

            Label lim_box = new Label();
            lim_box.Location = new Point(400, 25);
            lim_box.Text = Convert.ToString(Pl.lim);
            Pl.Ehandler += () => lim_box.Text = Convert.ToString(Pl.lim);
            this.Controls.Add(lim_box);
            //this.Close += (s, e) => Application.Close();

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
    }
}

namespace Classes
{
    public class Player
    {
        public PictureBox plbox;
        public string[] inventory;
        public int[] bounds;
        public int lim, step;
        public delegate void void_dg();
        public event void_dg Ehandler; 

        public Player(Point spawn, Size size, Form form, int[] bds)
        {
            plbox = new PictureBox
            {
                Size = size,
                ImageLocation = @"\player.png",
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = spawn
            };
            lim = 6;
            step = size.Width + 2;
            bounds = bds;
            form.KeyDown += Move;
        }
        public void Move(object sender, KeyEventArgs e)
        {
            Point l = plbox.Location;
            Point pr = plbox.Location;
            switch (e.KeyCode)
            {
                case Keys.W:
                    l.Y -= step;
                    break;
                case Keys.A:
                    l.X -= step;
                    break;
                case Keys.S:
                    l.Y += step;
                    break;
                case Keys.D:
                    l.X += step;
                    break;
            }
            bool possible_move = (bounds[0] <= l.X && bounds[2] >= l.X) &&
                                   (bounds[1] <= l.Y && bounds[3] >= l.Y) && lim > 0;
            if (possible_move)
            {
                plbox.Location = l;
                lim--;
                Ehandler?.Invoke();
                return;
            }
            plbox.Location = pr;


        }
        public void Erase(Form form)
        {
            form.KeyDown -= Move;
        }

        
    };

}

