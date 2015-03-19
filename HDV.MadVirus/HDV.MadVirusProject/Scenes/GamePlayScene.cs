#region Using Statements
using System;
using System.Collections.Generic;

using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Physics2D;

using WaveEngine.Components.Animation;

using WaveEngine.Components.UI;
using WaveEngine.Framework.UI;

using HDV.MadVirus.Models;
using HDV.MadVirus.Constants;
#endregion

namespace HDV.MadVirus.Scenes
{
    struct VirusCoord
    {
        private int row;
        private int column;
        public int R { get { return row; } }
        public int Q { get { return column; } }

        public VirusCoord(int r, int q)
        {
            this.row = r;
            this.column = q;
        }
    }

    class GamePlayScene : Scene
    {
        private int[,] virusIndexArray;
        private Entity[,] virusEntityArray;
        private List<VirusCoord> selectedVirusList;
        private VirusCoord[] startPos;
        private VirusCoord[,] directions = new VirusCoord[,] 
        {
            // q is even
            {new VirusCoord(0,1), new VirusCoord(-1,1), new VirusCoord(-1,0), new VirusCoord(-1,-1), new VirusCoord(0,-1), new VirusCoord(1,0)},
            // q is odd
            {new VirusCoord(1,1), new VirusCoord(0,1), new VirusCoord(-1,0), new VirusCoord(0,-1), new VirusCoord(1,-1), new VirusCoord(1,0)}
        };

        private int seletedColor;
        private int row_height;
        private int column_width;

        private Entity mapBackground;
        private Entity buttonBackground;

        protected override void CreateScene()
        {
            //Insert your scene definition here.
            this.CreateCamera();

            // Create virus map
            this.InitVirusMap();

            // Create the virus button for user to interative
            this.CreateVirusButton();

            // Test
            List<VirusCoord> list = this.FindNeighbor(5, 6);
            this.PrintListCoord(list);
        }
        protected override void Start()
        {
            base.Start();

        }

        private void InitVirusMap()
        {
            row_height = 8;
            column_width = 15;
            virusIndexArray = this.generateRandomMap(row_height, column_width, 1);
            // Add starPos to selectVirus
            selectedVirusList = new List<VirusCoord>();
            selectedVirusList.Add(startPos[0]);

            this.print2DArray(virusIndexArray);

            // Create virus map background
            // Use the mapBackground to AddChild all the virus
            // When scale or move, all the virus scale and move too
            this.mapBackground = new Entity("mapBackground")
            .AddComponent(new Transform2D()
            {
                X = 200,
                Y = 100,
                //Scale = new Vector2(1.5f, 1.5f)
            })
            .AddComponent(new Sprite("Content/mapBackground.wpk"))
            .AddComponent(new SpriteRenderer(DefaultLayers.Opaque))
            ;
            EntityManager.Add(mapBackground);
            // after generate an array of virus index
            // create the sprite of virus and add to entity manager
            this.CreateVirusMap(virusIndexArray);
        }

        private void CreateCamera()
        {
            // Create a 2D camera
            var camera = new FixedCamera2D("Camera2D")
            {
                BackgroundColor = new Color("#e7e7e7")
            };

            var camera2DComponent = camera.Entity.FindComponent<Camera2D>();
            //            camera2DComponent.Zoom = Vector2.One / 2.5f;

            #region Lens Effects
            ////if (WaveServices.Platform.PlatformType == PlatformType.Windows ||
            ////    WaveServices.Platform.PlatformType == PlatformType.Linux ||
            ////    WaveServices.Platform.PlatformType == PlatformType.MacOS)
            ////{
            ////    camera.Entity.AddComponent(ImageEffects.FishEye());
            ////    camera.Entity.AddComponent(new ChromaticAberrationLens() { AberrationStrength = 5.5f });
            ////    camera.Entity.AddComponent(new RadialBlurLens() { Center = new Vector2(0.5f, 0.75f), BlurWidth = 0.02f, Nsamples = 5 });
            ////    camera.Entity.AddComponent(ImageEffects.Vignette());
            ////    camera.Entity.AddComponent(new FilmGrainLens() { GrainIntensityMin = 0.075f, GrainIntensityMax = 0.15f });
            ////}  
            #endregion

            EntityManager.Add(camera);
        }

