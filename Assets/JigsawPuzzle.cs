using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    internal enum Direction
    {
        Top,
        Right,
        Bottom,
        Left
    }

    internal enum Connection
    {
        In,
        Out
    }

    internal class JigsawPiece
    {
        public JigsawPiece Top, Right, Bottom, Left;
        public Connection TopD, RightD, BottomD, LeftD;
    }

    internal class JigsawPuzzle
    {
        public JigsawPuzzle(int nx, int ny, JigsawPiece[,] pieces)
        {
            this.Nx = nx;
            this.Ny = ny;
            this.Pieces = pieces;
        }

        public int Nx { get; private set; }
        public int Ny { get; private set; }
        public JigsawPiece[,] Pieces { get; private set; }

        public static JigsawPuzzle Generate(int nx, int ny)
        {
            var pieces = new JigsawPiece[nx, ny];
            for (int i = 0; i < nx; ++i)
            {
                for (int j = 0; j < ny; ++j)
                {
                    pieces[i, j] = new JigsawPiece();
                }
            }

            for (int i = 0; i < nx; ++i)
            {
                pieces[i, 0].Bottom = null;
                pieces[i, ny - 1].Top = null;
            }

            for (int j = 0; j < ny; ++j)
            {
                pieces[0, j].Left = null;
                pieces[nx - 1, j].Right = null;
            }

            for (int x = 1; x < nx; ++x)
            {
                for (int y = 0; y < ny; ++y)
                {
                    bool inDirection = UnityEngine.Random.Range(0f, 1f) < 0.5f;
                    pieces[x - 1, y].Right = pieces[x, y];
                    pieces[x, y].Left = pieces[x - 1, y];

                    pieces[x - 1, y].RightD = inDirection ? Connection.In : Connection.Out;
                    pieces[x, y].LeftD = inDirection ? Connection.Out : Connection.In;
                }
            }

            for (int y = 1; y < ny; ++y)
            {
                for (int x = 0; x < nx; ++x)
                {
                    bool inDirection = UnityEngine.Random.Range(0f, 1f) < 0.5f;
                    pieces[x, y - 1].Top = pieces[x, y];
                    pieces[x, y].Bottom = pieces[x, y - 1];

                    pieces[x, y - 1].TopD = inDirection ? Connection.In : Connection.Out;
                    pieces[x, y].BottomD = inDirection ? Connection.Out : Connection.In;
                }
            }

            return new JigsawPuzzle(nx, ny, pieces);
        }
    }
}