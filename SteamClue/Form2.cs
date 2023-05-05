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
        int field_size = 4;
        int default_size = St_Size;

        Graphics gr;
        Pen field_pen = new Pen(Color.Green);

        public Form2(Form1 f)
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
            ICollideable[] obstes = { Ep1, new Stone(point_creator(151, 325), sz), new Stone(point_creator(301, 400), sz) };

            Pl.ep = Ep1;
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
                if (!(f.Visible)) f.Close();
            };

            Label lim_box = new Label();
            lim_box.Location = new Point(400, 25);
            lim_box.Text = Convert.ToString(Pl.lim);

            Button quit_btn = new Button
            {
                Location = point_creator(5, 550),
                Size = new Size(75, 50),
                TabStop = false,
                Text = "QUIT!"
            };

            Pl.Ehandler += () => lim_box.Text = Convert.ToString(Pl.lim);
            Ep1.WinEvent += () => Ep1.show_winscr();
            Ep1.FailEvent += () => Ep1.show_failscr();
            quit_btn.Click += (s, e) =>
            {
                f.Visible = true;
                this.Close();
            };

            this.Controls.Add(quit_btn);
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
        public ICollideable[] obstacles;
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
            lim = 6;
            step = size.Width + 2;
            bounds = bds;
            used_form = form;
            form.KeyDown += Move;
            
        }

        public void Call_Ehandler()
        {
           Ehandler?.Invoke();
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
            plbox.Location = l;
            bool iscollide = false;
            Func<ICollideable, bool> pred = (st) => st.CheckCollision(this);
            foreach (var item in obstacles)
            {
                if (pred(item)) iscollide = true;
            }
            bool possible_move = ( (bounds[0] <= l.X && bounds[2] >= l.X) &&
                                   (bounds[1] <= l.Y && bounds[3] >= l.Y) && lim > 0 )
                                   && !ep.CheckCollision(this) && !iscollide;
                               
            if (possible_move)
            {             
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

    public interface ICollideable
    {
        PictureBox obsbox { get; set; }
        bool CheckCollision(Player p);
    }
    
    public class EPoint : ICollideable
    {
        PictureBox ICollideable.obsbox { get; set;}
        public ICollideable IC;
        public bool lvl_complete;

        public delegate void fdg();
        public event fdg WinEvent;

        public event fdg FailEvent;
        

    public EPoint(Point spawn, Size size)
        {
            // Constructor Classes.Epoint
            IC = this;
            IC.obsbox = new PictureBox
            {
                Size = size,
                ImageLocation = @"\epoint.png",
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = spawn
            };           
        }
        public bool CheckCollision(Player p)
        {
            if (p.plbox.Location == IC.obsbox.Location && lvl_complete == default(bool))
            {               
                WinEvent?.Invoke();
                lvl_complete = true;
                return true;             
            }
            if (p.lim <= 0)
            {
                if (lvl_complete == default(bool)) FailEvent?.Invoke();
            }
            return false;
        }

        public void show_winscr()
        {
            MessageBox.Show("Level Completed!");
        }

        public void show_failscr()
        {
            MessageBox.Show("Level Failed!");
        }
    };


    public class BaseObstacle : ICollideable
    {
        PictureBox ICollideable.obsbox { get; set; }
        public ICollideable IC;
        public delegate void fdg();
        public event fdg CollideEvent;

        public BaseObstacle(Point spawn, Size size)
        {
            // Constructor Classes.Epoint
            IC = this;
            IC.obsbox = new PictureBox
            {
                Size = size,
                ImageLocation = null,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = spawn
            };
        }
        public bool CheckCollision(Player p)
        {
            if (p.plbox.Location == IC.obsbox.Location) {
                return true;
            }
            return false;
        }
    };

    public class Stone : BaseObstacle, ICollideable
    {
        public Stone(Point spawn, Size size) : base(spawn, size)
        {
            IC.obsbox.ImageLocation = @"stone.png";           
        }
    };

}

