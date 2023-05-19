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

namespace Classes
{
    public class Player
    {
        public PictureBox plbox;
        public Gear[] inventory = new Gear[3];
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
                ImageLocation = @"imgs\player.png",
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
            bool iscollide = true;
            Func<ICollideable, bool> pred = (st) => !st.CheckCollision(this);
            iscollide = obstacles.SkipWhile(obs => pred(obs)).ToArray().Length == 0;

            bool possible_move = ((bounds[0] <= l.X && bounds[2] >= l.X) &&
                                   (bounds[1] <= l.Y && bounds[3] >= l.Y) && lim > 0)
                                   && !ep.CheckCollision(this) && iscollide;
            if (possible_move)
            {
                Spike[] spikes = obstacles.OfType<Spike>().ToArray();
                foreach (Spike spike in spikes) spike.SwitchSpike();
                lim--;
                Ehandler?.Invoke();
                //MessageBox.Show(plbox.Location.ToString());
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
        bool EndingScrCallable { get; set; }
        bool CheckCollision(Player p);
    }

    public class EPoint : ICollideable
    {
        PictureBox ICollideable.obsbox { get; set; }
        bool ICollideable.EndingScrCallable { get; set; }
        public ICollideable IC;
        public bool lvl_complete;
        public bool detectEnd = false;

        public delegate void fdg();
        public event fdg WinEvent;

        public event fdg FailEvent;


        public EPoint(Point spawn, Size size)
        {
            // Constructor Classes.Epoint
            IC = this;
            IC.EndingScrCallable = true;
            IC.obsbox = new PictureBox
            {
                Size = size,
                ImageLocation = @"imgs\epoint.png",
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
        public void CallFailEvent()
        {
            FailEvent?.Invoke();
        }
    };


    public class BaseObstacle : ICollideable
    {
        PictureBox ICollideable.obsbox { get; set; }
        bool ICollideable.EndingScrCallable { get; set; }
        public ICollideable IC;
        public delegate void fdg();
        public event fdg CollideEvent;

        public BaseObstacle(Point spawn, Size size)
        {
            // Constructor Classes.Epoint
            IC = this;
            IC.EndingScrCallable = false;
            IC.obsbox = new PictureBox
            {
                Size = size,
                ImageLocation = null,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = spawn
            };
        }
        public virtual bool CheckCollision(Player p)
        {
            if (p.plbox.Location == IC.obsbox.Location)
            {
                return true;
            }
            return false;
        }

        public void CollideEventInvoke()
        {
            CollideEvent?.Invoke();
        }
    };

    public class Wall : BaseObstacle, ICollideable
    {
        public Wall(Point spawn, Size size) : base(spawn, size)
        {
            IC.obsbox.ImageLocation = @"imgs\wall.png";
        }
    };

    public class Spike : BaseObstacle, ICollideable
    {
        public EPoint related_ep;
        public bool hidden;
        public Spike(Point spawn, Size size, EPoint prep,
        string start_img = @"imgs\spike_up.png", bool hd = false) : base(spawn, size)
        {
            IC.EndingScrCallable = true;
            IC.obsbox.ImageLocation = start_img;
            related_ep = prep;
            hidden = hd;
            CollideEvent += () =>
            {
                related_ep.CallFailEvent();
            };
        }
        public override bool CheckCollision(Player p)
        {
            if (p.plbox.Location == IC.obsbox.Location)
            {
                if (hidden)
                {
                    SwitchSpike();
                    return false;
                }
                CollideEventInvoke();
                return true;
            }
            return false;
        }

        public void SwitchSpike()
        {
            if (hidden)
            {
                IC.obsbox.ImageLocation = @"imgs\spike_up.png";
            }
            else
            {
                IC.obsbox.ImageLocation = @"imgs\spike_hidden.png";
            }
            hidden = !hidden;
        }
    };

    public class Gear : BaseObstacle, ICollideable
    {
        bool collected = false;
        public Gear(Point spawn, Size size) : base(spawn, size)
        {
            IC.obsbox.ImageLocation = @"imgs\gear.png";           
        }
        

        public override bool CheckCollision(Player p)
        {
            if (p.plbox.Location == IC.obsbox.Location && !collected)
            {
                p.inventory.Append(this);
                collected = true;
                IC.obsbox.Visible = false;
                return false;
            }
            return false;
        }
    }

}
