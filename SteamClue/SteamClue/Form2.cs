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
            Point e_p = new Point(376, 175);
            Size sz = new Size(default_size - 2, default_size - 2);
            int[] b = new int[4];

            Player Pl = new Player(p_s, sz, this, b);
            EPoint Ep1 = new EPoint(e_p, sz);
            Pl.ep = Ep1;

            this.Click += (s, e) =>
            {
                Pl.bounds = paint_field(field_size);
                this.Controls.Add(Pl.plbox);
                this.Controls.Add(Ep1.epbox);
            }; // painting field & adding player

            this.FormClosed += (s, e) =>
            {
                f.Close();
            };

            Label lim_box = new Label();
            lim_box.Location = new Point(400, 25);
            lim_box.Text = Convert.ToString(Pl.lim);
            Pl.Ehandler += () => lim_box.Text = Convert.ToString(Pl.lim);
            Ep1.Ehandler += () => Ep1.finish_func();
            
            this.Controls.Add(lim_box);
            

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
        public EPoint ep;

        public delegate void void_dg();
        public event void_dg Ehandler;

        public Form used_form;
        
        public Player(Point spawn, Size size, Form form, int[] bds)
        {
            plbox = new PictureBox
            {
                Size = size,
                ImageLocation = @"\player.png",
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = spawn
            };
            lim = 10;
            step = size.Width + 2;
            bounds = bds;
            used_form = form;
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
                default:
                    plbox.Location = pr;
                    return;
            }
            bool possible_move = ( (bounds[0] <= l.X && bounds[2] >= l.X) &&
                                   (bounds[1] <= l.Y && bounds[3] >= l.Y) && lim > 0 )
                                   || ep.CheckCollision(this);
            //MessageBox.Show(Convert.ToString(l.Y));
                                 
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
    }
    public class EPoint
    {
        public PictureBox epbox;
        public bool lvl_complete;

        public delegate void fdg();
        public event fdg Ehandler;
        

    public EPoint(Point spawn, Size size)
        {
            // Constructor Classes.Epoint
            epbox = new PictureBox
            {
                Size = size,
                ImageLocation = @"C:\Users\пользователь\source\repos\SteamClue\SteamClue\epoint.jpg",
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = spawn
            };           
        }
        public bool CheckCollision(Player p)
        {
            if (p.plbox.Location.X == this.epbox.Location.X && p.plbox.Location.Y == this.epbox.Location.Y)
            {
                lvl_complete = true;
                Ehandler?.Invoke();
                return true;
            }
            return false;
        }

        public void finish_func()
        {
            MessageBox.Show("Level Completed!");
        }

    };

    public class BaseObstacle
    {

    };

    public class Stone : BaseObstacle
    {

    };

}

