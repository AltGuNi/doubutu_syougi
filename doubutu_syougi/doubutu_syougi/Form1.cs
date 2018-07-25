
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace doubutu_syougi
{
    enum koma
    {
        Lion1 = 1, Lion2, Kirin1, Kirin2, Zou1, Zou2,
        Hiyoko1, Hiyoko2, Niwatori1, Niwatori2
    };

    enum POINT
    {
        Piece = 100
    };
    public partial class Form1 : Form
    {
        private int GAME_MODE = 0; // 0:対人 1:コンピュータ 
        private int[,] position = new int[7, 4]; //現在の各盤面の状況
        private int[] team = new int[11]; //各駒が先後どちらのものか
        private int[] piece_x = new int[9] { 0, 1, 1, 2, 0, 0, 2, 1, 1 }; //現在の各駒の位置
        private int[] piece_y = new int[9] { 0, 3, 0, 3, 0, 3, 0, 2, 1 };
        private int[,] have = new int[300, 3];
        private int[] bord_x = new int[3] { 235, 350, 461 };
        private int[] bord_y = new int[4] { 79, 190, 302, 414 };
        private int[] niwatori_flag = new int[2] { 0, 0 };
        private int[] moves = new int[300];
        private int[] niwatori_changed = new int[300];
        private int[, ,] position_turn = new int[300, 3, 4];
        private int[,] win_results = new int[414, 20];
        private int turn = 1;
        private int turn_side = 1;
        private int hand_from = 99, hand_to, hand_pice;
        private int move_count = 1, end = 0;
        private System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));


        public Form1()
        {
            InitializeComponent();
            hiyoko2.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
            zou2.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
            kirin2.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
            lion2.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);

            for (int i = 0; i < 7; i++) //int koma[7,4] の初期化
            {
                for (int j = 0; j < 4; j++)
                {
                    position[i, j] = 0; // 0 : 駒がないことを示す
                }
            }

            team[0] = 0;
            for (int i = 1; i <= 10; i++)
            {
                if (i % 2 == 1)
                {
                    team[i] = 1;
                }
                else
                {
                    team[i] = -1;
                }
            }

            for (int i = 0; i < 300; i++)
            {
                moves[i] = 0;
                niwatori_changed[i] = 0;
                for (int j = 0; j < 3; j++)
                {
                    have[i, j] = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        position_turn[i, j, k] = 0;
                    }
                }
            }

            position[1, 3] = (int)koma.Lion1; // 先手のライオン : 1
            position[1, 0] = (int)koma.Lion2; // 後手のライオン : 2
            position[2, 3] = (int)koma.Kirin1;//先手のキリン : 3
            position[0, 0] = (int)koma.Kirin2;//後手のキリン : 4
            position[0, 3] = (int)koma.Zou1;  //先手のゾウ : 5
            position[2, 0] = (int)koma.Zou2;  //後手のゾウ : 6
            position[1, 2] = (int)koma.Hiyoko1;//先手のヒヨコ : 7
            position[1, 1] = (int)koma.Hiyoko2;//後手のヒヨコ : 8

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    position_turn[move_count, i, j] = position[i, j];
                }
            }

            textBox2.Text += "[先手]";
            textBox3.Text += "未選択";

            System.IO.StreamReader cReader = (
                new System.IO.StreamReader(@"../../\Resources\win_results.txt", System.Text.Encoding.Default)
            );
            int read_point = 0;
            while (cReader.Peek() >= 0)
            {
                string stBuffer = cReader.ReadLine();
                for (int i = 0; i < 20; i++)
                {
                    char read = stBuffer[i];
                    win_results[read_point, i] = int.Parse(read.ToString());
                }
                read_point++;
            }
            cReader.Close();
        }


        void turn_text(int turn)
        {
            if (end == 1)
            {
                turn *= -1;
            }
            if (turn > 0)
            {
                textBox2.Text = string.Empty;
                textBox2.Text += "[先手]";
            }
            else
            {
                textBox2.Text = string.Empty;
                textBox2.Text += "[後手]";
            }
        }

        void select_text(int from)
        {
            if (end == 1)
            {
                textBox3.Text = string.Empty;
                textBox3.Text += "の勝ち！";
            }
            else if (from != 99)
            {
                textBox3.Text = string.Empty;
                textBox3.Text += "選択中";
            }
            else
            {
                textBox3.Text = string.Empty;
                textBox3.Text += "未選択";
            }
        }


        int click_place(int x, int y)
        {
            int point = 0;

            Point cp = this.PointToClient(new Point(x, y));
            cp.Y += 20;

            if (cp.X >= 230 && cp.X < 345)
            {
                point = 0;
            }
            else if (cp.X >= 345 && cp.X < 458)
            {
                point = 10;
            }
            else if (cp.X >= 458 && cp.X < 570)
            {
                point = 20;
            }
            else if (cp.X >= 615 && cp.X < 718)
            {
                point = 30;
            }
            else if (cp.X >= 80 && cp.X < 182)
            {
                point = 50;
            }
            else
            {
                point = 99;
            }

            if (cp.Y >= 95 && cp.Y < 210)
            {
                point += 0;
            }
            else if (cp.Y >= 210 && cp.Y < 320)
            {
                point += 1;
            }
            else if (cp.Y >= 320 && cp.Y < 432)
            {
                point += 2;
            }
            else if (cp.Y >= 432 && cp.Y < 545)
            {
                point += 3;
            }
            else if (cp.Y == 30 || cp.Y == 50)
            {
                point += 5;
            }
            else
            {
                point = 99;
            }
            return point;

        }


        void move_piece(int from, int to, int piece)
        {
            int move;
            if (turn == team[piece])
            {
                if (from < 30)
                {
                    if (to - from == -1) // 上移動
                    {
                        move = position[to / 10, to % 10];
                        if (team[piece] != team[move])
                        {
                            switch (piece)
                            {
                                case 1:
                                    this.lion1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 100 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 2:
                                    this.lion2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 200 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 3:
                                    this.kirin1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 300 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 4:
                                    this.kirin2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 400 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 7:
                                    if (team[piece] > 0 || niwatori_flag[0] == 1)
                                    {
                                        this.hiyoko1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 700 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                        if (to % 10 == 0 && niwatori_flag[0] != 1)
                                        {
                                            niwatori_flag[0] = 1;
                                            this.hiyoko1.Image = ((System.Drawing.Image)(resources.GetObject("niwatori1.Image")));
                                            niwatori_changed[move_count] += 1;
                                        }
                                    }
                                    break;
                                case 8:
                                    if (team[piece] > 0 || niwatori_flag[1] == 1)
                                    {
                                        this.hiyoko2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 800 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                        if (to % 10 == 0 && niwatori_flag[1] != 1)
                                        {
                                            niwatori_flag[1] = 1;
                                            this.hiyoko2.Image = ((System.Drawing.Image)(resources.GetObject("niwatori1.Image")));
                                            niwatori_changed[move_count] += 2;
                                        }
                                    }
                                    break;
                                default:
                                    return;
                            }
                        }
                    }
                    else if (to - from == 1)  //下移動
                    {
                        move = position[to / 10, to % 10];
                        if (team[piece] != team[move])
                        {
                            switch (piece)
                            {
                                case 1:
                                    this.lion1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 100 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 2:
                                    this.lion2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 200 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 3:
                                    this.kirin1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 300 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 4:
                                    this.kirin2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 400 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 7:
                                    if (team[piece] < 0 || niwatori_flag[0] == 1)
                                    {
                                        this.hiyoko1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 700 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                        if (to % 10 == 3 && niwatori_flag[0] != 1)
                                        {
                                            niwatori_flag[0] = 1;
                                            this.hiyoko1.Image = ((System.Drawing.Image)(resources.GetObject("niwatori1.Image")));
                                            hiyoko1.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                            niwatori_changed[move_count] += 1;
                                        }
                                    }
                                    break;
                                case 8:
                                    if (team[piece] < 0 || niwatori_flag[1] == 1)
                                    {
                                        this.hiyoko2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 800 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                        if (to % 10 == 3 && niwatori_flag[1] != 1)
                                        {
                                            niwatori_flag[1] = 1;
                                            this.hiyoko2.Image = ((System.Drawing.Image)(resources.GetObject("niwatori1.Image")));
                                            hiyoko2.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                            niwatori_changed[move_count] += 2;
                                        }
                                    }
                                    break;
                                default:
                                    return;
                            }
                        }
                    }
                    else if (to - from == 10)  //右移動
                    {
                        move = position[to / 10, to % 10];
                        if (team[piece] != team[move])
                        {
                            switch (piece)
                            {
                                case 1:
                                    this.lion1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 100 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 2:
                                    this.lion2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 200 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 3:
                                    this.kirin1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 300 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 4:
                                    this.kirin2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 400 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 7:
                                    if (niwatori_flag[0] == 1)
                                    {
                                        this.hiyoko1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 700 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                    }
                                    break;
                                case 8:
                                    if (niwatori_flag[1] == 1)
                                    {
                                        this.hiyoko2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 800 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                    }
                                    break;
                                default:
                                    return; ;
                            }
                        }
                    }
                    else if (to - from == -10)  //左移動
                    {
                        move = position[to / 10, to % 10];
                        if (team[piece] != team[move])
                        {
                            switch (piece)
                            {
                                case 1:
                                    this.lion1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 100 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 2:
                                    this.lion2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 200 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 3:
                                    this.kirin1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 300 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 4:
                                    this.kirin2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 400 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 7:
                                    if (niwatori_flag[0] == 1)
                                    {
                                        this.hiyoko1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 700 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                    }
                                    break;
                                case 8:
                                    if (niwatori_flag[1] == 1)
                                    {
                                        this.hiyoko2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 800 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                    }
                                    break;
                                default:
                                    return;
                            }
                        }
                    }
                    else if (to - from == 9)  //右上移動
                    {
                        move = position[to / 10, to % 10];
                        if (team[piece] != team[move])
                        {
                            switch (piece)
                            {
                                case 1:
                                    this.lion1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 100 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 2:
                                    this.lion2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 200 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 5:
                                    this.zou1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 500 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 6:
                                    this.zou2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 600 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 7:
                                    if (team[piece] > 0 && niwatori_flag[0] == 1)
                                    {
                                        this.hiyoko1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 700 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                    }
                                    break;
                                case 8:
                                    if (team[piece] < 0 && niwatori_flag[1] == 1)
                                    {
                                        this.hiyoko2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 800 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                    }
                                    break;
                                default:
                                    return;
                            }
                        }
                    }
                    else if (to - from == -11)  //左上移動
                    {
                        move = position[to / 10, to % 10];
                        if (team[piece] != team[move])
                        {
                            switch (piece)
                            {
                                case 1:
                                    this.lion1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 100 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 2:
                                    this.lion2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 200 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 5:
                                    this.zou1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 500 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 6:
                                    this.zou2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 600 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 7:
                                    if (team[piece] > 0 && niwatori_flag[0] == 1)
                                    {
                                        this.hiyoko1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 700 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                    }
                                    break;
                                case 8:
                                    if (team[piece] > 0 && niwatori_flag[1] == 1)
                                    {
                                        this.hiyoko2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 800 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                    }
                                    break;
                                default:
                                    return;
                            }
                        }
                    }
                    else if (to - from == 11)  //右下移動
                    {
                        move = position[to / 10, to % 10];
                        if (team[piece] != team[move])
                        {
                            switch (piece)
                            {
                                case 1:
                                    this.lion1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 100 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 2:
                                    this.lion2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 200 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 5:
                                    this.zou1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 500 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 6:
                                    this.zou2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 600 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 7:
                                    if (team[piece] < 0 && niwatori_flag[0] == 1)
                                    {
                                        this.hiyoko1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 700 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                    }
                                    break;
                                case 8:
                                    if (team[piece] < 0 && niwatori_flag[1] == 1)
                                    {
                                        this.hiyoko2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 800 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                    }
                                    break;
                                default:
                                    return;
                            }
                        }
                    }
                    else if (to - from == -9)  //左下移動
                    {
                        move = position[to / 10, to % 10];
                        if (team[piece] != team[move])
                        {
                            switch (piece)
                            {
                                case 1:
                                    this.lion1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 100 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 2:
                                    this.lion2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 200 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 5:
                                    this.zou1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 500 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 6:
                                    this.zou2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    if (position[to / 10, to % 10] != 0)
                                    {
                                        get_piece(to);
                                    }
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    moves[move_count] = 600 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                    break;
                                case 7:
                                    if (team[piece] < 0 && niwatori_flag[0] == 1)
                                    {
                                        this.hiyoko1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 700 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                    }
                                    break;
                                case 8:
                                    if (team[piece] < 0 && niwatori_flag[1] == 1)
                                    {
                                        this.hiyoko2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                        if (position[to / 10, to % 10] != 0)
                                        {
                                            get_piece(to);
                                        }
                                        position[to / 10, to % 10] = piece;
                                        position[from / 10, from % 10] = 0;
                                        piece_x[piece] = to / 10;
                                        piece_y[piece] = to % 10;
                                        moves[move_count] = 800 + to;
                                        move_count++;
                                        turn *= -1;
                                        turn_text(turn);
                                    }
                                    break;
                                default:
                                    return;
                            }
                        }
                    }
                }
                else
                {
                    move = position[to / 10, to % 10];
                    if (move == 0)
                    {
                        switch (piece)
                        {
                            case 1:
                                this.lion1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                position[to / 10, to % 10] = piece;
                                position[from / 10, from % 10] = 0;
                                piece_x[piece] = to / 10;
                                piece_y[piece] = to % 10;
                                moves[move_count] = 100 + to;
                                move_count++;
                                turn *= -1;
                                turn_text(turn);
                                break;
                            case 2:
                                this.lion2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                position[to / 10, to % 10] = piece;
                                position[from / 10, from % 10] = 0;
                                piece_x[piece] = to / 10;
                                piece_y[piece] = to % 10;
                                moves[move_count] = 200 + to;
                                move_count++;
                                turn *= -1;
                                turn_text(turn);
                                break;
                            case 3:
                                this.kirin1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                position[to / 10, to % 10] = piece;
                                position[from / 10, from % 10] = 0;
                                piece_x[piece] = to / 10;
                                piece_y[piece] = to % 10;
                                have[move_count, 0] -= (team[3] == 1) ? 3 : 4;
                                moves[move_count] = 300 + to;
                                move_count++;
                                turn *= -1;
                                turn_text(turn);
                                break;
                            case 4:
                                this.kirin2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                position[to / 10, to % 10] = piece;
                                position[from / 10, from % 10] = 0;
                                piece_x[piece] = to / 10;
                                piece_y[piece] = to % 10;
                                have[move_count, 0] -= (team[4] == 1) ? 3 : 4;
                                moves[move_count] = 400 + to;
                                move_count++;
                                turn *= -1;
                                turn_text(turn);
                                break;
                            case 5:
                                this.zou1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                position[to / 10, to % 10] = piece;
                                position[from / 10, from % 10] = 0;
                                piece_x[piece] = to / 10;
                                piece_y[piece] = to % 10;
                                have[move_count, 1] -= (team[5] == 1) ? 3 : 4;
                                moves[move_count] = 500 + to;
                                move_count++;
                                turn *= -1;
                                turn_text(turn);
                                break;
                            case 6:
                                this.zou2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                position[to / 10, to % 10] = piece;
                                position[from / 10, from % 10] = 0;
                                piece_x[piece] = to / 10;
                                piece_y[piece] = to % 10;
                                have[move_count, 1] -= (team[6] == 1) ? 3 : 4;
                                moves[move_count] = 600 + to;
                                move_count++;
                                turn *= -1;
                                turn_text(turn);
                                break;
                            case 7:
                                if (to % 10 == 0 && team[piece] > 0)
                                {
                                    return;
                                }
                                else if (to % 10 == 3 && team[piece] < 0)
                                {
                                    return;
                                }
                                else
                                {
                                    this.hiyoko1.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    have[move_count, 2] -= (team[7] == 1) ? 3 : 4;
                                    moves[move_count] = 700 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                }
                                break;
                            case 8:
                                if (to % 10 == 0 && team[piece] > 0)
                                {
                                    return;
                                }
                                else if (to % 10 == 3 && team[piece] < 0)
                                {
                                    return;
                                }
                                else
                                {
                                    this.hiyoko2.Location = new System.Drawing.Point(bord_x[to / 10], bord_y[to % 10]);
                                    position[to / 10, to % 10] = piece;
                                    position[from / 10, from % 10] = 0;
                                    piece_x[piece] = to / 10;
                                    piece_y[piece] = to % 10;
                                    have[move_count, 2] -= (team[8] == 1) ? 3 : 4;
                                    moves[move_count] = 800 + to;
                                    move_count++;
                                    turn *= -1;
                                    turn_text(turn);
                                }
                                break;
                        }
                    }
                }
                if (piece == 1 && to % 10 == 0 && get_lion(piece, 0, 0, 0) == 0)
                {
                    end = 1;
                    turn_text(turn);
                    using (StreamWriter w = new StreamWriter(@"../../\Resources\record.txt", true))
                    {
                        for (int i = 1; i < move_count; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                for (int k = 0; k < 4; k++)
                                {
                                    w.Write("{0}", position_turn[i, j, k]);
                                }
                            }
                            w.Write("{0}{1}{2}", have[i - 1, 0], have[i - 1, 1], have[i - 1, 2]);
                            w.Write("{0}{1}", niwatori_changed[i], moves[i]);
                            if (i % 2 == 1)
                            {
                                w.WriteLine("1");
                            }
                            else
                            {
                                w.WriteLine("0");
                            }
                        }
                    }

                }
                else if (piece == 2 && to % 10 == 3 && get_lion(piece, 0, 0, 0) == 0)
                {
                    end = 1;
                    turn_text(turn);
                    using (StreamWriter w = new StreamWriter(@"../../\Resources\record.txt", true))
                    {
                        for (int i = 1; i < move_count; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                for (int k = 0; k < 4; k++)
                                {
                                    w.Write("{0}", position_turn[i, j, k]);
                                }
                            }
                            w.Write("{0}{1}{2}", have[i - 1, 0], have[i - 1, 1], have[i - 1, 2]);
                            w.Write("{0}{1}", niwatori_changed[i], moves[i]);
                            if (i % 2 == 1)
                            {
                                w.WriteLine("0");
                            }
                            else
                            {
                                w.WriteLine("1");
                            }
                        }
                    }
                }
            }
        }

        void get_piece(int place)
        {
            int pice, i, j = 0, two = 0;
            pice = position[place / 10, place % 10];
            switch (pice)
            {
                case 1: //終了
                    end = 1;
                    for (i = 1; i <= 2 && position[i + 4, j] != 0; i++)
                    {
                        for (j = 1; j < 4 && position[i + 4, j] != 0; j++) ;
                        if (j == 4)
                        {
                            j = 0; two = 1;
                        }
                    }
                    if (i != 1) i = i - 1;
                    position[i + two + 4, j] = pice;
                    this.lion1.Location = new System.Drawing.Point(160 - (i + two) * 80, 79 + (j * 105));
                    lion1.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    kirin1.Refresh();
                    piece_x[pice] = i + 4;
                    piece_y[pice] = j;
                    team[pice] *= -1;
                    j = 0;
                    two = 0;
                    moves[move_count] = position[place / 10, place % 10] * 100 + place;
                    using (StreamWriter w = new StreamWriter(@"../../\Resources\record.txt", true))
                    {
                        for (i = 1; i <= move_count; i++)
                        {
                            for (j = 0; j < 3; j++)
                            {
                                for (int k = 0; k < 4; k++)
                                {
                                    w.Write("{0}", position_turn[i, j, k]);
                                }
                            }
                            w.Write("{0}{1}{2}", have[i - 1, 0], have[i - 1, 1], have[i - 1, 2]);
                            w.Write("{0}{1}", niwatori_changed[i], moves[i]);
                            if (i % 2 == 1)
                            {
                                w.WriteLine("0");
                            }
                            else
                            {
                                w.WriteLine("1");
                            }
                        }
                    }
                    break;
                case 2: //終了
                    end = 1;
                    for (i = 1; i <= 2 && position[i + 2, j] != 0; i++)
                    {
                        for (j = 1; j < 4 && position[i + 2, j] != 0; j++) ;
                        if (j == 4)
                        {
                            j = 0; two = 1;
                        }
                    }
                    if (i != 1) i = i - 1;
                    position[i + two + 2, j] = pice;
                    this.lion2.Location = new System.Drawing.Point(534 + ((i + two) * 80), 414 - (j * 105));
                    lion2.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    lion2.Refresh();
                    piece_x[pice] = i + 2;
                    piece_y[pice] = j;
                    team[pice] *= -1;
                    j = 0;
                    two = 0;
                    moves[move_count] = position[place / 10, place % 10] * 100 + place;
                    using (StreamWriter w = new StreamWriter(@"../../\Resources\record.txt", true))
                    {
                        for (i = 1; i <= move_count; i++)
                        {
                            for (j = 0; j < 3; j++)
                            {
                                for (int k = 0; k < 4; k++)
                                {
                                    w.Write("{0}", position_turn[i, j, k]);
                                }
                            }
                            w.Write("{0}{1}{2}", have[i - 1, 0], have[i - 1, 1], have[i - 1, 2]);
                            w.Write("{0}{1}", niwatori_changed[i], moves[i]);
                            if (i % 2 == 1)
                            {
                                w.WriteLine("1");
                            }
                            else
                            {
                                w.WriteLine("0");
                            }
                        }
                    }
                    break;
                case 3:
                    if (turn == 1)
                    {
                        for (i = 1; i <= 2 && position[i + 2, j] != 0; i++)
                        {
                            for (j = 1; j < 4 && position[i + 2, j] != 0; j++) ;
                            if (j == 4)
                            {
                                j = 0; two = 1;
                            }
                        }
                        if (i != 1) i = i - 1;
                        position[i + two + 2, j] = pice;
                        this.kirin1.Location = new System.Drawing.Point(534 + ((i + two) * 80), 414 - (j * 105));
                        kirin1.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        kirin1.Refresh();
                        have[move_count, 0] += 3;
                        piece_x[pice] = i + 2;
                        piece_y[pice] = j; textBox1.Text += i + 2 + "," + j;
                        team[pice] *= -1;
                        j = 0;
                        two = 0;
                    }
                    else
                    {
                        for (i = 1; i <= 2 && position[i + 4, j] != 0; i++)
                        {
                            for (j = 1; j < 4 && position[i + 4, j] != 0; j++) ;
                            if (j == 4)
                            {
                                j = 0; two = 1;
                            }
                        }
                        if (i != 1) i = i - 1;
                        position[i + two + 4, j] = pice;
                        this.kirin1.Location = new System.Drawing.Point(160 - (i + two) * 80, 79 + (j * 105));
                        kirin1.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        kirin1.Refresh();
                        have[move_count, 0] += 4;
                        piece_x[pice] = i + 4;
                        piece_y[pice] = j;
                        team[pice] *= -1;
                        j = 0;
                        two = 0;
                    }
                    break;
                case 4:
                    if (turn == 1)
                    {
                        for (i = 1; i <= 2 && position[i + 2, j] != 0; i++)
                        {
                            for (j = 1; j < 4 && position[i + 2, j] != 0; j++) ;
                            if (j == 4)
                            {
                                j = 0; two = 1;
                            }
                        }
                        if (i != 1) i = i - 1;
                        position[i + two + 2, j] = pice;
                        textBox1.Text += i + "-" + j + ",";
                        this.kirin2.Location = new System.Drawing.Point(534 + ((i + two) * 80), 414 - (j * 105));
                        kirin2.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        kirin2.Refresh();
                        have[move_count, 0] += 3;
                        piece_x[pice] = i + 2;
                        piece_y[pice] = j;
                        team[pice] *= -1;
                        j = 0; two = 0;
                    }
                    else
                    {
                        for (i = 1; i <= 2; i++)
                        {
                            for (j = 1; j < 4 && position[i + 4, j] != 0; j++) ;
                            if (j == 4)
                            {
                                j = 0; two = 1;
                            }
                        }
                        if (i != 1) i = i - 1;
                        position[i + two + 4, j] = pice;
                        this.kirin2.Location = new System.Drawing.Point(160 - (i + two) * 80, 79 + (j * 105));
                        kirin2.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        kirin2.Refresh();
                        have[move_count, 0] += 4;
                        piece_x[pice] = i + 4;
                        piece_y[pice] = j;
                        team[pice] *= -1;
                        j = 0;
                        two = 0;
                    }
                    break;
                case 5:
                    if (turn == 1)
                    {
                        for (i = 1; i <= 2 && position[i + 2, j] != 0; i++)
                        {
                            for (j = 1; j < 4 && position[i + 2, j] != 0; j++) ;
                            if (j == 4)
                            {
                                j = 0; two = 1;
                            }
                        }
                        if (i != 1) i = i - 1;
                        position[i + two + 2, j] = pice;
                        this.zou1.Location = new System.Drawing.Point(534 + ((i + two) * 80), 414 - (j * 105));
                        zou1.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        zou1.Refresh();
                        have[move_count, 1] += 3;
                        piece_x[pice] = i + 2;
                        piece_y[pice] = j;
                        team[pice] *= -1;
                        j = 0;
                        two = 0;
                    }
                    else
                    {
                        for (i = 1; i <= 2 && position[i + 4, j] != 0; i++)
                        {
                            for (j = 1; j < 4 && position[i + 4, j] != 0; j++) ;
                            if (j == 4)
                            {
                                j = 0; two = 1;
                            }
                        }
                        if (i != 1) i = i - 1;
                        position[i + two + 4, j] = pice;
                        this.zou1.Location = new System.Drawing.Point(160 - (i + two) * 80, 79 + (j * 105));
                        zou1.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        zou1.Refresh();
                        have[move_count, 1] += 4;
                        piece_x[pice] = i + 4;
                        piece_y[pice] = j;
                        team[pice] *= -1;
                        j = 0;
                        two = 0;
                    }
                    break;
                case 6:
                    if (turn == 1)
                    {
                        for (i = 1; i <= 2 && position[i + 2, j] != 0; i++)
                        {
                            for (j = 0; j < 4 && position[i + 2, j] != 0; j++) ;
                            if (j == 4)
                            {
                                j = 0; two = 1;
                            }
                        }
                        if (i != 1) i = i - 1;
                        position[i + two + 2, j] = pice;
                        this.zou2.Location = new System.Drawing.Point(534 + ((i + two) * 80), 414 - (j * 105));
                        zou2.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        zou2.Refresh();
                        have[move_count, 1] += 3;
                        piece_x[pice] = i + 2;
                        piece_y[pice] = j;
                        team[pice] *= -1;
                        j = 0;
                        two = 0;
                    }
                    else
                    {
                        for (i = 1; i <= 2 && position[i + 4, j] != 0; i++)
                        {
                            for (j = 1; j < 4 && position[i + 4, j] != 0; j++) ;
                            if (j == 4)
                            {
                                j = 0; two = 1;
                            }
                        }
                        if (i != 1) i = i - 1;
                        position[i + two + 4, j] = pice;
                        this.zou2.Location = new System.Drawing.Point(160 - (i + two) * 80, 79 + (j * 105));
                        zou2.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        zou2.Refresh();
                        have[move_count, 1] += 4;
                        piece_x[pice] = i + 4;
                        piece_y[pice] = j;
                        team[pice] *= -1;
                        j = 0;
                    }
                    break;
                case 7:
                    if (turn == 1)
                    {
                        for (i = 1; i < 3 && position[i + 2, j] != 0; i++)
                        {
                            for (j = 1; j < 4 && position[i + 2, j] != 0; j++) ;
                            if (j == 4)
                            {
                                j = 0; two = 1;
                            }
                        }
                        if (i != 1) i = i - 1;
                        position[i + two + 2, j] = pice;
                        this.hiyoko1.Location = new System.Drawing.Point(534 + ((i + two) * 80), 414 - (j * 105));
                        if (niwatori_flag[0] == 1)
                        {
                            niwatori_flag[0] = 0;
                            this.hiyoko1.Image = ((System.Drawing.Image)(resources.GetObject("hiyoko1.Image")));
                            hiyoko2.Refresh();
                        }
                        else
                        {
                            hiyoko1.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            hiyoko1.Refresh();
                        }
                        have[move_count, 2] += 3;
                        piece_x[pice] = i + 2;
                        piece_y[pice] = j;
                        team[pice] *= -1;
                        j = 0;
                        two = 0;
                    }
                    else
                    {
                        for (i = 1; i <= 2 && position[i + 4, j] != 0; i++)
                        {
                            for (j = 1; j < 4 && position[i + 4, j] != 0; j++) ;
                            if (j == 4)
                            {
                                j = 0; two = 1;
                            }
                        }
                        if (i != 1) i = i - 1;
                        position[i + 4, j] = pice;
                        this.hiyoko1.Location = new System.Drawing.Point(160 - (i + two) * 80, 79 + (j * 105));
                        if (niwatori_flag[0] == 1)
                        {
                            niwatori_flag[0] = 0;
                            this.hiyoko1.Image = ((System.Drawing.Image)(resources.GetObject("hiyoko1.Image")));
                            hiyoko2.Refresh();
                        }
                        hiyoko1.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        hiyoko1.Refresh();
                        have[move_count, 2] += 4;
                        piece_x[pice] = i + 4;
                        piece_y[pice] = j;
                        team[pice] *= -1;
                        j = 0;
                        two = 0;
                    }
                    break;
                case 8:
                    if (turn == 1)
                    {
                        for (i = 1; i <= 2 && position[i + 2, j] != 0; i++)
                        {
                            for (j = 1; j < 4 && position[i + 2, j] != 0; j++) ;
                            if (j == 4)
                            {
                                j = 0; two = 1;
                            }
                        }
                        if (i != 1) i = i - 1;
                        position[i + two + 2, j] = pice;
                        this.hiyoko2.Location = new System.Drawing.Point(534 + ((i + two) * 80), 414 - (j * 105));
                        if (niwatori_flag[1] == 1)
                        {
                            niwatori_flag[1] = 0;
                            this.hiyoko2.Image = ((System.Drawing.Image)(resources.GetObject("hiyoko2.Image")));
                            hiyoko2.Refresh();
                        }
                        else
                        {
                            hiyoko2.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            hiyoko2.Refresh();
                        }
                        have[move_count, 2] += 3;
                        piece_x[pice] = i + 2;
                        piece_y[pice] = j;
                        team[pice] *= -1;
                        j = 0;
                        two = 0;
                    }
                    else
                    {
                        for (i = 1; i <= 2 && position[i + 4, j] != 0; i++)
                        {
                            for (j = 1; j < 4 && position[i + 4, j] != 0; j++) ;
                            if (j == 4)
                            {
                                j = 0; two = 1;
                            }
                        }
                        if (i != 1) i = i - 1;
                        position[i + two + 4, j] = pice;
                        this.hiyoko2.Location = new System.Drawing.Point(160 - (i + two) * 80, 79 + (j * 105));
                        if (niwatori_flag[1] == 1)
                        {
                            niwatori_flag[1] = 0;
                            this.hiyoko2.Image = ((System.Drawing.Image)(resources.GetObject("hiyoko2.Image")));
                            hiyoko2.Refresh();
                        }
                        hiyoko2.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        hiyoko2.Refresh();
                        have[move_count, 2] += 4;
                        piece_x[pice] = i + 4;
                        piece_y[pice] = j;
                        team[pice] *= -1;
                        j = 0;
                        two = 0;
                    }
                    break;
                default:
                    break;
            }

        }

        private int AI_move()	//AIの評価関数
        {
            int x, y, move_x = 0, move_y = 0, move_pice = 0, point = 0;
            int[] move_point = new int[13];
            int data = data_move();
            if (data != 999)
            {
                return data;
            }
            for (int i = 2; i <= 8; i++)
            {
                if (team[i] == -1)
                {
                    x = piece_x[i];
                    y = piece_y[i];
                    //textBox1.Text += "(" + x + "," + y + ")";
                    for (int j = 0; j < 13; j++) move_point[j] = 0; //配列move_pointの初期化
                    if (x < 3)
                    {
                        if (i == 2)		//ライオンを動かす場合
                        {
                            if (x != 0)
                            {
                                if (position[x - 1, y] != 0 && team[position[x - 1, y]] == 1)
                                {
                                    move_point[4] += (int)POINT.Piece;
                                    if (position[x - 1, y] == 1) move_point[4] = 2000;
                                }
                                if (team[position[x - 1, y]] != -1)
                                {
                                    move_point[4] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x - 1) * 10 + y, x * 10 + y);
                                }
                                if (get_lion(2, 2, (x - 1) * 10 + y, x * 10 + y) == 1) move_point[4] += -1000;
                                if (team[position[x - 1, y]] == -1) move_point[4] += -1000;
                                move_point[4] += can_move(i, x - 1, y) + 2000;
                                if (y != 0)
                                {
                                    if (position[x - 1, y - 1] != 0 && team[position[x - 1, y - 1]] == 1)
                                    {
                                        move_point[1] += (int)POINT.Piece;
                                        if (position[x - 1, y - 1] == 1) move_point[1] = 2000;
                                    }
                                    if (team[position[x - 1, y - 1]] != -1)
                                    {
                                        move_point[1] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x - 1) * 10 + y - 1, x * 10 + y);
                                    }
                                    if (get_lion(2, 2, (x - 1) * 10 + y - 1, x * 10 + y) == 1) move_point[1] += -1000;
                                    if (team[position[x - 1, y - 1]] == -1) move_point[1] += -1000;
                                    move_point[1] += can_move(i, x - 1, y - 1) + 2000;
                                }
                                if (y != 3)
                                {
                                    if (position[x - 1, y + 1] != 0 && team[position[x - 1, y + 1]] == 1)
                                    {
                                        move_point[7] += (int)POINT.Piece;
                                        if (position[x - 1, y + 1] == 1) move_point[7] = 2000;
                                    }
                                    if (team[position[x - 1, y + 1]] != -1)
                                    {
                                        move_point[7] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x - 1) * 10 + y + 1, x * 10 + y);
                                    }
                                    if (get_lion(2, 2, (x - 1) * 10 + y + 1, x * 10 + y) == 1) move_point[7] += -1000;
                                    if (team[position[x - 1, y + 1]] == -1) move_point[7] += -1000;
                                    move_point[7] += can_move(i, x - 1, y + 1) + 2000;
                                }
                            }
                            if (x != 2)
                            {
                                if (position[x + 1, y] != 0 && team[position[x + 1, y]] == 1)
                                {
                                    move_point[6] += (int)POINT.Piece;
                                    if (position[x + 1, y] == 1) move_point[6] = 2000;
                                }
                                if (team[position[x + 1, y]] != -1)
                                {
                                    move_point[6] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x + 1) * 10 + y, x * 10 + y);
                                }
                                if (get_lion(2, 2, (x + 1) * 10 + y, x * 10 + y) == 1) move_point[6] += -1000;
                                if (team[position[x + 1, y]] == -1) move_point[6] += -1000;
                                move_point[6] += can_move(i, x + 1, y) + 2000;
                                if (y != 0)
                                {
                                    if (position[x + 1, y - 1] != 0 && team[position[x + 1, y - 1]] == 1)
                                    {
                                        move_point[3] += (int)POINT.Piece;
                                        if (position[x + 1, y - 1] == 1) move_point[3] = 2000;
                                    }
                                    if (team[position[x + 1, y - 1]] != -1)
                                    {
                                        move_point[3] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x + 1) * 10 + y - 1, x * 10 + y);
                                    }
                                    if (get_lion(2, 2, (x + 1) * 10 + y - 1, x * 10 + y) == 1) move_point[3] += -1000;
                                    if (team[position[x + 1, y - 1]] == -1) move_point[3] += -1000;
                                    move_point[3] += can_move(i, x + 1, y - 1) + 2000;
                                }
                                if (y != 3)
                                {
                                    if (position[x + 1, y + 1] != 0 && team[position[x + 1, y + 1]] == 1)
                                    {
                                        move_point[9] += (int)POINT.Piece;
                                        if (position[x + 1, y + 1] == 1) move_point[9] = 2000;
                                    }
                                    if (team[position[x + 1, y + 1]] != -1)
                                    {
                                        move_point[9] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x + 1) * 10 + y + 1, x * 10 + y);
                                    }
                                    if (get_lion(2, 2, (x + 1) * 10 + y + 1, x * 10 + y) == 1) move_point[9] += -1000;
                                    if (team[position[x + 1, y + 1]] == -1) move_point[9] += -1000;
                                    move_point[9] += can_move(i, x + 1, y + 1) + 2000;
                                }
                            }
                            if (y != 0)
                            {
                                if (position[x, y - 1] != 0 && team[position[x, y - 1]] == 1)
                                {
                                    move_point[2] += (int)POINT.Piece;
                                    if (position[x, y - 1] == 1) move_point[2] = 2000;
                                }
                                if (team[position[x, y - 1]] != -1)
                                {
                                    move_point[2] += prediction_move(turn * -1, position, team, niwatori_flag, i, x * 10 + y - 1, x * 10 + y);
                                }
                                if (get_lion(2, 2, x * 10 + y - 1, x * 10 + y) == 1) move_point[2] += -1000;
                                if (team[position[x, y - 1]] == -1) move_point[2] = -1000;
                                move_point[2] += can_move(i, x, y - 1) + 2000;
                            }
                            if (y != 3)
                            {
                                if (position[x, y + 1] != 0 && team[position[x, y + 1]] == 1)
                                {
                                    move_point[8] += (int)POINT.Piece;
                                    if (position[x, y + 1] == 1) move_point[8] = 1000;
                                }
                                if (team[position[x, y + 1]] != -1)
                                {
                                    move_point[8] += prediction_move(turn * -1, position, team, niwatori_flag, i, x * 10 + y + 1, x * 10 + y);
                                }
                                if (get_lion(2, 2, x * 10 + y + 1, x * 10 + y) == 1) move_point[8] += -1000;
                                if (team[position[x, y + 1]] == -1) move_point[8] += -1000;
                                move_point[8] += can_move(i, x, y + 1) + 2000;
                            }
                        }
                        else if (i == 3 || i == 4)	//キリンを動かす場合
                        {
                            if (x != 0)
                            {
                                if (position[x - 1, y] != 0 && team[position[x - 1, y]] == 1)
                                {
                                    move_point[4] += (int)POINT.Piece;
                                    if (position[x - 1, y] == 1) move_point[4] = 2000;
                                }
                                if (team[position[x - 1, y]] != -1)
                                {
                                    move_point[4] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x - 1) * 10 + y, x * 10 + y);
                                }
                                if (get_lion(2, i, (x - 1) * 10 + y, x * 10 + y) == 1) move_point[4] += -1000;
                                if (team[position[x - 1, y]] == -1) move_point[4] += -1000;
                                move_point[4] += can_move(i, x - 1, y) + 2000;
                            }
                            if (x != 2)
                            {
                                if (position[x + 1, y] != 0 && team[position[x + 1, y]] == 1)
                                {
                                    move_point[6] += (int)POINT.Piece;
                                    if (position[x + 1, y] == 1) move_point[6] = 2000;
                                }
                                if (team[position[x + 1, y]] != -1)
                                {
                                    move_point[6] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x + 1) * 10 + y, x * 10 + y);
                                }
                                if (get_lion(2, i, (x + 1) * 10 + y, x * 10 + y) == 1) move_point[6] += -1000;
                                if (team[position[x + 1, y]] == -1) move_point[6] += -1000;
                                move_point[6] += can_move(i, x + 1, y) + 2000;
                            }
                            if (y != 0)
                            {
                                if (position[x, y - 1] != 0 && team[position[x, y - 1]] == 1)
                                {
                                    move_point[2] += (int)POINT.Piece;
                                    if (position[x, y - 1] == 1) move_point[2] = 2000;
                                }
                                if (team[position[x, y - 1]] != -1)
                                {
                                    move_point[2] += prediction_move(turn * -1, position, team, niwatori_flag, i, x * 10 + y - 1, x * 10 + y);
                                }
                                if (get_lion(2, i, x * 10 + y - 1, x * 10 + y) == 1) move_point[2] += -1000;
                                if (team[position[x, y - 1]] == -1) move_point[2] += -1000;
                                move_point[2] += can_move(i, x, y - 1) + 2000;
                            }
                            if (y != 3)
                            {
                                if (position[x, y + 1] != 0 && team[position[x, y + 1]] == 1)
                                {
                                    move_point[8] += (int)POINT.Piece;
                                    if (position[x, y + 1] == 1) move_point[8] = 2000;
                                }
                                if (team[position[x, y + 1]] != -1)
                                {
                                    move_point[8] += prediction_move(turn * -1, position, team, niwatori_flag, i, x * 10 + y + 1, x * 10 + y);
                                }
                                if (get_lion(2, i, x * 10 + y + 1, x * 10 + y) == 1) move_point[8] += -1000;
                                if (team[position[x, y + 1]] == -1) move_point[8] += -1000;
                                move_point[8] += can_move(i, x, y + 1) + 2000;
                            }
                        }
                        else if (i == 5 || i == 6)	//ゾウを動かす場合
                        {
                            if (x != 0)
                            {
                                if (y != 0)
                                {
                                    if (position[x - 1, y - 1] != 0 && team[position[x - 1, y - 1]] == 1)
                                    {
                                        move_point[1] += (int)POINT.Piece;
                                        if (position[x - 1, y - 1] == 1) move_point[1] = 2000;
                                    }
                                    if (team[position[x - 1, y - 1]] != -1)
                                    {
                                        move_point[1] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x - 1) * 10 + y - 1, x * 10 + y);
                                    }
                                    if (get_lion(2, i, (x - 1) * 10 + y - 1, x * 10 + y) == 1) move_point[1] += -1000;
                                    if (team[position[x - 1, y - 1]] == -1) move_point[1] += -1000;
                                    move_point[1] += can_move(i, x - 1, y - 1) + 2000;
                                }
                                if (y != 3)
                                {
                                    if (position[x - 1, y + 1] != 0 && team[position[x - 1, y + 1]] == 1)
                                    {
                                        move_point[7] += (int)POINT.Piece;
                                        if (position[x - 1, y + 1] == 1) move_point[7] = 2000;
                                    }
                                    if (team[position[x - 1, y + 1]] != -1)
                                    {
                                        move_point[7] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x - 1) * 10 + y + 1, x * 10 + y);
                                    }
                                    if (get_lion(2, i, (x - 1) * 10 + y + 1, x * 10 + y) == 1) move_point[7] += -1000;
                                    if (team[position[x - 1, y + 1]] == -1) move_point[7] += -1000;
                                    move_point[7] += can_move(i, x - 1, y + 1) + 2000;
                                }
                            }
                            if (x != 2)
                            {
                                if (y != 0)
                                {
                                    if (position[x + 1, y - 1] != 0 && team[position[x + 1, y - 1]] == 1)
                                    {
                                        move_point[3] += (int)POINT.Piece;
                                        if (position[x + 1, y - 1] == 1) move_point[3] = 2000;
                                    }
                                    if (team[position[x + 1, y - 1]] != -1)
                                    {
                                        move_point[3] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x + 1) * 10 + y - 1, x * 10 + y);
                                    }
                                    if (get_lion(2, i, (x + 1) * 10 + y - 1, x * 10 + y) == 1) move_point[3] += -1000;
                                    if (team[position[x + 1, y - 1]] == -1) move_point[3] += -1000;
                                    move_point[3] += can_move(i, x + 1, y - 1) + 2000;
                                }
                                if (y != 3)
                                {
                                    if (position[x + 1, y + 1] != 0 && team[position[x + 1, y + 1]] == 1)
                                    {
                                        move_point[9] += (int)POINT.Piece;
                                        if (position[x + 1, y + 1] == 1) move_point[9] = 2000;
                                    }
                                    if (team[position[x + 1, y + 1]] != -1)
                                    {
                                        move_point[9] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x + 1) * 10 + y + 1, x * 10 + y);
                                    }
                                    if (get_lion(2, i, (x + 1) * 10 + y + 1, x * 10 + y) == 1) move_point[9] += -1000;
                                    if (team[position[x + 1, y + 1]] == -1) move_point[9] += -1000;
                                    move_point[9] += can_move(i, x + 1, y + 1) + 2000;
                                }
                            }
                        }
                        else if (i == 7 || i == 8)	//ヒヨコを動かす場合
                        {
                            if ((i == 7 && niwatori_flag[0] == 1) || (i == 8 && niwatori_flag[1] == 1))
                            {
                                if (x != 0)
                                {
                                    if (position[x - 1, y] != 0 && team[position[x - 1, y]] == 1)
                                    {
                                        move_point[4] += (int)POINT.Piece;
                                        if (position[x - 1, y] == 1) move_point[4] = 2000;
                                    }
                                    if (team[position[x - 1, y]] != -1)
                                    {
                                        move_point[4] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x - 1) * 10 + y, x * 10 + y);
                                    }
                                    if (get_lion(2, i, (x - 1) * 10 + y, x * 10 + y) == 1) move_point[4] += -1000;
                                    if (team[position[x - 1, y]] == -1) move_point[4] += -1000;
                                    move_point[4] += can_move(i, x - 1, y) + 2000;
                                    if (y != 3)
                                    {
                                        if (position[x - 1, y + 1] != 0 && team[position[x - 1, y + 1]] == 1)
                                        {
                                            move_point[7] += (int)POINT.Piece;
                                            if (position[x - 1, y + 1] == 1) move_point[7] = 2000;
                                        }
                                        if (team[position[x - 1, y + 1]] != -1)
                                        {
                                            move_point[7] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x - 1) * 10 + y + 1, x * 10 + y);
                                        }
                                        if (get_lion(2, i, (x - 1) * 10 + y + 1, x * 10 + y) == 1) move_point[7] += -1000;
                                        if (team[position[x - 1, y + 1]] == -1) move_point[7] += -1000;
                                        move_point[7] += can_move(i, x - 1, y + 1) + 2000;
                                    }
                                }
                                if (x != 2)
                                {
                                    if (position[x + 1, y] != 0 && team[position[x + 1, y]] == 1)
                                    {
                                        move_point[6] += (int)POINT.Piece;
                                        if (position[x + 1, y] == 1) move_point[6] = 2000;
                                    }
                                    if (team[position[x + 1, y]] != -1)
                                    {
                                        move_point[6] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x + 1) * 10 + y, x * 10 + y);
                                    }
                                    if (get_lion(2, i, (x + 1) * 10 + y, x * 10 + y) == 1) move_point[6] += -1000;
                                    if (team[position[x + 1, y]] == -1) move_point[6] += -1000;
                                    move_point[6] += can_move(i, x + 1, y) + 2000;
                                    if (y != 3)
                                    {
                                        if (position[x + 1, y + 1] != 0 && team[position[x + 1, y + 1]] == 1)
                                        {
                                            move_point[9] += (int)POINT.Piece;
                                            if (position[x + 1, y + 1] == 1) move_point[9] = 2000;
                                        }
                                        if (team[position[x + 1, y + 1]] != -1)
                                        {
                                            move_point[9] += prediction_move(turn * -1, position, team, niwatori_flag, i, (x + 1) * 10 + y + 1, x * 10 + y);
                                        }
                                        if (get_lion(2, i, (x + 1) * 10 + y + 1, x * 10 + y) == 1) move_point[9] += -1000;
                                        if (team[position[x + 1, y + 1]] == -1) move_point[9] += -1000;
                                        move_point[9] += can_move(i, x + 1, y + 1) + 2000;
                                    }
                                }
                                if (y != 0)
                                {
                                    if (position[x, y - 1] != 0 && team[position[x, y - 1]] == 1)
                                    {
                                        move_point[2] += (int)POINT.Piece;
                                        if (position[x, y - 1] == 1) move_point[2] = 2000;
                                    }
                                    if (team[position[x, y - 1]] != -1)
                                    {
                                        move_point[2] += prediction_move(turn * -1, position, team, niwatori_flag, i, x * 10 + y - 1, x * 10 + y);
                                    }
                                    if (get_lion(2, i, x * 10 + y - 1, x * 10 + y) == 1) move_point[2] += -1000;
                                    if (team[position[x, y - 1]] == -1) move_point[2] += -1000;
                                    move_point[2] += can_move(i, x, y - 1) + 2000;
                                }
                            }
                            if (y != 3)
                            {
                                if (position[x, y + 1] != 0 && team[position[x, y + 1]] == 1)
                                {
                                    move_point[8] += (int)POINT.Piece;
                                    if (position[x, y + 1] == 1) move_point[8] = 2000;
                                }
                                if (team[position[x, y + 1]] != -1)
                                {
                                    move_point[8] += prediction_move(turn * -1, position, team, niwatori_flag, i, x * 10 + y + 1, x * 10 + y);
                                }
                                if (get_lion(2, i, x * 10 + y + 1, x * 10 + y) == 1) move_point[8] += -1000;
                                if (team[position[x, y + 1]] == -1) move_point[8] += -1000;
                                move_point[8] += can_move(i, x, y + 1) + 2000;
                            }
                        }
                    }
                    else if (x >= 5)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            for (int k = 1; k < 4; k++)
                            {
                                if (position[k - 1, j] == 0)
                                {
                                    if (i < 7)
                                    {
                                        if (get_lion(2, i, (k - 1) * 10 + j, x * 10 + y) == 1) move_point[j * 3 + k] += -1000;
                                        move_point[j * 3 + k] += can_move(i, k - 1, j) + 2000;
                                        move_point[j * 3 + k] += prediction_move(turn * -1, position, team, niwatori_flag, i, (k - 1) * 10 + j, x * 10 + y);
                                    }
                                    else
                                    {
                                        if (j != 3)
                                        {
                                            if (get_lion(2, i, (k - 1) * 10 + j, x * 10 + y) == 1) move_point[j * 3 + k] += -1000;
                                            move_point[j * 3 + k] += can_move(i, k - 1, j) + 2000;
                                            move_point[j * 3 + k] += prediction_move(turn * -1, position, team, niwatori_flag, i, (k - 1) * 10 + j, x * 10 + y);
                                        }
                                    }
                                }
                                // textBox1.Text += "(" + (k - 1) * 10 + j + "," + move_point[j * 3 + k] + ")";
                            }
                        }
                    }

                    for (int j = 1; j < 13; j++) //指し手の決定
                    {
                        //textBox1.Text += move_point[i] + ",";
                        if (move_point[j] >= point && piece_x[i] < 3)
                        {
                            point = move_point[j];
                            move_pice = i;
                            if (j % 3 == 1)
                            {       	     //x方向
                                move_x = x - 1;
                            }
                            else if (j % 3 == 2)
                            {
                                move_x = x;
                            }
                            else if (j % 3 == 0)
                            {
                                move_x = x + 1;
                            }
                            if (j <= 3)	     //y方向
                            {
                                move_y = y - 1;
                            }
                            else if (j <= 6)
                            {
                                move_y = y;
                            }
                            else
                            {
                                move_y = y + 1;
                            }
                        }
                        else if (move_point[j] >= point && piece_x[i] >= 5)
                        {
                            point = move_point[j];
                            move_pice = i;
                            move_x = (j - 1) % 3;
                            move_y = (j - 1) / 3;
                        }
                    }
                }
            }
            textBox1.Text += move_pice * 100 + move_x * 10 + move_y;
            piece_refresh();
            System.Threading.Thread.Sleep(500);
            return move_pice * 100 + move_x * 10 + move_y;
        }

        int can_move(int piece, int x, int y)
        {
            int point = 0;
            int lion_move = -5, kirin_move = 0, zou_move = 0;
            if (team[position[x, y]] != -1)
            {
                if (piece == 2)		//ライオンを動かす場合
                {
                    point = lion_move;
                    point++;    //元の位置に戻れるため
                    if (x != 0)
                    {
                        point++;
                        if (position[x - 1, y] != 0 && team[position[x - 1, y]] == -1)
                        {
                            point--;
                        }
                        if (y != 0)
                        {
                            point++;
                            if (position[x - 1, y - 1] != 0 && team[position[x - 1, y - 1]] == -1)
                            {
                                point--;
                            }
                        }
                        if (y != 3)
                        {
                            point++;
                            if (position[x - 1, y + 1] != 0 && team[position[x - 1, y + 1]] == -1)
                            {
                                point--;
                            }
                        }
                    }
                    if (x != 2)
                    {
                        point++;
                        if (position[x + 1, y] != 0 && team[position[x + 1, y]] == -1)
                        {
                            point--;
                        }
                        if (y != 0)
                        {
                            point++;
                            if (position[x + 1, y - 1] != 0 && team[position[x + 1, y - 1]] == -1)
                            {
                                point--;
                            }
                        }
                        if (y != 3)
                        {
                            point++;
                            if (position[x + 1, y + 1] != 0 && team[position[x + 1, y + 1]] == -1)
                            {
                                point--;
                            }
                        }
                    }
                    if (y != 0)
                    {
                        point++;
                        if (position[x, y - 1] != 0 && team[position[x, y - 1]] == -1)
                        {
                            point--;
                        }
                    }
                    if (y != 3)
                    {
                        point++;
                        if (position[x, y + 1] != 0 && team[position[x, y + 1]] == -1)
                        {
                            point--;
                        }
                    }
                }
                else if (piece == 3 || piece == 4)	//キリンを動かす場合
                {
                    point = kirin_move;
                    point++;    //元の位置に戻れるため
                    if (x != 0)
                    {
                        point++;
                        if (position[x - 1, y] != 0 && team[position[x - 1, y]] == -1)
                        {
                            point--;
                        }
                    }
                    if (x != 2)
                    {
                        point++;
                        if (position[x + 1, y] != 0 && team[position[x + 1, y]] == -1)
                        {
                            point--;
                        }
                    }
                    if (y != 0)
                    {
                        point++;
                        if (position[x, y - 1] != 0 && team[position[x, y - 1]] == -1)
                        {
                            point--;
                        }
                    }
                    if (y != 3)
                    {
                        point++;
                        if (position[x, y + 1] != 0 && team[position[x, y + 1]] == -1)
                        {
                            point--;
                        }
                    }
                }
                else if (piece == 5 || piece == 6)	//ゾウを動かす場合
                {
                    point = zou_move;
                    point++;    //元の位置に戻れるため
                    if (x != 0)
                    {
                        if (y != 0)
                        {
                            point++;
                            if (position[x - 1, y - 1] != 0 && team[position[x - 1, y - 1]] == -1)
                            {
                                point--;
                            }
                        }
                        if (y != 3)
                        {
                            point++;
                            if (position[x - 1, y + 1] != 0 && team[position[x - 1, y + 1]] == -1)
                            {
                                point--;
                            }
                        }
                    }
                    if (x != 2)
                    {
                        if (y != 0)
                        {
                            point++;
                            if (position[x + 1, y - 1] != 0 && team[position[x + 1, y - 1]] == -1)
                            {
                                point--;
                            }
                        }
                        if (y != 3)
                        {
                            point++;
                            if (position[x + 1, y + 1] != 0 && team[position[x + 1, y + 1]] == -1)
                            {
                                point--;
                            }
                        }
                    }
                }
                else if (piece == 7 || piece == 8)	//ヒヨコを動かす場合
                {
                    if ((piece == 7 && niwatori_flag[0] == 1) || (piece == 8 && niwatori_flag[1] == 1))
                    {
                        point++;    //元の位置に戻れるため
                        if (x != 0)
                        {
                            point++;
                            if (position[x - 1, y] != 0 && team[position[x - 1, y]] == -1)
                            {
                                point--;
                            }
                            if (y != 3)
                            {
                                point++;
                                if (position[x - 1, y + 1] != 0 && team[position[x - 1, y + 1]] == -1)
                                {
                                    point--;
                                }
                            }
                        }
                        if (x != 2)
                        {
                            point++;
                            if (position[x + 1, y] != 0 && team[position[x + 1, y]] == -1)
                            {
                                point--;
                            }
                            if (y != 3)
                            {
                                point++;
                                if (position[x + 1, y + 1] != 0 && team[position[x + 1, y + 1]] == -1)
                                {
                                    point--;
                                }
                            }
                        }
                        if (y != 0)
                        {
                            point++;
                            if (position[x, y - 1] != 0 && team[position[x, y - 1]] == -1)
                            {
                                point--;
                            }
                        }
                    }
                    if (y != 3)
                    {
                        point++;
                        if (position[x, y + 1] != 0 && team[position[x, y + 1]] == -1)
                        {
                            point--;
                        }
                    }
                }
            }
            return point;
        }

        int get_lion(int side, int piece, int move_to, int move_from) // side 1:先手ライオン 2:後手ライオン 
        {
            int[,] current_position = new int[7, 4];
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    current_position[i, j] = position[i, j];
                }
            }

            if (piece != 0)
            {
                current_position[move_to / 10, move_to % 10] = piece;
                current_position[move_from / 10, move_from % 10] = 0;
            }

            for (int i = 1; i <= 8; i++)
            {
                if (side == 2 && team[i] == 1) //後手ライオンの場合
                {
                    int x = piece_x[i];
                    int y = piece_y[i];
                    if (i == 1)		//ライオンを動かす場合
                    {
                        if (x != 0)
                        {
                            if (current_position[x - 1, y] == 2)
                            {
                                return 1;
                            }
                            if (y != 0)
                            {
                                if (current_position[x - 1, y - 1] == 2)
                                {
                                    return 1;
                                }
                            }
                            if (y != 3)
                            {
                                if (current_position[x - 1, y + 1] == 2)
                                {
                                    return 1;
                                }
                            }
                        }
                        if (x != 2)
                        {
                            if (current_position[x + 1, y] == 2)
                            {
                                return 1;
                            }
                            if (y != 0)
                            {
                                if (current_position[x + 1, y - 1] == 2)
                                {
                                    return 1;
                                }
                            }
                            if (y != 3)
                            {
                                if (current_position[x + 1, y + 1] == 2)
                                {
                                    return 1;
                                }
                            }
                        }
                        if (y != 0)
                        {
                            if (current_position[x, y - 1] == 2)
                            {
                                return 1;
                            }
                        }
                        if (y != 3)
                        {
                            if (current_position[x, y + 1] == 2)
                            {
                                return 1;
                            }
                        }
                    }
                    else if (i == 3 || i == 4)	//キリンを動かす場合
                    {
                        if (x != 0)
                        {
                            if (current_position[x - 1, y] == 2)
                            {
                                return 1;
                            }
                        }
                        if (x != 2)
                        {
                            if (current_position[x + 1, y] == 2)
                            {
                                return 1;
                            }
                        }
                        if (y != 0)
                        {
                            if (current_position[x, y - 1] == 2)
                            {
                                return 1;
                            }
                        }
                        if (y != 3)
                        {
                            if (current_position[x, y + 1] == 2)
                            {
                                return 1;
                            }
                        }
                    }
                    else if (i == 5 || i == 6)	//ゾウを動かす場合
                    {
                        if (x != 0)
                        {
                            if (y != 0)
                            {
                                if (current_position[x - 1, y - 1] == 2)
                                {
                                    return 1;
                                }
                            }
                            if (y != 3)
                            {
                                if (current_position[x - 1, y + 1] == 2)
                                {
                                    return 1;
                                }
                            }
                        }
                        if (x != 2)
                        {
                            if (y != 0)
                            {
                                if (current_position[x + 1, y - 1] == 2)
                                {
                                    return 1;
                                }
                            }
                            if (y != 3)
                            {
                                if (current_position[x + 1, y + 1] == 2)
                                {
                                    return 1;
                                }
                            }
                        }
                    }
                    else if (i == 7 || i == 8)	//ヒヨコを動かす場合
                    {
                        if ((i == 7 && niwatori_flag[0] == 1) || (i == 8 && niwatori_flag[1] == 1))
                        {
                            if (x != 0)
                            {
                                if (current_position[x - 1, y] == 2)
                                {
                                    return 1;
                                }
                                if (y != 0)
                                {
                                    if (current_position[x - 1, y - 1] == 2)
                                    {
                                        return 1;
                                    }
                                }
                            }
                            if (x != 2)
                            {
                                if (current_position[x + 1, y] == 2)
                                {
                                    return 1;
                                }
                                if (y != 0)
                                {
                                    if (current_position[x + 1, y - 1] == 2)
                                    {
                                        return 1;
                                    }
                                }
                            }
                            if (y != 3)
                            {
                                if (current_position[x, y + 1] == 2)
                                {
                                    return 1;
                                }
                            }
                        }
                        if (y != 0)
                        {
                            if (current_position[x, y - 1] == 2)
                            {
                                return 1;
                            }
                        }
                    }
                }
                else if (side == 1 && team[i] == -1) //先手ライオンの場合
                {
                    int x = piece_x[i];
                    int y = piece_y[i];
                    if (i == 2)		//ライオンを動かす場合
                    {
                        if (x != 0)
                        {
                            if (current_position[x - 1, y] == 1)
                            {
                                return 1;
                            }
                            if (y != 0)
                            {
                                if (current_position[x - 1, y - 1] == 1)
                                {
                                    return 1;
                                }
                            }
                            if (y != 3)
                            {
                                if (current_position[x - 1, y + 1] == 1)
                                {
                                    return 1;
                                }
                            }
                        }
                        if (x != 2)
                        {
                            if (current_position[x + 1, y] == 1)
                            {
                                return 1;
                            }
                            if (y != 0)
                            {
                                if (current_position[x + 1, y - 1] == 1)
                                {
                                    return 1;
                                }
                            }
                            if (y != 3)
                            {
                                if (current_position[x + 1, y + 1] == 1)
                                {
                                    return 1;
                                }
                            }
                        }
                        if (y != 0)
                        {
                            if (current_position[x, y - 1] == 1)
                            {
                                return 1;
                            }
                        }
                        if (y != 3)
                        {
                            if (current_position[x, y + 1] == 1)
                            {
                                return 1;
                            }
                        }
                    }
                    else if (i == 3 || i == 4)	//キリンを動かす場合
                    {
                        if (x != 0)
                        {
                            if (current_position[x - 1, y] == 1)
                            {
                                return 1;
                            }
                        }
                        if (x != 2)
                        {
                            if (current_position[x + 1, y] == 1)
                            {
                                return 1;
                            }
                        }
                        if (y != 0)
                        {
                            if (current_position[x, y - 1] == 1)
                            {
                                return 1;
                            }
                        }
                        if (y != 3)
                        {
                            if (current_position[x, y + 1] == 1)
                            {
                                return 1;
                            }
                        }
                    }
                    else if (i == 5 || i == 6)	//ゾウを動かす場合
                    {
                        if (x != 0)
                        {
                            if (y != 0)
                            {
                                if (current_position[x - 1, y - 1] == 1)
                                {
                                    return 1;
                                }
                            }
                            if (y != 3)
                            {
                                if (current_position[x - 1, y + 1] == 1)
                                {
                                    return 1;
                                }
                            }
                        }
                        if (x != 2)
                        {
                            if (y != 0)
                            {
                                if (current_position[x + 1, y - 1] == 1)
                                {
                                    return 1;
                                }
                            }
                            if (y != 3)
                            {
                                if (current_position[x + 1, y + 1] == 1)
                                {
                                    return 1;
                                }
                            }
                        }
                    }
                    else if (i == 7 || i == 8)	//ヒヨコを動かす場合
                    {
                        if ((i == 7 && niwatori_flag[0] == 1) || (i == 8 && niwatori_flag[1] == 1))
                        {
                            if (x != 0)
                            {
                                if (current_position[x - 1, y] == 1)
                                {
                                    return 1;
                                }
                                if (y != 3)
                                {
                                    if (current_position[x - 1, y + 1] == 1)
                                    {
                                        return 1;
                                    }
                                }
                            }
                            if (x != 2)
                            {
                                if (current_position[x + 1, y] == 1)
                                {
                                    return 1;
                                }
                                if (y != 3)
                                {
                                    if (current_position[x + 1, y + 1] == 1)
                                    {
                                        return 1;
                                    }
                                }
                            }
                            if (y != 0)
                            {
                                if (current_position[x, y - 1] == 1)
                                {
                                    return 1;
                                }
                            }
                        }
                        if (y != 3)
                        {
                            if (current_position[x, y + 1] == 1)
                            {
                                return 1;
                            }
                        }
                    }
                }
            }
            return 0;
        }

        int prediction_move(int turns, int[,] positions, int[] teams, int[] niwatori, int piece, int move_to, int move_from) // turn 1:先手 -1:後手 
        {
            int[,] current_position = new int[7, 4];
            int[] current_teams = new int[9];
            int[] current_niwatori = new int[2];
            int[] moves_point = new int[13];
            int point = 100;
            if (turns == -1) point *= -1;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    current_position[i, j] = positions[i, j];
                }
            }
            current_teams[0] = 0;
            for (int i = 1; i < 8; i++) current_teams[i] = teams[i];
            for (int i = 1; i < 13; i++) moves_point[i] = -100;
            current_niwatori[0] = niwatori[0];
            current_niwatori[1] = niwatori[1];

            if (piece != 0)
            {
                if (current_position[move_to / 10, move_to % 10] != 0)
                {
                    int two = 0, j, k = 0;
                    for (j = 1; j <= 2 && position[j + 4, k] != 0; j++)
                    {
                        for (k = 1; k < 4 && position[j + 4, k] != 0; k++) ;
                        if (k == 4)
                        {
                            k = 0; two = 1;
                        }
                    }
                    if (j != 1) j = j - 1;
                    current_position[j + two + 4, j] = current_position[move_to / 10, move_to % 10];
                    current_teams[current_position[move_to / 10, move_to % 10]] *= -1;
                    if (current_position[move_to / 10, move_to % 10] == 7 || current_position[move_to / 10, move_to % 10] == 8)
                    {
                        niwatori[current_position[move_to / 10, move_to % 10] - 7] = 0;
                    }
                }
                current_position[move_to / 10, move_to % 10] = piece;
                current_position[move_from / 10, move_from % 10] = 0;
            }

            for (int i = 1; i <= 8; i++)
            {
                for (int j = 0; j < 13; j++) moves_point[j] = -100;
                if (turns == 1 && current_teams[i] == 1) //先手の場合
                {
                    int x = 0, y = 0;
                    for (int j = 0; j < 7; j++)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            if (current_position[j, k] == i)
                            {
                                x = j;
                                y = k;
                            }
                        }
                    }
                    if (x < 3)
                    {
                        if (i == 1)		//ライオンを動かす場合
                        {
                            if (x != 0)
                            {
                                if (current_teams[current_position[x - 1, y]] != 1)
                                {
                                    moves_point[4] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x - 1) * 10 + y, x * 10 + y);
                                }
                                if (y != 0)
                                {
                                    if (current_teams[current_position[x - 1, y - 1]] != 1)
                                    {
                                        moves_point[1] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x - 1) * 10 + y - 1, x * 10 + y);
                                    }
                                }
                                if (y != 3)
                                {
                                    if (current_teams[current_position[x - 1, y + 1]] != 1)
                                    {
                                        moves_point[7] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x - 1) * 10 + y + 1, x * 10 + y);
                                    }
                                }
                            }
                            if (x != 2)
                            {
                                if (current_teams[current_position[x + 1, y]] != 1)
                                {
                                    moves_point[6] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x + 1) * 10 + y, x * 10 + y);
                                }
                                if (y != 0)
                                {
                                    if (current_teams[current_position[x + 1, y - 1]] != 1)
                                    {
                                        moves_point[3] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x + 1) * 10 + y - 1, x * 10 + y);
                                    }
                                }
                                if (y != 3)
                                {
                                    if (current_teams[current_position[x + 1, y + 1]] != 1)
                                    {
                                        moves_point[9] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x + 1) * 10 + y + 1, x * 10 + y);
                                    }
                                }
                            }
                            if (y != 0)
                            {
                                if (current_teams[current_position[x, y - 1]] != 1)
                                {
                                    moves_point[2] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, x * 10 + y - 1, x * 10 + y);
                                }
                            }
                            if (y != 3)
                            {
                                if (current_teams[current_position[x, y + 1]] != 1)
                                {
                                    moves_point[8] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, x * 10 + y + 1, x * 10 + y);
                                }
                            }

                        }
                        else if (i == 3 || i == 4)	//キリンを動かす場合
                        {
                            if (x != 0)
                            {
                                if (current_teams[current_position[x - 1, y]] != 1)
                                {
                                    moves_point[4] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x - 1) * 10 + y, x * 10 + y);
                                }
                            }
                            if (x != 2)
                            {
                                if (current_teams[current_position[x + 1, y]] != 1)
                                {
                                    moves_point[6] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x + 1) * 10 + y, x * 10 + y);
                                }
                            }
                            if (y != 0)
                            {
                                if (current_teams[current_position[x, y - 1]] != 1)
                                {
                                    moves_point[2] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, x * 10 + y - 1, x * 10 + y);
                                }
                            }
                            if (y != 3)
                            {
                                if (current_teams[current_position[x, y + 1]] != 1)
                                {
                                    moves_point[8] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, x * 10 + y + 1, x * 10 + y);
                                }
                            }
                        }
                        else if (i == 5 || i == 6)	//ゾウを動かす場合
                        {
                            if (x != 0)
                            {
                                if (y != 0)
                                {
                                    if (current_teams[current_position[x - 1, y - 1]] != 1)
                                    {
                                        moves_point[1] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x - 1) * 10 + y - 1, x * 10 + y);
                                    }
                                }
                                if (y != 3)
                                {
                                    if (current_teams[current_position[x - 1, y + 1]] != 1)
                                    {
                                        moves_point[7] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x - 1) * 10 + y + 1, x * 10 + y);
                                    }
                                }
                            }
                            if (x != 2)
                            {
                                if (y != 0)
                                {
                                    if (current_teams[current_position[x + 1, y - 1]] != 1)
                                    {
                                        moves_point[3] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x + 1) * 10 + y - 1, x * 10 + y);
                                    }
                                }
                                if (y != 3)
                                {
                                    if (current_teams[current_position[x + 1, y + 1]] != 1)
                                    {
                                        moves_point[9] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x + 1) * 10 + y + 1, x * 10 + y);
                                    }
                                }
                            }
                        }
                        else if (i == 7 || i == 8)	//ヒヨコを動かす場合
                        {
                            if ((i == 7 && current_niwatori[0] == 1) || (i == 8 && current_niwatori[1] == 1))
                            {
                                if (x != 0)
                                {
                                    if (current_teams[current_position[x - 1, y]] != 1)
                                    {
                                        moves_point[4] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x - 1) * 10 + y, x * 10 + y);
                                    }
                                    if (y != 0)
                                    {
                                        if (current_teams[current_position[x - 1, y - 1]] != 1)
                                        {
                                            moves_point[1] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x - 1) * 10 + y - 1, x * 10 + y);
                                        }
                                    }
                                }
                                if (x != 2)
                                {
                                    if (current_teams[current_position[x + 1, y]] != 1)
                                    {
                                        moves_point[6] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x + 1) * 10 + y, x * 10 + y);
                                    }
                                    if (y != 0)
                                    {
                                        if (current_teams[current_position[x + 1, y - 1]] != 1)
                                        {
                                            moves_point[3] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, (x + 1) * 10 + y - 1, x * 10 + y);
                                        }
                                    }
                                }
                                if (y != 3)
                                {
                                    if (current_teams[current_position[x, y + 1]] != 1)
                                    {
                                        moves_point[8] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, x * 10 + y + 1, x * 10 + y);
                                    }
                                }
                            }
                            if (y != 0)
                            {
                                if (current_teams[current_position[x, y - 1]] != 1)
                                {
                                    moves_point[2] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, x * 10 + y - 1, x * 10 + y);
                                }
                            }
                        }
                    }
                    else if (x == 3 || x == 4)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                if (current_teams[current_position[j, k]] == 0)
                                {
                                    if (k != 3)
                                    {
                                        moves_point[j * 3 + k + 1] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, j * 10 + k, x * 10 + y);
                                    }
                                    else if (k == 3)
                                    {
                                        if (i != 7 && i != 8)
                                        {
                                            moves_point[j * 3 + k + 1] = prediction_move(turns * -1, current_position, current_teams, niwatori, i, j * 10 + k, x * 10 + y);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (turns == -1 && team[i] == -1) //後手の場合
                {
                    int x = 0, y = 0;
                    for (int j = 0; j < 7; j++)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            if (current_position[j, k] == i)
                            {
                                x = j;
                                y = k;
                            }
                        }
                    }
                    if (x < 3)
                    {
                        if (i == 2)		//ライオンを動かす場合
                        {
                            if (x != 0)
                            {
                                if (current_teams[current_position[x - 1, y]] != -1)
                                {
                                    moves_point[4] = position_point(turns, current_position, current_teams, niwatori, i, (x - 1) * 10 + y, x * 10 + y);
                                }
                                if (y != 0)
                                {
                                    if (current_teams[current_position[x - 1, y - 1]] != -1)
                                    {
                                        moves_point[1] = position_point(turns, current_position, current_teams, niwatori, i, (x - 1) * 10 + y - 1, x * 10 + y);
                                    }
                                }
                                if (y != 3)
                                {
                                    if (current_teams[current_position[x - 1, y + 1]] != -1)
                                    {
                                        moves_point[7] = position_point(turns, current_position, current_teams, niwatori, i, (x - 1) * 10 + y + 1, x * 10 + y);
                                    }
                                }
                            }
                            if (x != 2)
                            {
                                if (current_teams[current_position[x + 1, y]] != -1)
                                {
                                    moves_point[6] = position_point(turns, current_position, current_teams, niwatori, i, (x + 1) * 10 + y, x * 10 + y);
                                }
                                if (y != 0)
                                {
                                    if (current_teams[current_position[x + 1, y - 1]] != -1)
                                    {
                                        moves_point[3] = position_point(turns, current_position, current_teams, niwatori, i, (x + 1) * 10 + y - 1, x * 10 + y);
                                    }
                                }
                                if (y != 3)
                                {
                                    if (current_teams[current_position[x + 1, y + 1]] != -1)
                                    {
                                        moves_point[9] = position_point(turns, current_position, current_teams, niwatori, i, (x + 1) * 10 + y + 1, x * 10 + y);
                                    }
                                }
                            }
                            if (y != 0)
                            {
                                if (current_teams[current_position[x, y - 1]] != -1)
                                {
                                    moves_point[2] = position_point(turns, current_position, current_teams, niwatori, i, x * 10 + y - 1, x * 10 + y);
                                }
                            }
                            if (y != 3)
                            {
                                if (current_teams[current_position[x, y + 1]] != -1)
                                {
                                    moves_point[8] = position_point(turns, current_position, current_teams, niwatori, i, x * 10 + y + 1, x * 10 + y);
                                }
                            }
                        }
                        else if (i == 3 || i == 4)	//キリンを動かす場合
                        {
                            if (x != 0)
                            {
                                if (current_teams[current_position[x - 1, y]] != -1)
                                {
                                    moves_point[4] = position_point(turns, current_position, current_teams, niwatori, i, (x - 1) * 10 + y, x * 10 + y);
                                }
                            }
                            if (x != 2)
                            {
                                if (current_teams[current_position[x + 1, y]] != -1)
                                {
                                    moves_point[6] = position_point(turns, current_position, current_teams, niwatori, i, (x + 1) * 10 + y, x * 10 + y);
                                }
                            }
                            if (y != 0)
                            {
                                if (current_teams[current_position[x, y - 1]] != -1)
                                {
                                    moves_point[2] = position_point(turns, current_position, current_teams, niwatori, i, x * 10 + y - 1, x * 10 + y);
                                }
                            }
                            if (y != 3)
                            {
                                if (current_teams[current_position[x, y + 1]] != -1)
                                {
                                    moves_point[8] = position_point(turns, current_position, current_teams, niwatori, i, x * 10 + y + 1, x * 10 + y);
                                }
                            }
                        }
                        else if (i == 5 || i == 6)	//ゾウを動かす場合
                        {
                            if (x != 0)
                            {
                                if (y != 0)
                                {
                                    if (current_teams[current_position[x - 1, y - 1]] != -1)
                                    {
                                        moves_point[1] = position_point(turns, current_position, current_teams, niwatori, i, (x - 1) * 10 + y - 1, x * 10 + y);
                                    }
                                }
                                if (y != 3)
                                {
                                    if (current_teams[current_position[x - 1, y + 1]] != -1)
                                    {
                                        moves_point[7] = position_point(turns, current_position, current_teams, niwatori, i, (x - 1) * 10 + y + 1, x * 10 + y);
                                    }
                                }
                            }
                            if (x != 2)
                            {
                                if (y != 0)
                                {
                                    if (current_teams[current_position[x + 1, y - 1]] != -1)
                                    {
                                        moves_point[3] = position_point(turns, current_position, current_teams, niwatori, i, (x + 1) * 10 + y - 1, x * 10 + y);
                                    }
                                }
                                if (y != 3)
                                {
                                    if (current_teams[current_position[x + 1, y + 1]] != -1)
                                    {
                                        moves_point[9] = position_point(turns, current_position, current_teams, niwatori, i, (x + 1) * 10 + y + 1, x * 10 + y);
                                    }
                                }
                            }
                        }
                        else if (i == 7 || i == 8)	//ヒヨコを動かす場合
                        {
                            if ((i == 7 && niwatori_flag[0] == 1) || (i == 8 && niwatori_flag[1] == 1))
                            {
                                if (x != 0)
                                {
                                    if (current_teams[current_position[x - 1, y]] != -1)
                                    {
                                        moves_point[4] = position_point(turns, current_position, current_teams, niwatori, i, (x - 1) * 10 + y, x * 10 + y);
                                    }
                                    if (y != 3)
                                    {
                                        if (current_teams[current_position[x - 1, y + 1]] != -1)
                                        {
                                            moves_point[7] = position_point(turns, current_position, current_teams, niwatori, i, (x - 1) * 10 + y + 1, x * 10 + y);
                                        }
                                    }
                                }
                                if (x != 2)
                                {
                                    if (current_teams[current_position[x + 1, y]] != -1)
                                    {
                                        moves_point[6] = position_point(turns, current_position, current_teams, niwatori, i, (x + 1) * 10 + y, x * 10 + y);
                                    }
                                    if (y != 3)
                                    {
                                        if (current_teams[current_position[x + 1, y + 1]] != -1)
                                        {
                                            moves_point[9] = position_point(turns, current_position, current_teams, niwatori, i, (x + 1) * 10 + y + 1, x * 10 + y);
                                        }
                                    }
                                }
                                if (y != 0)
                                {
                                    if (current_teams[current_position[x, y - 1]] != -1)
                                    {
                                        moves_point[2] = position_point(turns, current_position, current_teams, niwatori, i, x * 10 + y - 1, x * 10 + y);
                                    }
                                }
                            }
                            if (y != 3)
                            {
                                if (current_teams[current_position[x, y + 1]] != -1)
                                {
                                    moves_point[8] = position_point(turns, current_position, current_teams, niwatori, i, x * 10 + y + 1, x * 10 + y);
                                }
                            }
                        }
                    }
                    else if (x == 5 || x == 6)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                if (current_teams[current_position[j, k]] == 0)
                                {
                                    if (k != 0)
                                    {
                                        moves_point[j * 3 + k + 1] = position_point(turns, current_position, current_teams, niwatori, i, j * 10 + k, x * 10 + y);
                                    }
                                    else if (k == 0)
                                    {
                                        if (i != 7 && i != 8)
                                        {
                                            moves_point[j * 3 + k + 1] = position_point(turns, current_position, current_teams, niwatori, i, j * 10 + k, x * 10 + y);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (turns == 1)
                {
                    for (int j = 1; j < 13; j++)
                    {
                        if (point >= moves_point[j])
                        {
                            point = moves_point[j];
                        }
                    }
                }
                else if (turns == -1)
                {
                    for (int j = 1; j < 13; j++)
                    {
                        if (point <= moves_point[j])
                        {
                            point = moves_point[j];
                        }
                    }
                }
            }
            return point;
        }

        int position_point(int turns, int[,] positions, int[] teams, int[] niwatori, int piece, int move_to, int move_from) // turn 1:先手 -1:後手 
        {
            int[,] current_position = new int[7, 4];
            int[] current_teams = new int[9];
            int[] current_niwatori = new int[2];
            int point = 0;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    current_position[i, j] = positions[i, j];
                }
            }
            current_teams[0] = 0;
            for (int i = 1; i < 8; i++) current_teams[i] = teams[i];
            current_niwatori[0] = niwatori[0];
            current_niwatori[1] = niwatori[1];

            if (piece != 0)
            {
                if (current_position[move_to / 10, move_to % 10] != 0)
                {
                    int two = 0, j, k = 0;
                    for (j = 1; j <= 2 && position[j + 4, k] != 0; j++)
                    {
                        for (k = 1; k < 4 && position[j + 4, k] != 0; k++) ;
                        if (k == 4)
                        {
                            k = 0; two = 1;
                        }
                    }
                    if (j != 1) j = j - 1;
                    current_position[j + two + 4, j] = current_position[move_to / 10, move_to % 10];
                    current_teams[current_position[move_to / 10, move_to % 10]] *= -1;
                    if (current_position[move_to / 10, move_to % 10] == 7 || current_position[move_to / 10, move_to % 10] == 8)
                    {
                        niwatori[current_position[move_to / 10, move_to % 10] - 7] = 0;
                    }
                }
                current_position[move_to / 10, move_to % 10] = piece;
                current_position[move_from / 10, move_from % 10] = 0;
            }
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i < 3)
                    {
                        if (current_position[i, j] != 0)
                        {
                            if (current_teams[current_position[i, j]] == -1)
                            {
                                switch (current_position[i, j])
                                {
                                    case 2:
                                        point += 3 + j + 1;
                                        break;
                                    case 3:
                                    case 4:
                                    case 5:
                                    case 6:
                                        point += 2 + ((j + 1) * 2 / 3);
                                        break;
                                    case 7:
                                        point += 1 + ((j + 1) * 2 / 3);
                                        if (niwatori[0] == 1) point += 2;
                                        break;
                                    case 8:
                                        point += 1 + ((j + 1) / 2);
                                        if (niwatori[0] == 1) point += 2;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else if (current_teams[current_position[i, j]] == 1)
                            {
                                int a = 0;
                                switch (j)
                                {
                                    case 0: a = 3;
                                        break;
                                    case 1: a = 2;
                                        break;
                                    case 2: a = 1;
                                        break;
                                    case 3: a = 0;
                                        break;
                                    default:
                                        break;
                                }
                                switch (current_position[i, j])
                                {
                                    case 1:
                                        point -= 3 - a;
                                        break;
                                    case 3:
                                    case 4:
                                    case 5:
                                    case 6:
                                        point -= 2 - ((a + 1) * 2 / 3);
                                        break;
                                    case 7:
                                        point -= 1 - ((a + 1) * 2 / 3);
                                        if (niwatori[0] == 1) point -= 2;
                                        break;
                                    case 8:
                                        point -= 1 - ((a + 1) / 2);
                                        if (niwatori[0] == 1) point -= 2;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    else if (i == 3 || i == 4)
                    {
                        if (current_position[i, j] != 0)
                        {
                            if (current_teams[current_position[i, j]] == 1)
                            {
                                switch (current_position[i, j])
                                {
                                    case 3:
                                    case 4:
                                    case 5:
                                    case 6:
                                        point -= 2;
                                        break;
                                    case 7:
                                        point -= 1;
                                        break;
                                    case 8:
                                        point -= 1;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    else if (i == 5 || i == 6)
                    {
                        if (current_position[i, j] != 0)
                        {
                            switch (current_position[i, j])
                            {
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                    point += 2;
                                    break;
                                case 7:
                                    point += 1;
                                    break;
                                case 8:
                                    point += 1;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            return point;
        }

        int data_move()
        {
            for (int i = 0; i < 414; i++)
            {
                int[,] move_result = new int[4, 20];
                if (team[win_results[i, 16]] == -1)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        move_result[0, j] = win_results[i, j];
                        if (j % 3 == 0)
                        {
                            move_result[1, j] = win_results[i, j + 2];
                            if (j < 3)
                            {
                                move_result[3, j] = win_results[i, j + 8];
                            }
                            else if (j >= 3 && j < 6)
                            {
                                move_result[3, j] = win_results[i, j + 5];
                            }
                            else if (j >= 6 && j < 9)
                            {
                                move_result[3, j] = win_results[i, j - 5];
                            }
                            else if (j >= 9)
                            {
                                move_result[3, j] = win_results[i, j - 8];
                            }
                        }
                        else if (j % 3 == 2)
                        {
                            move_result[1, j] = win_results[i, j - 2];
                            if (j < 3)
                            {
                                move_result[3, j] = win_results[i, j + 4];
                            }
                            else if (j >= 3 && j < 6)
                            {
                                move_result[3, j] = win_results[i, j + 1];
                            }
                            else if (j >= 6 && j < 9)
                            {
                                move_result[3, j] = win_results[i, j - 1];
                            }
                            else if (j >= 9)
                            {
                                move_result[3, j] = win_results[i, j - 4];
                            }
                        }
                        else
                        {
                            move_result[1, j] = win_results[i, j];
                            if (j < 3)
                            {
                                move_result[3, j] = win_results[i, j + 6];
                            }
                            else if (j >= 3 && j < 6)
                            {
                                move_result[3, j] = win_results[i, j + 3];
                            }
                            else if (j >= 6 && j < 9)
                            {
                                move_result[3, j] = win_results[i, j - 3];
                            }
                            else if (j >= 9)
                            {
                                move_result[3, j] = win_results[i, j - 6];
                            }
                        }

                        if (j < 3)
                        {
                            move_result[2, j] = win_results[i, j + 6];
                        }
                        else if (j >= 3 && j < 6)
                        {
                            move_result[2, j] = win_results[i, j + 3];
                        }
                        else if (j >= 6 && j < 9)
                        {
                            move_result[2, j] = win_results[i, j - 3];
                        }
                        else if (j >= 9)
                        {
                            move_result[2, j] = win_results[i, j - 6];
                        }
                    }
                    for (int j = 12; j < 20; j++)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            move_result[k, j] = win_results[i, j];
                        }
                    }
                    int same = 0;
                    for (int j = 0; j < 4; j++)
                    {
                        for (int k = 0; k < 12 && same == 0; k++)
                        {
                            if (move_result[j, k] != position[k / 3, k % 3])
                            {
                                same = 1;
                            }
                        }
                        if (same == 0)
                        {
                            int x, y;
                            x = move_result[j, 17];
                            y = move_result[j, 18];
                            if (j == 0)
                            {

                                return move_result[j, 16] * 100 + x * 10 + y;
                            }
                            else if (j == 1)
                            {
                                if (x == 0)
                                {
                                    x = 2;
                                }
                                else if (x == 2)
                                {
                                    x = 0;
                                }
                                return move_result[j, 16] * 100 + x * 10 + y;
                            }
                            else if (j == 2)
                            {
                                if (y == 0)
                                {
                                    y = 3;
                                }
                                else if (y == 1)
                                {
                                    y = 2;
                                }
                                else if (y == 2)
                                {
                                    y = 1;
                                }
                                else if (y == 3)
                                {
                                    y = 0;
                                }
                                return move_result[j, 16] * 100 + x * 10 + y;
                            }
                            else if (j == 3)
                            {
                                if (x == 0)
                                {
                                    x = 2;
                                }
                                else if (x == 2)
                                {
                                    x = 0;
                                }

                                if (y == 0)
                                {
                                    y = 3;
                                }
                                else if (y == 1)
                                {
                                    y = 2;
                                }
                                else if (y == 2)
                                {
                                    y = 1;
                                }
                                else if (y == 3)
                                {
                                    y = 0;
                                }
                                return move_result[j, 16] * 100 + x * 10 + y;
                            }
                        }
                        same = 0;
                    }
                }
            }
            return 999;
        }

        private void piece_refresh()
        {
            lion1.Refresh();
            lion2.Refresh();
            kirin1.Refresh();
            kirin2.Refresh();
            zou1.Refresh();
            zou2.Refresh();
            hiyoko1.Refresh();
            hiyoko2.Refresh();
            return;
        }
        private void lion1_MouseDown(object sender, MouseEventArgs e)
        {
            Graphics g = this.CreateGraphics();
            if (end == 0)
            {
                int mouse_x, mouse_y, place;
                Image image;
                mouse_x = System.Windows.Forms.Cursor.Position.X;
                mouse_y = System.Windows.Forms.Cursor.Position.Y;
                place = click_place(mouse_x, mouse_y);
                image = Image.FromFile(@"../../\Resources\\lion-go.bmp");
                g.DrawImage(image, 330, 550);
           
                      
                if (hand_from == 99 && place != 99 && place != hand_from && turn == team[1])
                {
                    hand_from = piece_x[1] * 10 + piece_y[1];
                    hand_pice = 1;
             
                }
                else if (place == hand_from)
                {
                    hand_from = 99;
  
                }
                else
                {
                    have[move_count, 0] = have[move_count - 1, 0];
                    have[move_count, 1] = have[move_count - 1, 1];
                    have[move_count, 2] = have[move_count - 1, 2];
                    hand_to = place;
                    move_piece(hand_from, hand_to, hand_pice);
                    hand_from = 99;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            position_turn[move_count, i, j] = position[i, j];
                        }
                    }
                    if (GAME_MODE == 1 && end == 0 && turn == -1)
                    {
                        select_text(hand_from);
                        have[move_count, 0] = have[move_count - 1, 0];
                        have[move_count, 1] = have[move_count - 1, 1];
                        have[move_count, 2] = have[move_count - 1, 2];
                        int hand = AI_move();
                        move_piece(piece_x[hand / 100] * 10 + piece_y[hand / 100], hand % 100, hand / 100);
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                position_turn[move_count, i, j] = position[i, j];
                            }
                        }
                    }
                }
                select_text(hand_from);
            }
        }

        private void lion2_MouseDown(object sender, MouseEventArgs e)
        {
         
            if (end == 0)
            {
                textBox1.Text = string.Empty;
                int mouse_x, mouse_y, place;
                Image image;
                Graphics g = this.CreateGraphics();
                mouse_x = System.Windows.Forms.Cursor.Position.X;
                mouse_y = System.Windows.Forms.Cursor.Position.Y;
                place = click_place(mouse_x, mouse_y);
                image = Image.FromFile(@"../../\Resources\\lion-go.bmp");
                g.DrawImage(image, 330, 550);


                if (hand_from == 99 && place != 99 && place != hand_from && turn == team[2])
                {
                    hand_from = piece_x[2] * 10 + piece_y[2];
                    hand_pice = 2;
                  
                }
                else if (place == hand_from)
                {
                    hand_from = 99;

   
                }
                else
                {
                    have[move_count, 0] = have[move_count - 1, 0];
                    have[move_count, 1] = have[move_count - 1, 1];
                    have[move_count, 2] = have[move_count - 1, 2];
                    hand_to = place;
                    move_piece(hand_from, hand_to, hand_pice);
                    hand_from = 99;
                   
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            position_turn[move_count, i, j] = position[i, j];
                        }
                    }
                    if (GAME_MODE == 1 && end == 0 &&turn == -1)
                    {
                        select_text(hand_from);
                        have[move_count, 0] = have[move_count - 1, 0];
                        have[move_count, 1] = have[move_count - 1, 1];
                        have[move_count, 2] = have[move_count - 1, 2];
                        int hand = AI_move();
                        move_piece(piece_x[hand / 100] * 10 + piece_y[hand / 100], hand % 100, hand / 100);
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                position_turn[move_count, i, j] = position[i, j];
                            }
                        }
                    }
                }
                select_text(hand_from);
            }
        }

        private void kirin1_MouseDown(object sender, MouseEventArgs e)
        {
            if (end == 0)
            {
                int mouse_x, mouse_y, place;
                Image image;
                Graphics g = this.CreateGraphics();
                mouse_x = System.Windows.Forms.Cursor.Position.X;
                mouse_y = System.Windows.Forms.Cursor.Position.Y;
                place = click_place(mouse_x, mouse_y);
                image = Image.FromFile(@"../../\Resources\\kirin-go.bmp");
                g.DrawImage(image, 330, 550);

         
             
                if (hand_from == 99 && place != 99 && place != hand_from && turn == team[3])
                {
                    hand_from = piece_x[3] * 10 + piece_y[3];
                    hand_pice = 3;
                }
                else if (place == hand_from)
                {
                    hand_from = 99;
                  
                }
                else
                {
                    have[move_count, 0] = have[move_count - 1, 0];
                    have[move_count, 1] = have[move_count - 1, 1];
                    have[move_count, 2] = have[move_count - 1, 2];
                    hand_to = place;
                    move_piece(hand_from, hand_to, hand_pice);
                    hand_from = 99;

                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            position_turn[move_count, i, j] = position[i, j];
                        }
                    }
                    if (GAME_MODE == 1 && end == 0 && turn == -1)
                    {
                        select_text(hand_from);
                        have[move_count, 0] = have[move_count - 1, 0];
                        have[move_count, 1] = have[move_count - 1, 1];
                        have[move_count, 2] = have[move_count - 1, 2];
                        int hand = AI_move();
                        move_piece(piece_x[hand / 100] * 10 + piece_y[hand / 100], hand % 100, hand / 100);
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                position_turn[move_count, i, j] = position[i, j];
                            }
                        }
                    }
                }
                select_text(hand_from);
            }
        }

        private void kirin2_MouseDown(object sender, MouseEventArgs e)
        {
            if (end == 0)
            {
                int mouse_x, mouse_y, place;
                Image image;
                Graphics g = this.CreateGraphics();
                mouse_x = System.Windows.Forms.Cursor.Position.X;
                mouse_y = System.Windows.Forms.Cursor.Position.Y;
                place = click_place(mouse_x, mouse_y);
                image = Image.FromFile(@"../../\Resources\\kirin-go.bmp");
                g.DrawImage(image, 330, 550);
                


                if (hand_from == 99 && place != 99 && place != hand_from && turn == team[4])
                {
                    hand_from = piece_x[4] * 10 + piece_y[4];
                    hand_pice = 4;
   
                }
                else if (place == hand_from)
                {
                    hand_from = 99;
                
                }
                else
                {
                    have[move_count, 0] = have[move_count - 1, 0];
                    have[move_count, 1] = have[move_count - 1, 1];
                    have[move_count, 2] = have[move_count - 1, 2];
                    hand_to = place;
                    move_piece(hand_from, hand_to, hand_pice);
                    hand_from = 99;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            position_turn[move_count, i, j] = position[i, j];
                                  
                        }
                    }
                    if (GAME_MODE == 1 && end == 0 && turn == -1)
                    {
                        select_text(hand_from);
                        have[move_count, 0] = have[move_count - 1, 0];
                        have[move_count, 1] = have[move_count - 1, 1];
                        have[move_count, 2] = have[move_count - 1, 2];
                        int hand = AI_move();
                        move_piece(piece_x[hand / 100] * 10 + piece_y[hand / 100], hand % 100, hand / 100);
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                position_turn[move_count, i, j] = position[i, j];
                            }
                        }
                    }
                }
                select_text(hand_from);
            }
        }

        private void zou1_MouseDown(object sender, MouseEventArgs e)
        {
            if (end == 0)
            {
                int mouse_x, mouse_y, place;
                Image image;
                Graphics g = this.CreateGraphics();
                mouse_x = System.Windows.Forms.Cursor.Position.X;
                mouse_y = System.Windows.Forms.Cursor.Position.Y;
                place = click_place(mouse_x, mouse_y);
                image = Image.FromFile(@"../../\Resources\\zou-go.bmp");
                g.DrawImage(image, 330, 550);
               
                if (hand_from == 99 && place != 99 && place != hand_from && turn == team[5])
                {
                    hand_from = piece_x[5] * 10 + piece_y[5];
                    hand_pice = 5;
               
                }
                else if (place == hand_from)
                {
                    hand_from = 99;
                 
                }
                else if (place < 30)
                {
                    have[move_count, 0] = have[move_count - 1, 0];
                    have[move_count, 1] = have[move_count - 1, 1];
                    have[move_count, 2] = have[move_count - 1, 2];
                    hand_to = place;
                    move_piece(hand_from, hand_to, hand_pice);
                    hand_from = 99;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            position_turn[move_count, i, j] = position[i, j];
                         
                        }
                    }
                    if (GAME_MODE == 1 && end == 0 && turn == -1)
                    {
                        select_text(hand_from);
                        have[move_count, 0] = have[move_count - 1, 0];
                        have[move_count, 1] = have[move_count - 1, 1];
                        have[move_count, 2] = have[move_count - 1, 2];
                        int hand = AI_move();
                        move_piece(piece_x[hand / 100] * 10 + piece_y[hand / 100], hand % 100, hand / 100);
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                position_turn[move_count, i, j] = position[i, j];
                            }
                        }
                    }
                }
                select_text(hand_from);
            }
        }

        private void zou2_MouseDown(object sender, MouseEventArgs e)
        {
            if (end == 0)
            {
                int mouse_x, mouse_y, place;
                Image image;
                Graphics g = this.CreateGraphics();
                mouse_x = System.Windows.Forms.Cursor.Position.X;
                mouse_y = System.Windows.Forms.Cursor.Position.Y;
                place = click_place(mouse_x, mouse_y);
                image = Image.FromFile(@"../../\Resources\\zou-go.bmp");
                g.DrawImage(image, 330, 550);
                
                if (hand_from == 99 && place != 99 && place != hand_from && turn == team[6])
                {
                    hand_from = piece_x[6] * 10 + piece_y[6];
                    hand_pice = 6;
                 
                }
                else if (place == hand_from)
                {
                    hand_from = 99;
                 
                }
                else if (place < 30)
                {
                    have[move_count, 0] = have[move_count - 1, 0];
                    have[move_count, 1] = have[move_count - 1, 1];
                    have[move_count, 2] = have[move_count - 1, 2];
                    hand_to = place;
                    move_piece(hand_from, hand_to, hand_pice);
                    hand_from = 99;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            position_turn[move_count, i, j] = position[i, j];
                            
                        }
                    }
                    if (GAME_MODE == 1 && end == 0 && turn == -1)
                    {
                        select_text(hand_from);
                        have[move_count, 0] = have[move_count - 1, 0];
                        have[move_count, 1] = have[move_count - 1, 1];
                        have[move_count, 2] = have[move_count - 1, 2];
                        int hand = AI_move();
                        move_piece(piece_x[hand / 100] * 10 + piece_y[hand / 100], hand % 100, hand / 100);
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                position_turn[move_count, i, j] = position[i, j];
                            }
                        }
                    }
                }
                select_text(hand_from);
            }
        }

        private void hiyoko1_MouseDown(object sender, MouseEventArgs e)
        {
            if (end == 0)
            {
                int mouse_x, mouse_y, place;
                Image image;
                Graphics g = this.CreateGraphics();
                mouse_x = System.Windows.Forms.Cursor.Position.X;
                mouse_y = System.Windows.Forms.Cursor.Position.Y;
                place = click_place(mouse_x, mouse_y);
                image = Image.FromFile(@"../../\Resources\\hiyoko-go.bmp");
                g.DrawImage(image, 330,550);
               
                if(niwatori_flag[0] == 1){
                    image = Image.FromFile(@"../../\Resources\\niwatori-go.bmp");
                    g.DrawImage(image, 330, 550);
                }
                if (hand_from == 99 && place != 99 && place != hand_from && turn == team[7])
                {
                    hand_from = piece_x[7] * 10 + piece_y[7];
                    hand_pice = 7;
                  
                }
                else if (place == hand_from || place >25)
                {
                    hand_from = 99;
              
                }
                else
                {
                    have[move_count, 0] = have[move_count - 1, 0];
                    have[move_count, 1] = have[move_count - 1, 1];
                    have[move_count, 2] = have[move_count - 1, 2];
                    hand_to = place;
                    move_piece(hand_from, hand_to, hand_pice);
                    hand_from = 99;
                   
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            position_turn[move_count, i, j] = position[i, j];
                        
                        }
                    }
                    if (GAME_MODE == 1 && end == 0 && turn == -1)
                    {
                        select_text(hand_from);
                        have[move_count, 0] = have[move_count - 1, 0];
                        have[move_count, 1] = have[move_count - 1, 1];
                        have[move_count, 2] = have[move_count - 1, 2];
                        int hand = AI_move();
                        move_piece(piece_x[hand / 100] * 10 + piece_y[hand / 100], hand % 100, hand / 100);
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                position_turn[move_count, i, j] = position[i, j];
                            }
                        }
                    }
                }
                select_text(hand_from);
            }
        }

        private void hiyoko2_MouseDown(object sender, MouseEventArgs e)
        {
            if (end == 0)
            {
                int mouse_x, mouse_y, place;
                Image image;
                Graphics g = this.CreateGraphics();
                mouse_x = System.Windows.Forms.Cursor.Position.X;
                mouse_y = System.Windows.Forms.Cursor.Position.Y;
                place = click_place(mouse_x, mouse_y);
                image = Image.FromFile(@"../../\Resources\\hiyoko-go.bmp");
                g.DrawImage(image, 330, 550);
                if (niwatori_flag[1] == 1)
                {
                    image = Image.FromFile(@"../../\Resources\\niwatori-go.bmp");
                    g.DrawImage(image, 330, 550);
                }
                if (hand_from == 99 && place != 99 && place != hand_from && turn == team[8])
                {
                    hand_from = piece_x[8] * 10 + piece_y[8];
                    hand_pice = 8;
                }
                else if (place == hand_from || place > 25)
                {
                    hand_from = 99;
          
                }
                else
                {
                    have[move_count, 0] = have[move_count - 1, 0];
                    have[move_count, 1] = have[move_count - 1, 1];
                    have[move_count, 2] = have[move_count - 1, 2];
                    hand_to = place;
                    move_piece(hand_from, hand_to, hand_pice);
                    hand_from = 99;
                    
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            position_turn[move_count, i, j] = position[i, j];
                          
                        }
                    }
                    if (GAME_MODE == 1 && end == 0)
                    {
                        select_text(hand_from);
                        have[move_count, 0] = have[move_count - 1, 0];
                        have[move_count, 1] = have[move_count - 1, 1];
                        have[move_count, 2] = have[move_count - 1, 2];
                        int hand = AI_move();
                        move_piece(piece_x[hand / 100] * 10 + piece_y[hand / 100], hand % 100, hand / 100);
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                position_turn[move_count, i, j] = position[i, j];
                            }
                        }
                    }
                    if (GAME_MODE == 1 && end == 0 && turn == -1)
                    {
                        select_text(hand_from);
                        have[move_count, 0] = have[move_count - 1, 0];
                        have[move_count, 1] = have[move_count - 1, 1];
                        have[move_count, 2] = have[move_count - 1, 2];
                        int hand = AI_move();
                        move_piece(piece_x[hand / 100] * 10 + piece_y[hand / 100], hand % 100, hand / 100);
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                position_turn[move_count, i, j] = position[i, j];
                            }
                        }
                    }
                }
                select_text(hand_from);
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (end == 0)
            {
                int mouse_x, mouse_y, place;
                mouse_x = System.Windows.Forms.Cursor.Position.X;
                mouse_y = System.Windows.Forms.Cursor.Position.Y;
                place = click_place(mouse_x, mouse_y);
                if (hand_from != 99 && place < 30)
                {
                    have[move_count, 0] = have[move_count - 1, 0];
                    have[move_count, 1] = have[move_count - 1, 1];
                    have[move_count, 2] = have[move_count - 1, 2];
                    hand_to = place;
                    move_piece(hand_from, hand_to, hand_pice);
                    hand_from = 99;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            position_turn[move_count, i, j] = position[i, j];
                        }
                    }
                    if (GAME_MODE == 1 && end == 0 && turn == -1)
                    {
                        select_text(hand_from);
                        have[move_count, 0] = have[move_count - 1, 0];
                        have[move_count, 1] = have[move_count - 1, 1];
                        have[move_count, 2] = have[move_count - 1, 2];
                        int hand = AI_move();
                        move_piece(piece_x[hand / 100] * 10 + piece_y[hand / 100], hand % 100, hand / 100);
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                position_turn[move_count, i, j] = position[i, j];
                            }
                        }
                    }
                }
                else
                {
                    hand_from = 99;
                }
                select_text(hand_from);
            }
        }

        private void 対人対戦ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GAME_MODE = 0;
        }

        private void コンピュータ対戦ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GAME_MODE = 1;
        }

        private void 先手ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (move_count == 1) turn = 1;
            GAME_MODE = 1;
        }

        private void 後手ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (move_count == 1)
            {
                turn = -1;
                turn_side = -1;
                turn_text(turn);
                have[move_count, 0] = have[move_count - 1, 0];
                have[move_count, 1] = have[move_count - 1, 1];
                have[move_count, 2] = have[move_count - 1, 2];
                int hand = AI_move();
                move_piece(piece_x[hand / 100] * 10 + piece_y[hand / 100], hand % 100, hand / 100);
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        position_turn[move_count, i, j] = position[i, j];
                    }
                }
            }
            GAME_MODE = 1;
        }
    }
}