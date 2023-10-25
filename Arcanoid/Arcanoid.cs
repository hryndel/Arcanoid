using System;
using System.Drawing;
using System.Windows.Forms;

namespace Arcanoid
{
    public partial class FormArcanoid : Form
    {
        const int MapWidth = 26;
        const int MapHeight = 26;
        const int CellWidth = 20;
        const int CellHeight = 20;

        private int[,] map = new int[MapHeight + 1, MapWidth + 1];
        private int dirX = 0;
        private int dirY = 0;
        private int platformX;
        private int platformY;
        private int ballX;
        private int ballY;
        private int score;

        private Image arcanoidSet;
        private Image background;
        private Image scoreboard;

        public FormArcanoid()
        {
            InitializeComponent();
            Init();
            BackgroundImage = background;
        }

        private void Init()
        {
            Width = (MapWidth + 1) * CellWidth - 3;
            Height = (MapHeight + 4) * CellHeight;

            arcanoidSet = new Bitmap(Properties.Resources.arcanoid);
            background = new Bitmap(Properties.Resources.background, Width, Height);
            scoreboard = new Bitmap(Properties.Resources.scoreboard);
            score = 0;

            timer.Interval = 75;

            for (var i = 0; i < MapHeight; i++)
            {
                for (var j = 0; j < MapWidth; j++)
                {
                    map[i, j] = 0;
                }
            }

            platformX = (MapWidth - 1) / 2;
            platformY = MapHeight - 1;

            ballY = platformY - 1;
            ballX = platformX + 1;

            map[ballY, ballX] = 8;

            dirX = 1;
            dirY = -1;

            map[platformY, platformX] = 9;
            map[platformY, platformX + 1] = 99;
            map[platformY, platformX + 2] = 99;
            map[platformY, platformX + 3] = 99;
            map[platformY, platformX + 4] = 99;

            PlatformsGenerate();

            timer.Start();
        }

        private void PlatformsGenerate()
        {
            Random random = new Random();
            for (var i = 0; i < MapHeight / 3; i++)
            {
                for (var j = 0; j < MapWidth; j += 2)
                {
                    map[i, j] = random.Next(1, 5);
                    map[i, j + 1] = map[i, j] * 11;
                }
            }
        }

        private void FormArcanoid_Paint(object sender, PaintEventArgs e)
        {
            DrawScoreBoard(e.Graphics);
            DrawMap(e.Graphics);
        }

        private void DrawScoreBoard(Graphics g)
        {
            g.DrawImage(scoreboard,
            new Rectangle(new Point(0, 0),
                          new Size(MapWidth * CellWidth, 2 * CellHeight)),
            0,
            0,
            2560,
            600,
            GraphicsUnit.Pixel);

            g.DrawString($"Score: {score}", new Font("Microsoft Sans Serif", 20, FontStyle.Bold), Brushes.AliceBlue, new Point(10, 5));
        }