        private void CreateVirusMap(int[,] arr)
        {

            System.Console.WriteLine("row count: {0}", arr.GetLength(0));
            System.Console.WriteLine("Column count: {0}", arr.GetLength(1));
            Transform2D sampleTrans = new Transform2D();// by default: X = 0, Y = 0, top-left conner
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    //this.CreateVirus(i, j, arr[i, j]);
                    if(arr[i,j] != 0)
                        mapBackground.AddChild(this.CreateVirus(i, j, arr[i, j]));
                    //switch (arr[i,j]) 
                    //{
                    //    case Configuration.BLUE_VIRUS_ID:
                    //        break;
                    //    case Configuration.GREEN_VIRUS_ID:
                    //        break;
                    //    case Configuration.MAGENTA_VIRUS_ID:
                    //        break;
                    //    case Configuration.ORANGE_VIRUS_ID:
                    //        break;
                    //    case Configuration.RED_VIRUS_ID:
                    //        break;
                    //    case Configuration.YELLOW_VIRUS_ID:
                    //        break;
                    //    default:
                    //        break;
                    //}
                }
            }
        }

        private int[,] generateRandomMap(int r, int q, int startPosCount)
        {
            int[,] arrayMap = new int[r, q];
            // Init Array of virus entity
            virusEntityArray = new Entity[r, q];
            System.Random rnd = new System.Random();
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < q; j++)
                {

                    arrayMap[i, j] = rnd.Next(6);
                }
            }

            // Pick a valid random virus as a start position
            startPos = new VirusCoord[startPosCount];
            for (int i = 0; i < startPosCount; i++)
            {
                while (true)
                {
                    System.Random rndR = new System.Random();
                    System.Random rndQ = new System.Random();
                    int a = rndR.Next(r - 1);
                    int b =  rndQ.Next(q - 1);
                    if (arrayMap[a, b] > 0)
                    {
                        VirusCoord virusCoord = new VirusCoord(a, b);
                        startPos[i] = virusCoord;
                        break;
                    }
                }
            }

            // Add startPos to virus map
            for (int i = 0; i < startPosCount; i++)
            {
                System.Random rnd2 = new System.Random();
                int temp = -rnd2.Next(1,6);
                arrayMap[startPos[i].R, startPos[i].Q] = temp;
            }

            return arrayMap;
        }

        // Create new Virus and Draw (add to EntityManager)
        private Entity CreateVirus(int row, int collumn, int color)
        {
            if (color != 0)
            {
                String spriteName = "Content/Virus/virus_" + color.ToString() + ".wpk";
                Entity virus = new Entity("virus" + row.ToString() + collumn.ToString())
                 .AddComponent(new Transform2D()
                 {
                     X = collumn * Configuration.VIRUS_SPRITE_WIDTH * 3 / 4, // Don't ask why, I just found the formula
                     Y = row * Configuration.VIRUS_SPRITE_HEIGHT + (collumn % 2) * 74 / 2 //  Don't ask why, I just found the formula
                 })
                .AddComponent(new Sprite(spriteName))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                ;
                EntityManager.Add(virus);
                virusEntityArray[row, collumn] = virus;

                this.PrintVirusCoordAndId(row, collumn, color);
                return virus;

            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="color"></param>
        private void UpdateSpriteOfVirus(int row, int column, int color)
        {
            String spriteName = "Content/Virus/virus_" + color.ToString() + ".wpk";
            Entity virus = virusEntityArray[row, column];
            virus.RemoveComponent<Sprite>();
            virus.RemoveComponent<SpriteRenderer>();
            virus.AddComponent(new Sprite(spriteName))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));
        }

        // Show Button to choose virus color in mobile
        private void CreateVirusButton()
        {
            buttonBackground = new Entity("buttonBackground")
            .AddComponent(new Transform2D()
            {

            })
            .AddComponent(new Sprite("Content/buttonBackground.wpk"))
            .AddComponent(new SpriteRenderer(DefaultLayers.Opaque))
            ;
            EntityManager.Add(buttonBackground);
            for (int i = 0; i < 6; i++)
            {
                String spriteName = "Content/Virus/virus_" + (i + 1).ToString() + ".wpk";
                Entity virusButton = new Entity("virusButton" + (i + 1).ToString())
                .AddComponent(new Transform2D()
                {
                    X = 0,
                    Y = Configuration.VIRUS_SPRITE_HEIGHT * (i) * 4 / 3,
                    Scale = new Vector2(1.2f, 1.2f)
                })
                .AddComponent(new Sprite(spriteName))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new RectangleCollider())
                .AddComponent(new TouchGestures()
                {
                    EnabledGestures = SupportedGesture.Translation
                })
                .AddComponent(new VirusButtonBehavior(i + 1));
                EntityManager.Add(virusButton);
                buttonBackground.AddChild(virusButton);
                // Add event click
                var virusButtonBehavior = virusButton.FindComponent<VirusButtonBehavior>();
                virusButtonBehavior.click += this.PlayWithColor;
            }
        }

        /// <summary>
        /// Find all the neighbors that valid and unseleted
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        private List<VirusCoord> FindNeighbor(VirusCoord coord)
        {
            int r = coord.R;
            int q = coord.Q;
            List<VirusCoord> neighborList = new List<VirusCoord>();
            var parity = q % 2;
            for (int i = 0; i < directions.GetLength(1); i++)
            {
                int r2 = r + directions[parity, i].R;
                int q2 = q + directions[parity, i].Q;
                if (IsCoordValid(r2, q2))
                {
                    neighborList.Add(new VirusCoord(r2, q2));
                }

            }
            return neighborList;
        }

        private List<VirusCoord> FindNeighbor(int r, int q)
        {
            List<VirusCoord> neighborList = new List<VirusCoord>();
            var parity = q % 2;
            for (int i = 0; i < directions.GetLength(1); i++)
            {
                int r2 = r + directions[parity, i].R;
                int q2 = q + directions[parity, i].Q;
                if (IsCoordValid(r2, q2) && virusIndexArray[r2, q2] > 0)
                {
                    neighborList.Add(new VirusCoord(r2, q2));
                }

            }
            return neighborList;
        }
        private bool IsCoordValid(int r, int q)
        {
            if (r >= 0 && r < row_height && q >= 0 && q < column_width && virusIndexArray[r, q] > 0)
                return true;
            return false;
        }

        private void PlayWithColor(int color)
        {
            System.Console.WriteLine(color.ToString());
            this.seletedColor = color;
            //  iterate through the selectedVirus

            this.IteratingVirusListWithColor(color, selectedVirusList);

            // Update the sprite of virus
            for (int i = 0; i < virusIndexArray.GetLength(0); i++)
            {
                for (int j = 0; j < virusIndexArray.GetLength(1); j++)
                {
                    if (virusIndexArray[i, j] < 0)
                    {
                        UpdateSpriteOfVirus(i, j, -color);
                    }
                }
            }
        }

        private void IteratingVirusListWithColor(int color, List<VirusCoord> listVirus)
        {
            List<VirusCoord> tempSelectedVirusList = new List<VirusCoord>();

            // virus that all it's neighbor is selected will not use to find
            List<VirusCoord> listVirusWillDeleteFromSelected = new List<VirusCoord>();
            foreach (var item in listVirus)
            {
                List<VirusCoord> neighborList = this.FindNeighbor(item);
                if (neighborList.Count > 0)
                {
                    foreach (var neighbor in neighborList)
                    {
                        // if the color is the same with the color we select
                        if (virusIndexArray[neighbor.R, neighbor.Q] == color)
                        {
                            // Choose this virus
                            tempSelectedVirusList.Add(new VirusCoord(neighbor.R, neighbor.Q));
                            // Change the color id
                            virusIndexArray[neighbor.R, neighbor.Q] = -color;
                        }
                    }
                }
                else
                {
                    listVirusWillDeleteFromSelected.Add(item);
                }
            }

            if (listVirusWillDeleteFromSelected.Count > 0)
            {
                foreach (var item in listVirusWillDeleteFromSelected)
                {
                    selectedVirusList.Remove(item);
                }
            }

            // Update the virus map
            if (tempSelectedVirusList.Count > 0)
            {
                foreach (var item in tempSelectedVirusList)
                {
                    selectedVirusList.Add(item);
                }
                this.IteratingVirusListWithColor(color, tempSelectedVirusList);
            }
        }

        #region Debug
        private void print2DArray(int[,] arr)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    System.Console.WriteLine("Element({0},{1})={2}", i, j, arr[i, j]);
                }
            }
        }

        private void PrintListCoord(List<VirusCoord> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine("Coord at row {0}, column {1}", item.R, item.Q);
            }
        }

        private void PrintVirusCoordAndId(int r, int q, int id)
        {
            // Print Text block to debug
            TextBlock title = new TextBlock()
            {
                Text = r.ToString() + q.ToString() + id.ToString(),
                Width = Configuration.VIRUS_SPRITE_WIDTH,
                Foreground = Color.White,
                Margin = new Thickness(q * Configuration.VIRUS_SPRITE_WIDTH * 3 / 4 + 20 + 200,
                    r * Configuration.VIRUS_SPRITE_HEIGHT + (q % 2) * 74 / 2 + 20 + 100,
                    0, 0)
            };
            EntityManager.Add(title);
        }
        #endregion

    }
}
