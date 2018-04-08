using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

using LMDMono2D; // DotMath import

namespace DarkEcho
{
    #region public class Application
    public class MainApp 
    {
        public static ApplicationForm AppForm;
        public static Timer GameTimer;
        public static PictureBox GameScreen;
        public static Bitmap Screen;
        public static Graphics GScreen;

        private static Timer FPSTimer;
        private static Int32 FPSNow = 0;
        private static Int32 FPS = 0;

        #region public static void Main()
        public static void Main()
        {
            AppForm = new ApplicationForm();
            ElementsInit();
            Application.Run(AppForm);
        }
        #endregion
        #region private static void ElementsInit()
        private static void ElementsInit()
        {
            // AppForm
            AppForm.Width = 800;
            AppForm.Height = 500;
            AppForm.FormBorderStyle = FormBorderStyle.FixedSingle;
            AppForm.MaximizeBox = false;
            AppForm.ShowIcon = false;
            AppForm.Text = "DarkEcho";

            AppForm.KeyPreview = true;
            AppForm.KeyDown += OnKeyDown;
            AppForm.KeyUp += OnKeyUp;

            // OnGameStart()
            AppForm.OnFrameStart();

            // GameTimer
            GameTimer = new Timer();
            GameTimer.Interval = 1000 / 40; // 30 FPS
            GameTimer.Tick += OnGameTimerTick;
            GameTimer.Start();

            // FPSTimer
            FPSTimer = new Timer();
            FPSTimer.Interval = 1000;
            FPSTimer.Tick += OnFPSTimerTick;
            FPSTimer.Start();

            // GameScreen
            GameScreen = new PictureBox();
            GameScreen.Parent = AppForm;
            GameScreen.Width = AppForm.Width;
            GameScreen.Height = AppForm.Height;
            GameScreen.BackColor = Color.Black;

            // OnAppFormMouseMove
            GameScreen.MouseMove += OnAppFormMouseMove;
            // OnAppFormMouseDown
            GameScreen.MouseDown += OnAppFormMouseDown;
            // OnAppFormMouseUp
            GameScreen.MouseUp += OnAppFormMouseUp;

            // Screen
            Screen = new Bitmap(GameScreen.Width, GameScreen.Height);

            // GScreen
            GScreen = Graphics.FromImage(Screen);
            GScreen.SmoothingMode = SmoothingMode.HighQuality;
        }
        #endregion

