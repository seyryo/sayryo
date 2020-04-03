﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Breakout
{
    public partial class Form1 : Form
    {
        Vector ballPos; //位置(Vector：2D空間における変位を表す)
        Vector ballSpeed;
        int ballRadius; //半径
        Rectangle paddlePos; //パドル位置(Rectangle:四角形を作成)
        List<Rectangle> blockPos; //ブロックの位置(リスト化)

        public Form1()
        {
            InitializeComponent(); //設定したハンドラ等の初期設定

            this.ballPos = new Vector(200, 200);
            this.ballSpeed = new Vector(-4, -8);
            this.ballRadius = 10;
            this.paddlePos = new Rectangle(100, this.Height - 50, 100, 5); //(位置横縦,サイズ横縦)
            this.blockPos = new List<Rectangle>();
            for (int x = 0; x <= this.Height; x += 100)
            {
                for (int y = 0; y <= 150; y += 40)
                {
                    this.blockPos.Add(new Rectangle(25 + x, y, 80, 25));
                }
            }
            Timer timer = new Timer();
            timer.Interval = 33;
            timer.Tick += new EventHandler(Update); //timer.Trik：Timer有効時に呼ばれる
            timer.Start();
        }

        /// <summary>
        /// 内積計算
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        double DotProduct(Vector a, Vector b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        /// <summary>
        /// 当たり判定
        /// </summary>
        /// <param name="p1">パドル左端座標</param>
        /// <param name="p2">パドル右端座標</param>
        /// <param name="center">ボール中心</param>
        /// <param name="radius">ボール半径</param>
        /// <returns></returns>
        bool LineVsCircle(Vector p1, Vector p2, Vector center, float radius)
        {
            Vector lineDir = (p2 - p1); //パドルのベクトル(パドルの長さ)
            Vector n = new Vector(lineDir.Y, -lineDir.X); //パドルの法線
            n.Normalize();

            Vector dir1 = center - p1;
            Vector dir2 = center - p2;

            double dist = Math.Abs(DotProduct(dir1, n));
            double a1 = DotProduct(dir1, lineDir);
            double a2 = DotProduct(dir2, lineDir);
            Console.WriteLine(dist);

            return (a1 * a2 < 0 && dist < radius) ? true : false;
        }

        int BlockVsCircle(Rectangle block, Vector ball)
        {
            if (LineVsCircle(new Vector(block.Left, block.Top),
                new Vector(block.Right, block.Top), ball, ballRadius))
                return 1;

            if (LineVsCircle(new Vector(block.Left, block.Bottom),
                new Vector(block.Right, block.Bottom), ball, ballRadius))
                return 2;

            if (LineVsCircle(new Vector(block.Right, block.Top),
                new Vector(block.Right, block.Bottom), ball, ballRadius))
                return 3;

            if (LineVsCircle(new Vector(block.Left, block.Top),
                new Vector(block.Left, block.Bottom), ball, ballRadius))
                return 4;

            return -1;
        }

        private void Update(object sender, EventArgs e)
        {
            //ボールの移動
            ballPos += ballSpeed;

            //左右の壁でのバウンド
            if (ballPos.X + ballRadius * 2 > this.Bounds.Width || ballPos.X - ballRadius < 0)
            {
                ballSpeed.X *= -1;
            }

            //上の壁でバウンド
            if (ballPos.Y - ballRadius < 0)
            {
                ballSpeed.Y *= -1;
            }

            //パドルの当たり判定
            if (LineVsCircle(new Vector(this.paddlePos.Left, this.paddlePos.Top),
                             new Vector(this.paddlePos.Right, this.paddlePos.Top),
                             ballPos, ballRadius)
                )
            {
                ballSpeed.Y *= -1;
            }

            // ブロックとのあたり判定
            for (int i = 0; i < this.blockPos.Count; i++)
            {
                int collision = BlockVsCircle(blockPos[i], ballPos);
                if (collision == 1 || collision == 2)
                {
                    ballSpeed.Y *= -1;
                    this.blockPos.Remove(blockPos[i]);
                }
                else if (collision == 3 || collision == 4)
                {
                    ballSpeed.X *= -1;
                    this.blockPos.Remove(blockPos[i]);
                }
            }

            //画面再描画
            Invalidate();
        }

        private void Draw(object sender, PaintEventArgs e) //Draw意味:描画する
        {
            SolidBrush pinkBrush = new SolidBrush(Color.HotPink); //SolidBrush(ブラシ)は.Netのクラスで図形を塗り潰す
            SolidBrush grayBrush = new SolidBrush(Color.DimGray);
            SolidBrush blueBrush = new SolidBrush(Color.LightBlue);

            //左上からの位置を設定
            float px = (float)this.ballPos.X - ballRadius; //マイナス半径とすることで円の中心になる
            float py = (float)this.ballPos.Y - ballRadius;

            //e.描画.円(色, 横, 縦, 物質幅, 物質高さ)
            e.Graphics.FillEllipse(pinkBrush, px, py, this.ballRadius * 2, this.ballRadius * 2);
            //e.描画.長方形(色, 長方形)
            e.Graphics.FillRectangle(grayBrush, paddlePos);
            //ブロック描画
            for (int i = 0; i < this.blockPos.Count; i++)
            {
                e.Graphics.FillRectangle(blueBrush, blockPos[i]);
            }
        }

        private void KeyPressed(object sender, KeyPressEventArgs e) //押下毎
        {
            if (e.KeyChar == 'a' && paddlePos.Left > 0) //A押下時
            {
                this.paddlePos.X -= 20;
            }
            else if (e.KeyChar == 's' && paddlePos.Right < this.Width) //S押下時
            {
                this.paddlePos.X += 20;
                int a = 5;
            }
        }
    }
}