        private void DrawMap(Graphics g)
        {
            for (var i = 0; i < MapHeight; i++)
            {
                for (var j = 0; j < MapWidth; j++)
                {
                    int pointX = j * CellWidth;
                    int pointY = i * CellHeight + 40;
                    switch (map[i, j])
                    {
                        case 9:
                            g.DrawImage(arcanoidSet,
                                    new Rectangle(new Point(pointX, pointY),
                                                  new Size(5 * CellWidth, CellHeight)),
                                    398,
                                    17,
                                    150,
                                    50,
                                    GraphicsUnit.Pixel);
                            break;
                        case 8:
                            g.DrawImage(arcanoidSet,
                                    new Rectangle(new Point(pointX, pointY),
                                                  new Size(CellWidth, CellHeight)),
                                    806,
                                    548,
                                    73,
                                    73,
                                    GraphicsUnit.Pixel);
                            break;
                        case 1:
                            g.DrawImage(arcanoidSet,
                                    new Rectangle(new Point(pointX, pointY),
                                                  new Size(2 * CellWidth, CellHeight)),
                                    20,
                                    16,
                                    170,
                                    59,
                                    GraphicsUnit.Pixel);
                            break;
                        case 2:
                            g.DrawImage(arcanoidSet,
                                    new Rectangle(new Point(pointX, pointY),
                                                  new Size(2 * CellWidth, CellHeight)),
                                    20,
                                    16 + 77 * (map[i, j] - 1),
                                    170,
                                    59,
                                    GraphicsUnit.Pixel);
                            break;
                        case 3:
                            g.DrawImage(arcanoidSet,
                                    new Rectangle(new Point(pointX, pointY),
                                                  new Size(2 * CellWidth, CellHeight)),
                                    20,
                                    16 + 77 * (map[i, j] - 1),
                                    170,
                                    59,
                                    GraphicsUnit.Pixel);
                            break;
                        case 4:
                            g.DrawImage(arcanoidSet,
                                    new Rectangle(new Point(pointX, pointY),
                                                  new Size(2 * CellWidth, CellHeight)),
                                    20,
                                    16 + 77 * (map[i, j] - 1),
                                    170,
                                    59,
                                    GraphicsUnit.Pixel);
                            break;
                        case 5:
                            g.DrawImage(arcanoidSet,
                                    new Rectangle(new Point(pointX, pointY),
                                                  new Size(2 * CellWidth, CellHeight)),
                                    20,
                                    16 + 77 * (map[i, j] - 1),
                                    170,
                                    59,
                                    GraphicsUnit.Pixel);
                            break;
                    }
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (ballY + dirY > MapHeight - 1)
            {
                timer.Stop();
                if (DialogResult.Yes == MessageBox.Show("Ещё раз?", "Вы проиграли!", MessageBoxButtons.YesNo))
                {
                    Init();
                }
                else
                {
                    Close();
                }

            }

            if (score >= 1000)
            {
                timer.Stop();
                if (DialogResult.Yes == MessageBox.Show("Ещё раз?", "Вы выиграли!", MessageBoxButtons.YesNo))
                {
                    Init();
                }
                else
                {
                    Close();
                }
            }

            map[ballY, ballX] = 0;
            IsCollideX();
            ballX += dirX;
            IsCollideY();
            ballY += dirY;
            map[ballY, ballX] = 8;

            Invalidate();
        }

        private void IsCollideX()
        {
            if (ballX + dirX > MapWidth - 1 || ballX + dirX < 0) // столкновение с границами карты
            {
                dirX *= -1;
            }

            if (map[ballY, ballX + dirX] != 0) // столкновение с объектом
            {
                if (map[ballY, ballX + dirX] > 10 && map[ballY, ballX + dirX] <= 55)
                {
                    map[ballY, ballX + dirX] = 0;
                    map[ballY, ballX + dirX - 1] = 0;
                    score += 50;
                }

                if (map[ballY, ballX + dirX] > 0 && map[ballY, ballX + dirX] <= 5)
                {
                    map[ballY, ballX + dirX] = 0;
                    map[ballY, ballX + dirX + 1] = 0;
                    score += 50;
                }

                dirX *= -1;
            }
        }

        private void IsCollideY()
        {
            if (ballY + dirY > MapHeight - 1 || ballY + dirY < 0) // столкновение с границами карты
            {
                dirY *= -1;
            }

            if (map[ballY + dirY, ballX] != 0) // столкновение с объектом
            {
                if (map[ballY + dirY, ballX] > 10 && map[ballY + dirY, ballX] <= 55)
                {
                    map[ballY + dirY, ballX] = 0;
                    map[ballY + dirY, ballX - 1] = 0;
                    score += 50;
                }

                if (map[ballY + dirY, ballX] > 0 && map[ballY + dirY, ballX] <= 5)
                {
                    map[ballY + dirY, ballX] = 0;
                    map[ballY + dirY, ballX + 1] = 0;
                    score += 50;
                }

                dirY *= -1;
            }
        }

        private void FormArcanoid_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            map[platformY, platformX] = 0;
            map[platformY, platformX + 1] = 0;
            map[platformY, platformX + 2] = 0;
            map[platformY, platformX + 3] = 0;
            map[platformY, platformX + 4] = 0;

            switch (e.KeyCode)
            {
                case Keys.Right:
                    if (platformX < MapWidth - 5)
                    {
                        platformX++;
                    }
                    break;
                case Keys.Left:
                    if (platformX > 0)
                    {
                        platformX--;
                    }
                    break;
            }

            map[platformY, platformX] = 9;
            map[platformY, platformX + 1] = 99;
            map[platformY, platformX + 2] = 99;
            map[platformY, platformX + 3] = 99;
            map[platformY, platformX + 4] = 99;
        }

        private void FormArcanoid_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
        }
    }
}