        #region private static void OnGameTimerTick(Object sender, EventArgs e)
        private static void OnGameTimerTick(Object sender, EventArgs e)
        {
            GScreen.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0 , 0)), 0, 0, GameScreen.Width, GameScreen.Height);
            AppForm.OnFrameTick(GScreen);
            GameScreen.Image = Screen;
            FPSNow++;
        }
        #endregion
        #region private static void OnAppFormMouseMove(Object sender, EventArgs e)
        private static void OnAppFormMouseMove(Object sender, MouseEventArgs e)
        {
            AppForm.MPos = new Point(e.X, e.Y);
        }
        #endregion
        #region private static void OnAppFormMouseDown(Object sender, EventArgs e)
        private static void OnAppFormMouseDown(Object sender, MouseEventArgs e)
        {
            AppForm.MPos = new Point(e.X, e.Y);
            AppForm.MDown = true;
        }
        #endregion
        #region private static void OnAppFormMouseUp(Object sender, EventArgs e)
        private static void OnAppFormMouseUp(Object sender, MouseEventArgs e)
        {
            AppForm.MPos = new Point(e.X, e.Y);
            AppForm.MDown = false;
        }
        #endregion

        #region private static void OnKeyDown(Object sender, KeyEventArgs e)
        private static void OnKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) { AppForm.Keys[0] = true; }
            if (e.KeyCode == Keys.S) { AppForm.Keys[1] = true; }
            if (e.KeyCode == Keys.A) { AppForm.Keys[2] = true; }
            if (e.KeyCode == Keys.D) { AppForm.Keys[3] = true; }
            if (e.KeyCode == Keys.Space) { AppForm.Keys[4] = true; }
            if (e.KeyCode == Keys.E) { AppForm.Keys[5] = true; }
        }
        #endregion
        #region private static void OnKeyUp(Object sender, KeyEventArgs e)
        private static void OnKeyUp(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) { AppForm.Keys[0] = false; }
            if (e.KeyCode == Keys.S) { AppForm.Keys[1] = false; }
            if (e.KeyCode == Keys.A) { AppForm.Keys[2] = false; }
            if (e.KeyCode == Keys.D) { AppForm.Keys[3] = false; }
            if (e.KeyCode == Keys.Space) { AppForm.Keys[4] = false; }
            if (e.KeyCode == Keys.E) { AppForm.Keys[5] = false; }
        }
        #endregion

        #region private static void OnFPSTimerTick(Object sender, EventArgs e)
        private static void OnFPSTimerTick(Object sender, EventArgs e)
        {
            FPS = FPSNow;
            FPSNow = 0;
            AppForm.FPS = FPS;
        }
        #endregion
    }
    #endregion
    #region public class ApplicationForm : Form
    public class ApplicationForm : Form
    {
        public Point MPos = new Point(0, 0);
        public Boolean MDown = false;
        public Boolean[] Keys = new Boolean[6]; // up down left right echo WallVisible
        public Int32 FPS = 0;


        float PReload = 1f;
        float ReloadSpeed = 0.01f;
        Dot PPos = new Dot();
        float PSpeed = 2f;
        List<SoundBall> SBalls = new List<SoundBall>();
        Dot WinPos = new Dot(200f, 200f);
        float WinRadius = 50f;
        List<Dot> Walls = new List<Dot>();

        #region public void OnFrameStart()
        public void OnFrameStart()
        {
            String[] lvl = System.IO.File.ReadAllText("LEVEL.TXT").Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            String[] param = lvl[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            WinPos.X = (float)Convert.ToDouble(param[0]);
            WinPos.Y = (float)Convert.ToDouble(param[1]);
            WinRadius = (float)Convert.ToDouble(param[2]);

            for (int i = 1; i < lvl.Length; i++)
            {
                param = lvl[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Walls.Add(new Dot((float)Convert.ToDouble(param[0]), (float)Convert.ToDouble(param[1])));
                Walls.Add(new Dot((float)Convert.ToDouble(param[2]), (float)Convert.ToDouble(param[3])));
            }
        }
        #endregion
        #region public void OnFrameTick(Graphics g)
        public void OnFrameTick(Graphics g)
        {
            Dot DCenter = new Dot(this.Width / 2f, this.Height / 2f);
            if (PReload * 64 >= 16)
            {
                g.FillEllipse(new SolidBrush(Color.FromArgb((int)(255 * PReload), 255, 255, 255)), DCenter.X - (10 * PReload), DCenter.Y - (10 * PReload), (20 * PReload), (20 * PReload));
            }
            g.DrawEllipse(new Pen(Color.White, 2), DCenter.X - 10, DCenter.Y - 10, 20, 20);

            #region PlayerMove
            if (Keys[0] == true)
            {
                PPos.Y -= PSpeed;
            }
            if (Keys[1] == true)
            {
                PPos.Y += PSpeed;
            }
            if (Keys[2] == true)
            {
                PPos.X -= PSpeed;
            }
            if (Keys[3] == true)
            {
                PPos.X += PSpeed;
            }
            #endregion
            #region Добавление звуковых шаров при клике
            if (Keys[4] == true)
            {
                Keys[4] = false;
                Int32 n = (int)(64 * PReload);
                if (n >= 16)
                {
                    PReload = 0f;
                    for (int i = 0; i < n; i++)
                    {
                        Dot vect = new Dot();
                        vect.R = 3f;
                        vect.A = (float)Math.PI * 2 / n * i;
                        SBalls.Add(new SoundBall(PPos, vect, 100f));
                    }
                }
            }
            #endregion
            #region отрисовка звуковых шаров
            for (int i = 0; i < SBalls.Count; i++)
            {
                SBalls[i].Step();
                float coi = (float)Math.Max(SBalls[i].LifeTime / SBalls[i].MaxLifeTime, 0);
                g.FillEllipse(new SolidBrush(Color.FromArgb((int)(coi * 255), 255, 255, 255)), SBalls[i].Pos.X - (int)(10 * coi) - PPos.X + DCenter.X, SBalls[i].Pos.Y - (int)(10 * coi) - PPos.Y + DCenter.Y, (int)(20 * coi), (int)(20 * coi));

                Dot C = SBalls[i].Pos;
                for (int j = 0; j < Walls.Count / 2; j++)
                {
                    if (DotMath.Distance(DotMath.DotClosestSegment(Walls[j * 2], Walls[j * 2 + 1], C), C) <= 10f)
                    {
                        //SBalls[i].ToDel = true;
                        Dot N = (DotMath.DotClosestSegment(Walls[j * 2], Walls[j * 2 + 1], C) - SBalls[i].Pos).GetUnitVector();
                        SBalls[i].Vect -= N;
                    }
                }
            }
            #endregion
            #region удаление звуковых шаров
            for (int i = 0; i < SBalls.Count; i++)
            {
                if (SBalls[i].LifeTime <= 0f || SBalls[i].ToDel == true)
                {
                    SBalls.RemoveAt(i);
                }
            }
            #endregion
            #region Отталкивание от стен
            for (int i = 0; i < Walls.Count / 2; i++)
            {
                Dot DCS = DotMath.DotClosestSegment(Walls[i * 2], Walls[i * 2 + 1], PPos);
                if (DotMath.Distance(DCS, PPos) <= 10f)
                {

                    Dot back = (DCS - PPos).GetUnitVector() * PSpeed * 2f;
                    PPos -= back;
                } 
            }
            #endregion

            PReload += ReloadSpeed;
            if (PReload > 1f) { PReload = 1f; }

            if (Keys[5] == true)
            {
                for (int j = 0; j < Walls.Count / 2; j++)
                {
                    g.DrawLine(new Pen(Color.White, 1), Walls[j * 2] - PPos + DCenter, Walls[j * 2 + 1] - PPos + DCenter);
                }
            }

            Dot pointer = (WinPos - PPos).GetUnitVector() * 14f + DCenter;
            g.FillEllipse(new SolidBrush(Color.Lime), pointer.X - 2, pointer.Y - 2, 4, 4);

            g.FillEllipse(new SolidBrush(Color.FromArgb(10, 0, 255, 0)), WinPos.X - WinRadius - PPos.X + DCenter.X, WinPos.Y - WinRadius - PPos.Y + DCenter.Y, WinRadius * 2f, WinRadius * 2f);
            float szc = (float)Math.Abs(Math.Sin(Environment.TickCount % 2000 / 1000f));
            g.FillEllipse(new SolidBrush(Color.FromArgb((int)(20f * szc), 100, 255, 100)), (float)(WinPos.X - WinRadius * szc - PPos.X + DCenter.X), (float)(WinPos.Y - WinRadius * szc - PPos.Y + DCenter.Y), (float)(WinRadius * szc * 2f), (float)(WinRadius * szc * 2f));

            g.DrawString(FPS.ToString(), new Font("arial", 10), new SolidBrush(Color.White), 0, 0);
        }
        #endregion
    }
    #endregion
}
