using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace QI_Plantform
{
    class file
    {
        string path = @"E:\t.txt";
        public void F(int[][] a)
        {
            //创建并写入(将覆盖已有文件)
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    //sw.WriteLine("Hello");
                    for (int i = 0; i < 15; i++)
                    {
                        for (int j = 0; j < 15; j++)
                        {
                            a[i][j].ToString();
                            sw.Write(a[i][j] + " ");
                        }
                        sw.WriteLine("\n");
                    }
                }
            }
            //读取文件 OK
            using (StreamReader sr = File.OpenText(path))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    //MessageBox.Show(s);
                }
            }
        }
    }
}
