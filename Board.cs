using System;
using System.Collections.Generic;

namespace PacManGame
{
    public class Board
    {
        private static readonly Dictionary<int, TileType> IntToTile = new()
        {
            { 0, TileType.Empty },
            { 1, TileType.Wall },
            { 2, TileType.Dot },
            { 3, TileType.PowerPellet },
            { 4, TileType.Fruit },
            { 5, TileType.GhostHouse },
            { 6, TileType.DeadSpace }
        };

        public int TileHeight { get; }
        public int TileWidth { get; }
        public int Score { get; set; }
        public int PelletCounter { get; private set; }
        public int DotCounter { get; private set; }
        public Tile[,] Grid { get; private set; }

        public Board(int[][] reference, int tileWidth, int tileHeight)
        {
            TileHeight = tileHeight;
            TileWidth = tileWidth;
            Score = 0;
            PelletCounter = 0;
            DotCounter = 0;
            Grid = SetupBoard(reference);
        }

        private Tile[,] SetupBoard(int[][] reference)
        {
            Tile[,] tiles = new Tile[reference.Length, reference[0].Length];

            for (int i = 0; i < tiles.GetLength(0); i++)  // Rows
            {
                for (int j = 0; j < tiles.GetLength(1); j++)  // Columns
                {
                    int value = reference[i][j];
                    TileType type = IntToTile[value];
                    tiles[i, j] = new Tile(TileHeight, TileWidth, type);

                    if (type == TileType.Dot)
                        DotCounter++;
                    else if (type == TileType.PowerPellet)
                        PelletCounter++;
                }
            }
            return tiles;
        }

        public void DisplayBoard()
        {
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    char symbol = Grid[i, j].Type switch
                    {
                        TileType.Empty => ' ',
                        TileType.Wall => '#',
                        TileType.Dot => '.',
                        TileType.PowerPellet => 'O',
                        TileType.Fruit => 'F',
                        TileType.GhostHouse => 'G',
                        TileType.DeadSpace => 'X',
                        _ => '?'
                    };
                    Console.Write(symbol);
                }
                Console.WriteLine();
            }
            Console.WriteLine($"Dots: {DotCounter}, Power Pellets: {PelletCounter}");
        }
    }
}