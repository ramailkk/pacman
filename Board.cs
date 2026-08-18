using System;
using System.Collections.Generic;

namespace PacManGame
{
    public class Board
    {
        private static readonly Dictionary<int, (TileType Type, bool IsRedZone)> IntToTile = new()
        {
            { 0, (TileType.Empty, false) },
            { 1, (TileType.Wall, false) },
            { 2, (TileType.Dot, false) },
            { 3, (TileType.PowerPellet, false) },
            { 4, (TileType.Fruit, false) },
            { 5, (TileType.GhostHouse, false) },
            { 6, (TileType.DeadSpace, false) },
            { 7, (TileType.Tunnel, false) },
            { 8, (TileType.Empty, true) },   // red zone, nothing in it — same code as before
            { 9, (TileType.HouseGate, false) },
            { 10, (TileType.Dot, true) },    // NEW — red zone with a dot in it
        };

        public int TileHeight { get; }
        public int TileWidth { get; }
        public int Score { get; set; }
        public int TotalDots { get; private set; }
        public int TotalEnergizers { get; private set; }
        public int TotalSmallDots { get; private set; }
        public List<(int x, int y)> EnergizersCords;
        public int RemainingDots { get; private set; }
        public Tile[,] Grid { get; private set; }
        public int[][] Reference;
        public int LEVEL;
        public Random rng;
        private const int Seed = 12345;
        public Dictionary<(int col, int row), char> ReadyMessage;
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
            rng = new Random(Seed);
            EnergizersCords = new List<(int x, int y)>(4);
            Grid = SetupBoard(reference);
            LEVEL = 1;
        }
        private Tile[,] SetupBoard(int[][] reference)
        {
            Tile[,] tiles = new Tile[reference.Length, reference[0].Length];
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    int value = reference[i][j];
                    (TileType type, bool isRedZone) = IntToTile[value];
                    tiles[i, j] = new Tile(TileHeight, TileWidth, type, isRedZone);
                    if (type == TileType.Dot)
                        TotalSmallDots++;
                    else if (type == TileType.PowerPellet)
                    {
                        EnergizersCords.Add((i, j));
                        TotalEnergizers++;
                    }
                }
            }
            rng = new Random(Seed);
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
        public void UpdateFruitScore()
        {
            Score += LevelSpecs.GetEntry(LEVEL, LevelSpecs.BonusPoints);
        }
        public void SetupNextLevel()
        {
            TotalDots = 0;
            TotalEnergizers = 0;
            TotalSmallDots = 0;
            RemainingDots = 0;
            Grid = SetupBoard(Reference);
            LEVEL++;
        }
    }
}