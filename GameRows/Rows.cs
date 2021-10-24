using System;
using System.Media;

public delegate void ShowBox(int x, int y, int ball);
public delegate void PlaySound();

namespace GameRows
{
    class Rows
    {
        public const int SIZE = 9; //размерность массива кнопок
        public const int BALLS = 7; //кол-во картинок шариков
        const int ADD_BALLS = 81;
        public int score = 0;
        public const string filename = "scores.txt";

        ShowBox showBox;
        PlaySound playSound;
        Random random = new Random();

        int[,] map;
        int fromX, fromY;
        int toX, toY;
        bool isBoolTaken;
        bool canCut = true;
        internal Rows(ShowBox showBox, PlaySound playSound)
        {
            this.showBox = showBox;
            this.playSound = playSound;
            map = new int[SIZE, SIZE];
        }
        public void Start()
        {
            ClearMap();
            AddAllBalls();
            CutBalls();
            isBoolTaken = false;
            PlayMusic();
        }
        public void Click(int x, int y)
        {
            if (isBoolTaken)       //если шар взят
                MoveBall(x, y);    //то его передвинуть
            else TakeBall(x, y);   //иначе взять
        }
        private void TakeBall(int x, int y)
        {
            fromX = x;
            fromY = y;
            isBoolTaken = true;
        }
        private void MoveBall(int x, int y)
        {
            toX = x;
            toY = y;
            if (!isBoolTaken) return;
            if (!CanMove(toX, toY)) return;
            ChangeBalls(fromX, fromY, toX, toY);
            isBoolTaken = false;
            if (!CutBalls())
            {
                AddRandomBalls();
                CutBalls();
                ChangeBalls(toX, toY, fromX, fromY);
            }
        }
        private void ChangeBalls(int x1, int y1, int x2, int y2)
        {
            int temp;
            temp = map[x2, y2];
            SetMap(x2, y2, map[x1, y1]);
            SetMap(x1, y1, temp);
        }
        private void ClearMap()
        {
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    SetMap(x, y, 0);  //задает полю пустые картинки (ball==0)
                }
            }
        }
        private void SetMap(int x, int y, int ball) //задает полю картинки
        {
            map[x, y] = ball;                      //задаем
            showBox(x, y, ball);                   //и показываем
        }
        private bool OnMap(int x, int y)
        {
            return x >= 0 && x < SIZE &&  //x и y в пределах от 0 до SIZE
                   y >= 0 && y < SIZE;
        }
        private int GetMap(int x, int y) //проверка чтобы не выйти за пределы карты
        {
            if (!OnMap(x, y)) return 0; //если есть такая координата
            return map[x, y];
        }
        private void AddRandomBalls()
        {
            for (int j = 0; j < ADD_BALLS; j++)
            {
                AddRandomBall();
            }
        }
        private void AddRandomBall()
        {
            int x, y;
            int loop = SIZE * SIZE;
            do
            {
                x = random.Next(SIZE);               //присваивать рандомные коррдинаты
                y = random.Next(SIZE);
                if (--loop <= 0) return;             //если нет места, то выход
            } while (map[x, y] > 0);                 //пока не будут заняты все ячейки
            int ball = 1 + random.Next(BALLS - 1);   //устанавливаем цвет шарика
            SetMap(x, y, ball);
        }
        private void AddAllBalls()
        {
            for (int nr = 0; nr < SIZE * SIZE; nr++)
            {
                map[nr % SIZE, nr / SIZE] = 1 + random.Next(BALLS - 1);
                SetMap(nr % SIZE, nr / SIZE, map[nr % SIZE, nr / SIZE]);
            }
        }
        private bool[,] mark; //для хранения координат тех шариков, что нужно удалить
        private bool CutBalls()
        {
            int balls = 0;
            mark = new bool[SIZE, SIZE];
            //ищем шарики
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    balls += CutBall(x, y, 1, 0); //вправо
                    balls += CutBall(x, y, 0, 1); //вниз
                }
            }
            if (balls > 0) //если найденных шариков больше 0
            {
                playSound();
                for (int x = 0; x < SIZE; x++)
                {
                    for (int y = 0; y < SIZE; y++)
                    {
                        if (mark[x, y]) 
                        {
                            SetMap(x, y, 0); //идем по карте и стриаем записанные шарики
                            score++;
                        }                        
                    }
                }
                
                AddRandomBalls();
                CutBalls();
                return true;
            }
            return false;
        }
        private int CutBall(int x0, int y0, int sx, int sy)
        {
            int ball = map[x0, y0]; //запоминаем картинку шарика
            if (ball == 0) return 0;  //если она пустая - выходим
            int count = 0;
            for (int x = x0, y = y0; GetMap(x, y) == ball; x += sx, y += sy)
            {                //движемся вправо и вниз от x0 и y0, не выходим за карту
                count++; //считаем
            }
            if (count < 3) return 0; //нашли меньше трех - не подходит
            for (int x = x0, y = y0; GetMap(x, y) == ball; x += sx, y += sy)
            {                //движемся вправо и вниз от x0 и y0, не выходим за карту
                mark[x, y] = true;  //записываем
            }
            return count; //нашли больше трех - выводим количество
        }
        private bool CanMove(int toX, int toY)
        {
            if (Math.Abs(toX - fromX) == 1 && fromY == toY ||
                Math.Abs(toY - fromY) == 1 && fromX == toX)
            {
                if (canCut) return true;
                else return false;
            }
            else
            {
                isBoolTaken = false;
                return false;
            }
        }
        public void PlayMusic()
        {
            //SoundPlayer sound_main = new SoundPlayer("12.wav");
            //sound_main.PlayLooping();
        }
    }
}
