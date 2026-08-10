using System;
using System.Collections.Generic;

namespace PacManGame
{
    public class Board
    {
        private static readonly Dictionary<int, TileType> IntToTile = new(){
            { 0, TileType.Empty },
            { 1, TileType.Wall },
            { 2, TileType.Dot },
            { 3, TileType.PowerPellet },
            { 4, TileType.Fruit },
            { 5, TileType.GhostHouse },
            { 6, TileType.DeadSpace },
            { 7, TileType.Tunnel},
            { 8, TileType.RedZone},
            { 9, TileType.HouseGate}
        };

        public int TileHeight { get; }
        public int TileWidth { get; }
        public int Score { get; set; }
        public int TotalDots {get; private set;}
        public int TotalEnergizers { get; private set; }
        public int TotalSmallDots { get; private set; }
        public int RemainingDots { get; private set; }
        
        public Tile[,] Grid { get; private set; }
        public int[][] Reference;
        public int LEVEL;

        public Board(int[][] reference, int tileWidth, int tileHeight)
        {
            TileHeight = tileHeight;
            TileWidth = tileWidth;
            Reference = reference;

            Score = 0;
            TotalSmallDots = 0;
            TotalEnergizers = 0;
            TotalDots = 0;
            RemainingDots = 0;

            Grid = SetupBoard(reference);
            LEVEL = 1;
        }
        // public void Main(string[] args)
        // {
        //     Board board = new Board()
        // }
        private Tile[,] SetupBoard(int[][] reference)
        {
            Tile[,] tiles = new Tile[reference.Length, reference[0].Length];

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    int value = reference[i][j];
                    TileType type = IntToTile[value];
                    tiles[i, j] = new Tile(TileHeight, TileWidth, type);
                    if (type == TileType.Dot)
                        TotalSmallDots++;
                    else if (type == TileType.PowerPellet)
                        TotalEnergizers++;
                }
            }
            TotalDots = TotalSmallDots + TotalEnergizers;
            RemainingDots = TotalDots;
            return tiles;
        }
        public void UpdateDotScore()
        {
            Score += 10;
            RemainingDots--;
        }
        public void UpdatePowerScore()
        {
            Score += 50;
            RemainingDots--;
        }
        public void SetupNextLevel()
        {
            Score = 0;
            TotalDots = 0;
            TotalEnergizers = 0;
            TotalSmallDots = 0;
            RemainingDots = 0;
            Grid = SetupBoard(Reference);
            LEVEL++;
        }
    }
}