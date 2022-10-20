using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml.XPath;

namespace TetrisNTBT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Tfield = new int[Theight, Twidth];
            for (int i = 0; i < Theight; i++)
            {
                for (int j = 0; j < Twidth; j++)
                {
                    Tfield[i, j] = 0;
                }
            }
        }
        public int Twidth = 10;
        public int Theight = 20;
        public int Tcount = 0;
        public int x = 3;
        public int y = 0;
        public int rotate = 0;

        public int[,] Tfield;
        public int[,,] Imino = {
            {
                {0,0,0,0 },
                {1,1,1,1 },
                {0,0,0,0 },
                {0,0,0,0 }
            },
            {
                {0,0,1,0 },
                {0,0,1,0 },
                {0,0,1,0 },
                {0,0,1,0 }
            },
            {
                {0,0,0,0 },
                {0,0,0,0 },
                {1,1,1,1 },
                {0,0,0,0 }
            },
            {
                {0,1,0,0 },
                {0,1,0,0 },
                {0,1,0,0 },
                {0,1,0,0 }
            }
        };

        private void Tpaint() { // 描画全般
            Console.WriteLine("Info: Drawing");
            Console.WriteLine("Info: TCount -> " + Tcount);
            Console.WriteLine("Info: X -> " + x);
            Console.WriteLine("Info: Y -> " + y);
            Bitmap canvas = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics graphics = Graphics.FromImage(canvas);
            SolidBrush brush = new SolidBrush(Color.Gray);
            graphics.FillRectangle(brush, 0, 0, pictureBox1.Width, pictureBox1.Height);

            for (int i = 0; i < Theight; i++)
            {
                for (int j = 0; j < Twidth; j++)
                {
                    if (Tfield[i, j] != 0) /// field側の描画
                    {
                        Color c = getColorById(Tfield[i, j]);
                        SolidBrush brush1 = new SolidBrush(c);
                        graphics.FillRectangle(brush1, 300 / Twidth * j, 600 / Theight * i, 30, 30); /// 青四角描画
                        Pen pen1 = new Pen(Color.Black);
                        graphics.DrawRectangle(pen1, 300 / Twidth * j, 600 / Theight * i, 30, 30); /// 枠線描画
                        brush1.Dispose();
                        pen1.Dispose();
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (Imino[rotate, i, j] != 0)
                    {
                        Color c = getColorById(Imino[rotate, i, j]);
                        SolidBrush brush1 = new SolidBrush(c);
                        graphics.FillRectangle(brush1, (300 / Twidth * x) + j * 300 / Twidth, y * 600/Theight + i * 600 / Theight, 30, 30);
                        Pen pen1 = new Pen(Color.Black);
                        graphics.DrawRectangle(pen1, (300 / Twidth * x) + j * 300 / Twidth, y * 600/Theight + i * 600 / Theight, 30, 30);
                        brush1.Dispose();
                        pen1.Dispose();
                    }
                }
            }

            pictureBox1.Image = canvas;
            graphics.Dispose();
            brush.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Tinit();
        }

        private Color getColorById(int id)
        {
            if (id == 1)
            {
                return Color.Cyan;
            }

            return Color.Brown;
        }

        private void Tinit() // メインループ
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1000 / 60;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(Function);
        }

        private void Function(object sender, ElapsedEventArgs eventArgs)
        {
            Tpaint();
            Tcount++;

            if (GetY(rotate) - y == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (Imino[rotate, i, j] != 0)
                        {
                            Tfield[y + i, x + j] = 1;
                        }
                    }
                }
                TRemove();
                x = 3;
                y = 0;
                rotate = 0;
            }

            if (CanPlace(rotate))
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (Imino[rotate, i, j] != 0)
                        {
                            Tfield[y + i, x + j] = 1;
                        }
                    }
                }

                TRemove();
                x = 3;
                y = 0;
                rotate = 0;
            }

            if (Tcount >= 15)
            {
                y++;
                Tcount = 0;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                if (x == GetMaxX(rotate))
                {
                    return;
                }
                if (!CanMove(rotate, 0))
                {
                    return;
                }
                x++;
            }
            if (e.KeyCode == Keys.Left)
            {
                if (x == GetMinX(rotate))
                {
                    return;
                }
                if (!CanMove(rotate, 1))
                {
                    return;
                }
                x--;
            }
            if (e.KeyCode == Keys.Down) y++;
            if (e.KeyCode == Keys.Z)
            {
                if (rotate == 0)
                {
                    if (CheckSpace(3))
                    rotate = 3;
                }
                else
                {
                    if (CheckSpace(rotate - 1))
                        rotate--;
                }
            }
            if (e.KeyCode == Keys.X)
            {
                if (rotate == 3)
                {
                    if (CheckSpace(0))
                        rotate = 0;
                }
                else
                {
                    if (CheckSpace(rotate + 1))
                    rotate++;
                }
            }
            if(e.KeyCode == Keys.Up)
            {
                if (y == GetY(rotate))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (Imino[rotate, i, j] != 0)
                            {
                                Tfield[y + i, x + j] = 1;
                            }
                        }
                    }

                    TRemove();
                    x = 3;
                    y = 0;
                    rotate = 0;
                }

                else if (CanPlace(rotate))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (Imino[rotate, i, j] != 0)
                            {
                                Tfield[y + i, x + j] = 1;
                            }
                        }
                    }

                    TRemove();
                    x = 3;
                    y = 0;
                    rotate = 0;
                }
                else
                {
                    for(int k = y; k < Theight; k++)
                    {
                        if(CanPlace(rotate, k))
                        {
                            y = k;
                            for (int i = 0; i < 4; i++)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    if (Imino[rotate, i, j] != 0)
                                    {
                                        Tfield[y + i, x + j] = 1;
                                    }
                                }
                            }

                            TRemove();
                            x = 3;
                            y = 0;
                            rotate = 0;
                            break;
                        }
                    }
                }
            }
        }
        
        private int GetMinX(int rotate) // xの可能最小値
        {
            if (rotate == 1)
            {
                return -2;
            }
            if(rotate == 3)
            {
                return -1;
            }
            return 0;
        }

        private int GetMaxX(int rotate) // xの可能最大値
        {
            if (rotate == 1)
            {
                return 7;
            }
            if (rotate == 3)
            {
                return 8;
            }
            return 6;
        }
        private int GetY(int rotate) /// 判定用のY座標をget
        {
            if (rotate == 0)
            {
                return 18;
            }
            else if (rotate == 1 || rotate == 3)
            {
                return 16;
            }
            else if (rotate == 2)
            {
                return 17;
            }
            else return 16;
        }


        private bool CanPlace(int rotate) /// 下面に対する判定
        {
            if(y >= 19)
            {
                return true;
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (Imino[rotate, i, j] != 0)
                    {
                        if (y + i + 1 >= 20)
                        {
                            return true;
                        }

                        if (Tfield[i + y + 1, x + j] != 0)
                        {
                            return true;
                        }

                    }

                }
            }
            return false;
        }

        private bool CanMove(int rotate, int direction) /// ブロックに対する回転の判定
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (Imino[rotate, i, j] != 0)
                    {
                        if (direction == 0)
                        {
                            if(y >= GetY(rotate) && Tfield[y+i, x+j+1] != 0)
                            {
                                return false;
                            }
                            if (Tfield[y+i,x+j+1] != 0)
                            {
                                return false;
                            }
                        }
                        if (direction == 1)
                        {
                            if(y >= GetY(rotate) && Tfield[y+i, x+j-1] != 0)
                            {
                                return false;
                            }
                            if(Tfield[y+i,x+j-1] != 0)
                            {
                                return false;
                            }
                        }

                    }

                }
            }
            return true;
        }

        private bool CheckSpace(int rotate)
        {
            if(rotate == 0)
            {
                for(int i = 0; i < 4; i++)
                {
                    if(x+i >= 10)
                    {
                        return false;
                    }
                    if(x+i <= 0)
                    {
                        return false;
                    }
                    if (Tfield[y, x + i] != 0)
                    {
                        return false;
                    }
                }
            }

            if(rotate == 1 && rotate == 3)
            {
                for(int j = 0; j < 4; j++)
                {
                    if (y + j >= 20)
                    {
                        return false;
                    }
                    if (Tfield[y + j, x] != 0)
                    {
                        return false;
                    }
                }
            }

            if(rotate == 2)
            {
                for(int i=0; i<4; i++)
                {
                    if (x + i < 0)
                    {
                        return false;
                    }
                    if (x + i >= 10)
                    {
                        return false;
                    }
                    if (Tfield[y, x + i] != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
        private void TRemove()
        {
            for(int i = 0; i < Theight; i++)
            {
                int c = 0;
                for(int j = 0; j < Twidth; j++)
                {
                    if(Tfield[i, j] == 0)
                    {
                        break;
                    }
                    c++;

                    if (c == 10)
                    {
                        Tfield[i, j] = 0;
                        for (int k = i - 1; 0 <= k; k--)
                        {
                            for(int l = 0; l < Twidth; l++)
                            {
                                Tfield[k + 1, l] = Tfield[k, l];
                            }
                        }
                    }
                }
            }
        }

        private bool CanPlace(int rotate, int y)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (Imino[rotate, i, j] != 0)
                    {
                        if(y+i+1 >= Theight)
                        {
                            return true;
                        }
                        if (Tfield[y + i + 1, x + j] != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
