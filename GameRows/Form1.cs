using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GameRows
{
    public partial class Form1 : Form
    {
        Button[,] buttons;
        Image[] images;

        Rows rows;

        const string filename = ".png";

        int tk;
        int i;
        string c;
        int record = int.Parse(File.ReadAllText(Rows.filename));
        SoundPlayer sound;

        void Start()
        {
            rows = new Rows(ShowBox, PlaySound);
            CreatePanel();
            InitImages();
            rows.Start();            
        }

        public void ShowBox(int x, int y, int ball)
        {
            buttons[x, y].Image = images[ball];
        }
        public void PlaySound()
        {
            sound = new SoundPlayer("2.wav");
            sound.Play();
        }
        public void Click(Button buttonX)//при нажатии на кнопки
        {
            string name = buttonX.Name;
            int nr = GetNumber(name);
            int x = nr % Rows.SIZE;
            int y = nr / Rows.SIZE;
            Debug.Print($"clicked {name} {x} {y}");
            rows.Click(x, y);
        }        
        public void CreatePanel()
        {
            FlowLayoutPanel flowLayoutPanel1 = new FlowLayoutPanel() //игровая сетка
            {
                Size = new Size(594, 600),
                Location = new Point(10, 10),
                BackColor = Color.Black,                 
            };
            Button button_Refresh = new Button() //кнопка обновления поля
            {
                Text = "",
                BackColor = Color.Black,
                Size = new Size(80, 80),
                Location = new Point(670, 210),                
                Image = new Bitmap(Image.FromFile("refresh.png"), new Size(45, 45)),
            };
            button_Refresh.Click += (sender, args) => { rows.Start(); };

            Button button_Pause = new Button() //пауза
            {
                Text = "",
                BackColor = Color.Black,
                Size = new Size(80, 80),
                Location = new Point(670, 310),
                Image = new Bitmap(Image.FromFile("pause.png"), new Size(45,45)),
            };
            bool paused = false;
            button_Pause.Click += (sender, args) =>
            { 
                if (paused)
                {
                    paused = false;
                    timer1.Start();
                    flowLayoutPanel1.Enabled = true;
                    button_Refresh.Enabled = true;
                    button_Pause.Image = new Bitmap(Image.FromFile("pause.png"), new Size(45, 45));
                }
                else
                {
                    paused = true;
                    timer1.Stop();
                    flowLayoutPanel1.Enabled = false;
                    button_Refresh.Enabled = false;
                    button_Pause.Image = new Bitmap(Image.FromFile("play.png"), new Size(45, 45));
                }
            };

            Button button_SoundOff = new Button() //мут
            {
                Text = "",
                BackColor = Color.Black,
                Size = new Size(80, 80),
                Location = new Point(670, 410),
                Image = new Bitmap(Image.FromFile("sound-off.png"), new Size(45, 45)),
            };
            bool sound_bool = false;
            button_SoundOff.Click += (sender, args) => 
            { 
                if (sound_bool)
                {
                    sound_bool = false;
                    button_SoundOff.Image = new Bitmap(Image.FromFile("sound-off.png"), new Size(45, 45));
                }
                else
                {
                    sound_bool = true;
                    button_SoundOff.Image = new Bitmap(Image.FromFile("sound-on.png"), new Size(45, 45));
                }
            };

            Button button_Exit = new Button() //выход
            {
                Text = "",
                BackColor = Color.Black,
                Size = new Size(80, 80),
                Location = new Point(670, 510),
                Image = new Bitmap(Image.FromFile("exit.png"), new Size(45, 45)),
            };
            button_Exit.Click += (sender, args) => { Application.Exit(); };

            Label label_time = new Label() //время
            {
                Text = c,
                BackColor = Color.Black,
                Size = new Size(140, 65),
                Location = new Point(640, 15),
                Font = new Font(FontFamily.GenericSansSerif, 35),
                ForeColor = Color.White,
            };

            Panel panel_GameOver = new Panel() //панель конца игры
            {
                BackColor = Color.Black,
                Size = new Size(450, 320),
                Location = new Point(180, 120),
            };

            Label label_score2 = new Label() //счет в конце игры
            {
                Text = $"Игра окончена!\r\n    Счет: {rows.score}",
                BackColor = Color.Transparent,
                Size = new Size(400, 130),
                Location = new Point(90, 10),
                ForeColor = Color.White,
                Font = new Font(FontFamily.GenericSansSerif, 28),
            };
            
            Label label_score1 = new Label() //счет в процессе игры
            {
                Text = $"Текущий счет:\r\n{rows.score}\r\nЛучший счет:\r\n{record}",
                BackColor = Color.Transparent,
                Size = new Size(180, 120),
                Location = new Point(610, 90),
                ForeColor = Color.White,
                Font = new Font(FontFamily.GenericSansSerif, 18),
            };

            Button button_Replay = new Button() //кнопка новой игры
            {
                Text = "Играть снова",
                BackColor = Color.Black,
                Size = new Size(200, 120),
                Location = new Point(130, 160),
                ForeColor = Color.White,
                Font = new Font(FontFamily.GenericSansSerif, 28),
            };
            button_Replay.Click += (sender, args) => { Application.Restart(); };

            Button button_Play = new Button() //кнопка в начале игры
            {
                Text = "Играть",
                BackColor = Color.Black,
                Size = new Size(300,200),
                Location = new Point(250,180),
                Font = new Font(FontFamily.GenericSansSerif, 39),
                ForeColor = Color.White,
            };
            button_Play.Click += (sender, args) =>
            {
                flowLayoutPanel1.Visible = true;
                label_time.Visible = true;
                button_Pause.Visible = true;
                button_Play.Visible = true;
                button_Refresh.Visible = true;
                button_SoundOff.Visible = true;
                button_Exit.Visible = true;
                label_score1.Visible = true;
                panel_GameOver.Visible = false;
                button_Play.Visible = false;
                button_Replay.Visible = false;                
                label_score2.Visible = false;

                i = 60;
                c = "00:60";
                label_time.Text = c;
                timer1.Interval = 1000;
                timer1.Enabled = true;
                timer1.Start();
            };

            timer1.Tick += (sender, args) =>
            {
                tk = --i;
                TimeSpan span = TimeSpan.FromMinutes(tk);
                string label = span.ToString("hh':'mm");
                label_time.Text = label.ToString();
                label_score1.Text = $"Текущий счет:\r\n{rows.score}\r\nЛучший счет:\r\n{record}";
                label_score2.Text = $"Игра окончена!\r\n     Счет: {rows.score}";
                if (i < 11)
                    label_time.ForeColor = Color.Red;
                if (i < 0)
                {
                    timer1.Stop();              
                    button_Pause.Image = new Bitmap(Image.FromFile("pause.png"), new Size(45, 45));
                    flowLayoutPanel1.Enabled = false;
                    button_Refresh.Enabled = false;
                    button_Pause.Enabled = false;
                    panel_GameOver.Visible = true;
                    label_score2.Visible = true;
                    button_Replay.Visible = true;
                    SoundPlayer sound_GameOver = new SoundPlayer("4.wav");
                    sound_GameOver.Play();
                    if (isNewRecord())
                    {
                        label_score2.Text = $"Игра окончена!\r\nНовый рекорд!\r\n      Счет: {rows.score}";
                    }
                }                 
            };

            panel_GameOver.Controls.Add(label_score2);
            panel_GameOver.Controls.Add(button_Replay);
            Controls.Add(button_Exit);
            Controls.Add(panel_GameOver);
            Controls.Add(label_score1);
            Controls.Add(button_Play);
            Controls.Add(label_time);
            Controls.Add(button_SoundOff);
            Controls.Add(button_Pause);
            Controls.Add(button_Refresh);
            Controls.Add(flowLayoutPanel1);
            InitButtons(flowLayoutPanel1);

            button_Replay.Visible = false;
            label_score2.Visible = false;
            panel_GameOver.Visible = false;
            flowLayoutPanel1.Visible = false;
            button_SoundOff.Visible = false;
            button_Refresh.Visible = false;
            button_Pause.Visible = false;
            button_Exit.Visible = true;
            label_time.Visible = false;
            label_score1.Visible = false;
            button_Play.Visible = true;

        }
        internal void InitButtons(FlowLayoutPanel flowLayoutPanel1)
        {
            buttons = new Button[Rows.SIZE, Rows.SIZE];
            for (int nr = 0; nr < Rows.SIZE * Rows.SIZE; nr++)
            {
                Button button = new Button()
                {
                    Name = $"button({nr})",
                    Text = "",
                    BackColor = Color.Black,                  
                    Size = new Size(60, 60),
                };
                buttons[nr % Rows.SIZE, nr / Rows.SIZE] = button;
                button.Click += (sender, args) => { Click( button); };
                flowLayoutPanel1.Controls.Add(button);
            }            
        }        
        private void InitImages()
        {
            images = new Image[Rows.BALLS];
            for (int j = 0; j < Rows.BALLS; j++)
            {
                Bitmap bitmap = new Bitmap(Image.FromFile
                    (j + filename), new Size(44, 44));
                images[j] = bitmap;
            }
        }
        public Form1()
        {
            InitializeComponent();           
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Start();           
        }
        private int GetNumber(string name)
        {
            Regex regex = new Regex(@"(\d+)");
            Match match = regex.Match(name);
            if (!match.Success)
            {
                throw new Exception("Unrecognized object name");
            }
            Group group = match.Groups[1];
            string number = group.Value;
            return Convert.ToInt32(number);
        }        
        private bool isNewRecord()
        {
            if (record < rows.score)
            {
                File.WriteAllText(Rows.filename, string.Empty);
                File.WriteAllText(Rows.filename, rows.score.ToString());
                return true;
            }
            return false;
        }
    }
}
