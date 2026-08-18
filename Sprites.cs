using System.Numerics;
using Raylib_cs;

namespace PacManGame
{
    static class Sprites
    {
        private static readonly Dictionary<Vector2D, int> DirToInt = new()
        {
            {Vector2D.Right, 0},
            {Vector2D.Left,  1},
            {Vector2D.Up ,   2},
            {Vector2D.Down,  3}
        };
        public static List<List<Rectangle>> PacmanDirectionList;
        public static List<List<Rectangle>> BlinkyDirectionList;
        public static List<List<Rectangle>> PinkyDirectionList;
        public static List<List<Rectangle>> InkyDirectionList;
        public static List<List<Rectangle>> ClydeDirectionList;
        public static List<Rectangle> FrightBlue;
        public static List<Rectangle> FrightWhite;
        public static List<Rectangle> DeadEyes;
        public static List<Rectangle> GhostPoints;
        public static Dictionary<FruitType, Rectangle> FruitToRect;
        public static Dictionary<FruitType, Rectangle> FruitToPointsRect;
        public static List<Rectangle> PacManDead;
        public static Rectangle PacManLife;
        static Sprites()
        {
            (int offsetX, int offsetY) = (16, 16);

            // ---- Pac-Man ----
            (int startPacX, int startPacY) = (0, 0);

            List<Rectangle> PacmanRight = new List<Rectangle>();
            List<Rectangle> PacmanLeft = new List<Rectangle>();
            List<Rectangle> PacmanDown = new List<Rectangle>();
            List<Rectangle> PacmanUp = new List<Rectangle>();

            for (int i = 0; i < 3; i++)
            {
                PacmanRight.Add(new Rectangle(startPacX + offsetX * i, startPacY, offsetX, offsetY));
                PacmanLeft.Add(new Rectangle(startPacX + offsetX * i, startPacY + offsetY, offsetX, offsetY));
                PacmanUp.Add(new Rectangle(startPacX + offsetX * i, startPacY + offsetY * 2, offsetX, offsetY));
                PacmanDown.Add(new Rectangle(startPacX + offsetX * i, startPacY + offsetY * 3, offsetX, offsetY));

            }
            PacmanDirectionList = [PacmanRight, PacmanLeft, PacmanUp, PacmanDown];


            // ---- Right-facing ghosts (Blinky/Pinky/Inky/Clyde share one block) ----
            (int startRightX, int startRightY) = (0, 64);

            List<Rectangle> BlinkyRight = new List<Rectangle>();
            List<Rectangle> PinkyRight = new List<Rectangle>();
            List<Rectangle> InkyRight = new List<Rectangle>();
            List<Rectangle> ClydeRight = new List<Rectangle>();

            for (int i = 0; i < 2; i++)
            {
                BlinkyRight.Add(new Rectangle(startRightX + offsetX * i, startRightY, offsetX, offsetY));
                PinkyRight.Add(new Rectangle(startRightX + offsetX * i, startRightY + offsetY, offsetX, offsetY));
                InkyRight.Add(new Rectangle(startRightX + offsetX * i, startRightY + offsetY * 2, offsetX, offsetY));
                ClydeRight.Add(new Rectangle(startRightX + offsetX * i, startRightY + offsetY * 3, offsetX, offsetY));
            }

            // ---- Left-facing ghosts ----
            (int startLeftX, int startLeftY) = (32, 64); // TODO: fill in

            List<Rectangle> BlinkyLeft = new List<Rectangle>();
            List<Rectangle> PinkyLeft = new List<Rectangle>();
            List<Rectangle> InkyLeft = new List<Rectangle>();
            List<Rectangle> ClydeLeft = new List<Rectangle>();

            for (int i = 0; i < 2; i++)
            {
                BlinkyLeft.Add(new Rectangle(startLeftX + offsetX * i, startLeftY, offsetX, offsetY));
                PinkyLeft.Add(new Rectangle(startLeftX + offsetX * i, startLeftY + offsetY, offsetX, offsetY));
                InkyLeft.Add(new Rectangle(startLeftX + offsetX * i, startLeftY + offsetY * 2, offsetX, offsetY));
                ClydeLeft.Add(new Rectangle(startLeftX + offsetX * i, startLeftY + offsetY * 3, offsetX, offsetY));
            }

            // ---- Up-facing ghosts ----
            (int startUpX, int startUpY) = (64, 64); // TODO: fill in

            List<Rectangle> BlinkyUp = new List<Rectangle>();
            List<Rectangle> PinkyUp = new List<Rectangle>();
            List<Rectangle> InkyUp = new List<Rectangle>();
            List<Rectangle> ClydeUp = new List<Rectangle>();

            for (int i = 0; i < 2; i++)
            {
                BlinkyUp.Add(new Rectangle(startUpX + offsetX * i, startUpY, offsetX, offsetY));
                PinkyUp.Add(new Rectangle(startUpX + offsetX * i, startUpY + offsetY, offsetX, offsetY));
                InkyUp.Add(new Rectangle(startUpX + offsetX * i, startUpY + offsetY * 2, offsetX, offsetY));
                ClydeUp.Add(new Rectangle(startUpX + offsetX * i, startUpY + offsetY * 3, offsetX, offsetY));
            }

            // ---- Down-facing ghosts ----
            (int startDownX, int startDownY) = (96, 64); // TODO: fill in

            List<Rectangle> BlinkyDown = new List<Rectangle>();
            List<Rectangle> PinkyDown = new List<Rectangle>();
            List<Rectangle> InkyDown = new List<Rectangle>();
            List<Rectangle> ClydeDown = new List<Rectangle>();

            for (int i = 0; i < 2; i++)
            {
                BlinkyDown.Add(new Rectangle(startDownX + offsetX * i, startDownY, offsetX, offsetY));
                PinkyDown.Add(new Rectangle(startDownX + offsetX * i, startDownY + offsetY, offsetX, offsetY));
                InkyDown.Add(new Rectangle(startDownX + offsetX * i, startDownY + offsetY * 2, offsetX, offsetY));
                ClydeDown.Add(new Rectangle(startDownX + offsetX * i, startDownY + offsetY * 3, offsetX, offsetY));
            }

            BlinkyDirectionList = [BlinkyRight, BlinkyLeft, BlinkyUp, BlinkyDown];

            PinkyDirectionList = [PinkyRight, PinkyLeft, PinkyUp, PinkyDown];

            InkyDirectionList = [InkyRight, InkyLeft, InkyUp, InkyDown];

            ClydeDirectionList = [ClydeRight, ClydeLeft, ClydeUp, ClydeDown];

            FrightBlue = new List<Rectangle>();
            FrightWhite = new List<Rectangle>();

            (int startFrightBlueX, int startFrightBlueY) = (128, 64);
            (int startFrightWhiteX, int startFrightWhiteY) = (160, 64);
            for (int i = 0; i < 2; i++)
            {
                FrightBlue.Add(new Rectangle(startFrightBlueX + offsetX * i, startFrightBlueY, offsetX, offsetY));
                FrightWhite.Add(new Rectangle(startFrightWhiteX + offsetX * i, startFrightWhiteY, offsetX, offsetY));
            }
            DeadEyes = new List<Rectangle>();
            (int startEyesX, int startEyesY) = (128, 80);
            for (int i = 0; i < 4; i++)
            {
                DeadEyes.Add(new Rectangle(startEyesX + offsetX * i, startEyesY, offsetX, offsetY));
            }

            (int startFruitX, int startFruitY) = (48, 48);
            FruitToRect = new Dictionary<FruitType, Rectangle>();
            var values = (FruitType[])Enum.GetValues(typeof(FruitType));
            for (int i = 0; i < values.Length; i++)
            {
                FruitType fruit = values[i];
                FruitToRect[fruit] = new Rectangle(startFruitX + offsetX * i, startFruitY, offsetX, offsetY);
            }
            FruitToPointsRect = new Dictionary<FruitType, Rectangle>();
            (int startFruitPointsX, int startFruitPointsY) = (0, 144);
            for (int i = 0; i < 5; i++)
            {
                FruitType fruit = values[i];
                if (i == 4)
                    FruitToPointsRect[fruit] = new Rectangle(startFruitPointsX, startFruitPointsY, offsetX+4, offsetY);
                else
                    FruitToPointsRect[fruit] = new Rectangle(startFruitPointsX, startFruitPointsY, offsetX, offsetY);
                startFruitPointsX += offsetX;
            }



            startFruitPointsX = 63;
            startFruitPointsY = 160;
            for (int i = 5; i < values.Length; i++)
            {
                FruitType fruit = values[i];
                FruitToPointsRect[fruit] = new Rectangle(startFruitPointsX, startFruitPointsY + (offsetY * (i-5)), offsetX+4, offsetY);
            }

            PacManDead = new List<Rectangle>();
            (int startDeadX, int startDeadY) = (48, 0);
            for (int i = 0; i < 11; i++)
            {
                PacManDead.Add(new Rectangle(startDeadX + offsetX * i, startDeadY, offsetX, offsetY));
            }

            PacManLife = new Rectangle(146, 16, offsetX, offsetY);

            (int startGhostPointsX, int startGhostPointsY) = (0, 128);
            GhostPoints = new List<Rectangle>();
            for (int i = 0; i < 4; i++)
            {
                if (i == 3)
                    GhostPoints.Add(new Rectangle(startGhostPointsX + offsetX * i, startGhostPointsY, offsetX+4, offsetY));
                else
                    GhostPoints.Add(new Rectangle(startGhostPointsX + offsetX * i, startGhostPointsY, offsetX, offsetY));
            }

        }
        public static List<Rectangle> PacManDirectionSelector(Vector2D direction)
        {
            return PacmanDirectionList[DirToInt[direction]];
        }
        public static List<Rectangle> PacManDeathSelector()
        {
            return PacManDead;
        }
        public static Rectangle GhostPointsSelector(int EatenGhostsCounter)
        {
            return GhostPoints[EatenGhostsCounter - 1];
        }

        public static Rectangle GetPacManLife()
        {
            return PacManLife;
        }
        public static List<Rectangle> GhostTypeAndDirectionSelector(GhostType ghostType, Vector2D direction)
        {
            List<List<Rectangle>> DirectionList;
            switch (ghostType)
            {
                case GhostType.Blinky:
                    DirectionList = BlinkyDirectionList;
                    break;
                case GhostType.Pinky:
                    DirectionList = PinkyDirectionList;
                    break;
                case GhostType.Inky:
                    DirectionList = InkyDirectionList;
                    break;
                default:
                    DirectionList = ClydeDirectionList;
                    break;
            }
            return DirectionList[DirToInt[direction]];
        }
        public static List<Rectangle> GhostFrightSelector(bool isBlue)
        {
            return isBlue ? FrightBlue : FrightWhite;
        }
        public static Rectangle GhostDeadSelector(Vector2D direction)
        {
            return DeadEyes[DirToInt[direction]];
        }
        public static Rectangle FruitSelector(FruitType fruit)
        {
            return FruitToRect[fruit];
        }
        public static Rectangle FruitPointsSelector(FruitType fruit)
        {
            return FruitToPointsRect[fruit];
        }
    }
}