using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace QI_Plantform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int chess_color = 1;
        int line_num = 15;
        int num_space = 31;
        double chess_man_x = 25;
        double chess_man_y = 25;

        int space_length = 42;
        int line_length = 15;

        int[] dx = { -1, -1, 0, 1 };
        int[] dy = { 0, 1, 1, 1 };

        int time_limit = 0;

        bool FINISH = false;

        bool VS = false;

        int count_chess = 0;

        bool downChess = false;
        bool continueChess = false;

        string file_path_1 = "";
        string file_path_2 = "";

        Plantform_init single_qi = new Plantform_init();

        Qi_double_xy xy = new Qi_double_xy();

        List<Qi_struct> List_qi = new List<Qi_struct>();
        List<Qi_struct> List_qi_removed = new List<Qi_struct>();

        public MainWindow()
        {
            InitializeComponent();

            time_limit = int.Parse(time_box.Text);

            for (int i = 1; i <= line_num; i++)
            {
                huaQipanLine(space_length, space_length * i, space_length * line_length, space_length * i); //画横线 
                huaQipanLine(space_length * i, space_length, space_length * i, space_length * line_length); //画竖线 
            }
            QiPan_init();//add
        }

        public void QiPan_init()//棋盘初始化
        {
            single_qi.init_plantform();
        }

        public void huaQipanLine(int x1, int y1, int x2, int y2)//画棋盘
        {
            Line l = new Line();
            l.X1 = x1;
            l.Y1 = y1;
            l.X2 = x2;
            l.Y2 = y2;
            l.Stroke = new SolidColorBrush(Colors.Black);
            QiPan.Children.Add(l);
        }

        public bool DrawChessman(int chess_color, Point pt, int ax, int by, bool t)//下棋
        {
            int x = 0;
            int y = 0;
            double a = 0;
            double b = 0;
            bool change = false;

            //System.Windows.MessageBox.Show((pt.X-QiPan.Margin.Left).ToString()+"  "+ (pt.Y-QiPan.Margin.Top).ToString());
            if (!t)
            {
                a = (Math.Round((pt.X - QiPan.Margin.Left) / space_length)) * space_length - 25 / 2;
                b = (Math.Round((pt.Y - QiPan.Margin.Top) / space_length)) * space_length - 25 / 2;
                int a_ = Convert.ToInt32(a);
                int b_ = Convert.ToInt32(b);
                x = (a_ - 26) / space_length;
                y = (b_ - 26) / space_length;
            }

            if (t)
            {
                x = ax;
                y = by;
                a = ax * space_length + 28.0;
                b = by * space_length + 28.0;
            }

            xy = new Qi_double_xy(a, b);

            if (((pt.X - QiPan.Margin.Left >= 40 && pt.Y - QiPan.Margin.Top >= 40) || t == true) && x < line_num && y < line_num && not_chessed(x, y))//下子
            {
                single_qi.qi[x][y].set_XY(x, y);
                single_qi.qi[x][y].set_empty(false);
                single_qi.qi[x][y].set_color(chess_color);
                //System.Windows.MessageBox.Show(chess_color.ToString());
                count_chess++;
                single_qi.qi[x][y].set_value(count_chess);
                List_qi.Add(single_qi.qi[x][y]);
                up.IsEnabled = true;
                if (t != true)
                {
                    down.IsEnabled = false;
                    List_qi_removed.RemoveRange(0, List_qi_removed.Count);
                }
                change = true;
            }
            return change;
        }

        public void draw_chess()//画棋子
        {
            Ellipse el = new Ellipse();
            switch (chess_color)
            {
                case 1: el.Fill = new SolidColorBrush(Colors.White); break;
                case 2: el.Fill = new SolidColorBrush(Colors.Black); break;
            }
            el.Width = chess_man_x;
            el.Height = chess_man_y;
            el.ToolTip = count_chess.ToString();

            QiPan.Children.Add(el);
            Canvas.SetLeft(el, xy.get_x());
            Canvas.SetTop(el, xy.get_y());

            color();//color changed

            int a_ = Convert.ToInt32(xy.get_x());
            int b_ = Convert.ToInt32(xy.get_y());
            int x = (a_ - 26) / space_length;
            int y = (b_ - 26) / space_length;

            //int c = single_qi.qi[x][y].get_color();
            //System.Windows.MessageBox.Show(c.ToString());

            if (win_chess(x, y) == true)//judge if finished
            {
                FINISH = true;
                if (chess_color == 1)
                    System.Windows.MessageBox.Show("Black WIN");
                else
                    System.Windows.MessageBox.Show("White WIN");
                continueChess = false;
            }
        }

        public bool not_chessed(int x, int y)//判断是否可以落子
        {
            return single_qi.qi[x][y].get_empty();
        }

        public void color()//判断棋子颜色
        {
            if (chess_color == 1)
            {
                chess_color = 2;
            }
            else
            {
                chess_color = 1;
            }
        }

        public bool win_chess(int x, int y)//判断是否胜利
        {
            //判断下完该子之后，周围8个方向上与其同色的棋子数是否大于5
            for (int i = 0; i < 4; i++)//左-右、左下-右上、下-上、右下-左上
            {
                int color = single_qi.qi[x][y].get_color();
                int t = 0;
                int t1 = 0;
                //正方向:左、左下、下、右下
                while (x + t * dx[i] >= 0
                    && y + t * dy[i] >= 0
                    && x + t * dx[i] < line_num
                    && y + t * dy[i] < line_num
                    && single_qi.qi[x + t * dx[i]][y + t * dy[i]].get_empty() == false
                    && single_qi.qi[x + t * dx[i]][y + t * dy[i]].get_color() == color)
                    t++;
                t1 = t;
                t = 0;
                //反方向：右、右上、上、左上
                while (x - t * dx[i] >= 0
                    && y - t * dy[i] >= 0
                    && x - t * dx[i] < line_num
                    && y - t * dy[i] < line_num
                    && single_qi.qi[x - t * dx[i]][y - t * dy[i]].get_empty() == false
                    && single_qi.qi[x - t * dx[i]][y - t * dy[i]].get_color() == color)
                    t++;
                t1 = t1 + t - 1;

                if (t1 >= 5)
                    return true;

            }
            return false;
        }

        public delegate void NextChess();

        private void QiPan_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)//单击下子
        {
            if (FINISH == false)
            {
                Point pt = e.GetPosition(this);
                if (DrawChessman(chess_color, pt, -1, -1, false))
                {
                    if (VS)//human vs AI
                    {
                        draw_chess();
                        this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle, new NextPrimeDelegate(this.runAI));
                    }
                    else draw_chess();//show chess on board
                }
            }
            else System.Windows.MessageBox.Show("GAME OVER");
            if (List_qi.Count == 0) up.IsEnabled = false;
            if (List_qi_removed.Count == 0) down.IsEnabled = false;
        }

        public void Restart()
        {
            chess_color = 1;//白子先手
            QiPan.Children.RemoveRange(num_space, count_chess);//清空棋盘
            FINISH = false;
            QiPan_init();//新建棋子结构体
            count_chess = 0;
            List_qi.RemoveRange(0, List_qi.Count);//棋子列表清空
            List_qi_removed.RemoveRange(0, List_qi_removed.Count);//棋子列表清空
            xy.init_xy();//判断是否该点结束，清空
            up.IsEnabled = false;
            down.IsEnabled = false;
            load.IsEnabled = true;
            load_2.IsEnabled = true;
            single.IsEnabled = true;
            continueChess = false;
            downChess = false;
            VS = false;
            load_tip.Text = "";
            load_tip_2.Text = "";
        }

        private void restart_Click(object sender, RoutedEventArgs e)//重新开始
        {
            Restart();
        }

        private void up_Click(object sender, RoutedEventArgs e)//悔棋
        {
            down.IsEnabled = true;
            //撤销（一步）
            Qi_struct temp = List_qi[count_chess - 1];
            Qi_struct cp = new Qi_struct();
            cp = temp.copy(cp);
            List_qi.RemoveAt(count_chess - 1);
            List_qi_removed.Add(cp);
            single_qi.qi[cp.get_X()][cp.get_Y()].init();
            QiPan.Children.RemoveRange(count_chess + num_space - 1, 1);
            count_chess--;

            if (List_qi.Count == 0) up.IsEnabled = false;
            color();
        }

        private void down_Click(object sender, RoutedEventArgs e)//撤销
        {
            //前一步
            Qi_struct temp = List_qi_removed[List_qi_removed.Count - 1];
            List_qi_removed.RemoveAt(List_qi_removed.Count - 1);

            Point pt = new Point();
            if (DrawChessman(temp.get_color(), pt, temp.get_X(), temp.get_Y(), true))
                draw_chess();
            if (List_qi_removed.Count == 0) down.IsEnabled = false;
        }

        public void runAI()//运行AI
        {
            bool restart = false;
            Process process = new Process();
            //声明一个程序信息类
            ProcessStartInfo Info = process.StartInfo;
            //设置外部程序名
            if (VS) Info.FileName = file_path_1;
            else
            {
                if (chess_color == 1) Info.FileName = file_path_1;
                if (chess_color == 2) Info.FileName = file_path_2;
            }

            string v = chess_color + "\n";
            for (int i = 0; i < line_num; i++)
            {
                for (int j = 0; j < line_num; j++)
                    v = v + single_qi.qi[i][j].get_value() + " ";
                v = v + "\n";
            }

            Info.UseShellExecute = false;
            Info.RedirectStandardInput = true;//重定向输入流
            Info.RedirectStandardOutput = true;//重定向输出流
            Info.CreateNoWindow = true;

            //声明一个程序类
            Stopwatch timer = new Stopwatch();//计时

            try
            {
                timer.Start();
                //启动外部程序   
                process.Start();
            }
            catch (System.ComponentModel.Win32Exception a)
            {
                Console.WriteLine("系统找不到指定的程序文件。\r{0}", a);
                System.Windows.MessageBox.Show("System cannot find the right file !");
                return;
            }

            //获取输入流的操作Writer
            StreamWriter input = process.StandardInput;
            input.Write(v);

            //获取进程的输出流
            StreamReader output = process.StandardOutput;

            process.WaitForExit();

            timer.Stop();
            if (timer.ElapsedMilliseconds > time_limit)//判断是否超时
            {
                // Error();
                System.Windows.MessageBox.Show(timer.ElapsedMilliseconds + "ms >" + time_limit + "ms Time's up ! ERROR !");
                if (System.Windows.Forms.MessageBox.Show("Restart ?", "error tip", System.Windows.Forms.MessageBoxButtons.YesNo).Equals(
                    System.Windows.Forms.DialogResult.Yes))
                {
                    Restart();
                    restart = true;
                }
            }

            //System.Windows.MessageBox.Show(output.ReadToEnd());
            //等待
            // process.WaitForExit(time_limit);

            if (!restart)
            {
                string[] s = output.ReadToEnd().Split(' ');
                Point pt = new Point();
                int a1 = int.Parse(s[0]);
                int b1 = int.Parse(s[1]);
                if (DrawChessman(chess_color, pt, int.Parse(s[0]) - 1, int.Parse(s[1]) - 1, true))//set info
                    draw_chess();

                if (continueChess)
                {
                    load.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle, new NextPrimeDelegate(this.runAI));
                }
            }
        }

        public delegate void NextPrimeDelegate();

        public string load_file(string file_path)//文件选择器
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".exe";
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open file 
                string filename = dlg.FileName;
                file_path = filename;
            }
            return file_path;
        }

        private void load_Click(object sender, RoutedEventArgs e)//载入AI
        {
            load.IsEnabled = false;
            time_limit = int.Parse(time_box.Text);
            file_path_1 = load_file(file_path_1);
            if (file_path_1.Equals(""))
                load.IsEnabled = true;
            else
            {
                if (load_tip_2.Text.Equals("")) load_tip.Text = file_path_1;
                else
                {
                    if (continueChess)
                    {
                        continueChess = false;
                    }
                    else
                    {
                        load_tip.Text = file_path_1;
                        single.IsEnabled = false;
                        continueChess = true;
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new NextPrimeDelegate(runAI));
                    }
                }
            }
        }

        private void single_Click(object sender, RoutedEventArgs e)
        {
            if (!VS) file_path_1 = load_file(file_path_1);

            if (!file_path_1.Equals(""))
            {
                time_limit = int.Parse(time_box.Text);
                load_tip.Text = "human";
                load_tip.TextAlignment = System.Windows.TextAlignment.Center;
                load_tip_2.Text = file_path_1;
                load_tip_2.TextAlignment = System.Windows.TextAlignment.Center;
                load.IsEnabled = false;
                load_2.IsEnabled = false;
                VS = true;
                runAI();
            }
            //VS = true;
        }

        private void button1_Click(object sender, RoutedEventArgs e)//载入AI_2
        {
            load_2.IsEnabled = false;
            file_path_2 = load_file(file_path_2);
            time_limit = int.Parse(time_box.Text);
            if (file_path_2.Equals("")) load_2.IsEnabled = true;
            else
            {
                if (load_tip.Text.Equals("")) load_tip_2.Text = file_path_2;
                else
                {
                    if (continueChess)
                    {
                        continueChess = false;
                    }
                    else
                    {
                        load_tip_2.Text = file_path_2;
                        single.IsEnabled = false;
                        continueChess = true;
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new NextPrimeDelegate(runAI));
                    }
                }
            }
        }
    }
}
/*
（1）什么是呈现线程和UI线程：呈现线程仅是用于绘画窗口界面元素，每一个绘画都是由UI线程触发的，UI线程向消息队列发送绘画相关消息，由呈现线程实现绘画，比如重绘消息。
（2）在可视化应用中，如果，没有新建线程，那么所有代码都是由UI线程执行的，因此，在执行一个大任务时，由于UI线程忙，无法发送重绘消息，窗口就不会有响应，好像死掉一样。
（3）对UI的操作，比如更改控件属性（引起重绘），都必须由UI线程来执行，其它线程来更改UI的信息的话，会抛出异常。
（4）为了避免执行大任务导致窗口没有反应，你需要新建一个线程，让它执行任务，当新线程需要操作UI时，它需要利用Invoke和BeginInvoke来操作，这两个函数最终转给UI线程执行。
 */


