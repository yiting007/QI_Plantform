using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QI_Plantform
{

    public class Qi_double_xy
    {
        private double x;
        private double y;

        public Qi_double_xy()
        {
        }

        public Qi_double_xy(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double get_x()
        {
            return this.x;
        }

        public double get_y()
        {
            return this.y;
        }
        public void init_xy()
        {
            this.x = 0;
            this.y = 0;
        }
    }

    public class Qi_struct
    {
        private int x;
        private int y;
        private int value;
        private bool empty;
        private int color;//1=black 0=null 2=white

        public Qi_struct copy(Qi_struct p)
        {
            p.set_XY(this.x,this.y);
            p.set_color(this.color);
            p.set_empty(false);
            return p;
        }

        public void set_value(int v)
        {
            this.value = v;
        }

        public int get_value()
        {
            return this.value;
        }

        public void set_XY(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int get_X()
        {
            return this.x;
        }

        public int get_Y()
        {
            return this.y;
        }

        public void set_empty(bool e)
        {
            this.empty = e;
        }

        public bool get_empty()
        {
            return this.empty;
        }

        public void set_color(int c)
        {
             this.color = c;
        }

        public int get_color()
        {
            return this.color;
        }

        public void init()
        {
            this.x = -1;
            this.y = -1;
            this.color = 0;
            this.value = 0;
            this.empty = true;
        }

    }


    public class Plantform_init
    {
        static int line_num = 16;
        public Qi_struct[][] qi = new Qi_struct[line_num][];
        public void init_plantform()
        {        
            for (int i = 0; i < qi.Length; i++)
            {
                qi[i] = new Qi_struct[line_num];
            }

            for (int i = 0; i < line_num; i++)
                for (int j = 0; j < line_num; j++)
                {
                    qi[i][j] = new Qi_struct();
                    qi[i][j].init();
                }
        }
    }
}
