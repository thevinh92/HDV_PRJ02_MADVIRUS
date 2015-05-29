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

using Newtonsoft.Json;
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
        private int moves;
        private float mapScale;
        //private float mapScaleV;
        private int level; // tương đương với level là số lượng màu sắc của virus
        private int colorTypeCount;

        private Entity virusMapEntity;
        private Entity gamePlayBackground;
        private Entity sideBarBackground;
        private Entity helpButton;
        private Entity newGameButton;

        private Entity panelBar;
        private Entity turnPanel;
        private Entity scorePanel;
        private Entity levelPanel;

        public GamePlayScene()
        {

        }

        public GamePlayScene(int level)
        {

        }

        public GamePlayScene(bool isLoad)
        {
            if (isLoad)
            {
                this.LoadPlayerPref();
            }
        }
        protected override void CreateScene()
        {
            // Create Background
            Entity backgound= new Entity("background")
            .AddComponent(new Transform2D()
            {
                X = 0,
                Y = 0,
                DrawOrder = Configuration.BACKGROUND_DRAW_ORDER
            })
            .AddComponent(new Sprite("Content/background.wpk"))
            .AddComponent(new SpriteRenderer(DefaultLayers.Opaque))
            ;
            EntityManager.Add(backgound);
            //Insert your scene definition here.
            this.CreateCamera();

            // Create Layout panel
            this.CreateLayoutPanel();
            // Create virus map
            this.InitVirusMap(16, 29);

            // Create the virus button for user to interative
            this.CreateVirusButton();

            // 
        }
        protected override void Start()
        {
            base.Start();

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

        #region Layout
        private void CreateLayoutPanel()
        {
            this.panelBar = new Entity("panelBar")
                .AddComponent(new Transform2D()
                {
                    X = 155,
                    Y = 0,
                })
                ;
            EntityManager.Add(panelBar);
            this.CreatePanel(turnPanel, "turnPanel", "Luot choi", "0", new Vector2(0,0));
            this.CreatePanel(scorePanel, "scorePanel", "Diem so", "0", new Vector2(345,0));
            this.CreatePanel(levelPanel, "levelPanel", "Level:", "1", new Vector2(690,0));
            
        }

        private void CreatePanel(Entity entity, string name, string title, string value, Vector2 vector2)
        {
            entity = new Entity(name)
            .AddComponent(new Transform2D()
            {
                X = vector2.X,
                Y = vector2.Y,
                DrawOrder = Configuration.VIRUS_DRAW_ORDER
            })
            .AddComponent(new Sprite("Content/panel/background-gameplay-3.wpk"))
            .AddComponent(new TextControl("Content/fonts/tahoma.spr")
            {
                Margin = Thickness.Zero,
                Text = title,
                // HorizontalAlignment = HorizontalAlignment.Center,
                // VerticalAlignment = VerticalAlignment.Center,
                Foreground = new WaveEngine.Common.Graphics.Color("#00fce1")
            })
            .AddComponent(new TextControlRenderer())
            .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            ;
            entity.FindComponent<TextControl>().Transform2D = new Transform2D();
            entity.FindComponent<TextControl>().Transform2D.X = 100;
            entity.FindComponent<TextControl>().Transform2D.Y = 100;
            panelBar.AddChild(entity);
        }
        #endregion

        #region Init Virus Map
        private void InitVirusMap(int row, int column)
        {
            row_height = row;
            column_width = column;
            moves = 0;
            virusIndexArray = this.generateRandomMap(row_height, column_width, 1);
            // Add starPos to selectVirus
            selectedVirusList = new List<VirusCoord>();
            selectedVirusList.Add(startPos[0]);

            this.print2DArray(virusIndexArray);

            // Create back ground gameplay
            gamePlayBackground = new Entity("backgroundGamePlay")
            .AddComponent(new Transform2D()
            {
                X = 0,
                Y = 0,
                Scale = new Vector2(1.0f, 1.0f),
                DrawOrder = Configuration.FOREGROUND_DRAW_ORDER
            })
            .AddComponent(new Sprite("Content/background-gameplay.wpk"))
            .AddComponent(new SpriteRenderer(DefaultLayers.Additive))
            ;
            EntityManager.Add(gamePlayBackground);

            // Create virus map Entity
            // Use The invisible Entity to AddChild all the virus
            // When scale or move, all the virus scale and move too
            float mapScaleH = Configuration.BACKGROUND_GAMEPLAY_WIDTH / ((column * 5 / 6) * Configuration.VIRUS_SPRITE_WIDTH);
            float mapScaleV = Configuration.BACKGROUND_GAMEPLAY_HEIGHT / (row * Configuration.VIRUS_SPRITE_HEIGHT);
            this.virusMapEntity = new Entity("mapBackgroundEntity")
            .AddComponent(new Transform2D()
            {
                X = Configuration.VIRUS_MAP_ENTITY_X,
                Y = Configuration.VIRUS_MAP_ENTITY_Y,
                Scale = new Vector2(mapScaleH, mapScaleV)
            })
            ;
            // add virus map entity to background gameplay
            // When move the background gameplay, we can move all the virus
            gamePlayBackground.AddChild(virusMapEntity);

            //Start draw virus from this coordinate
            Transform2D startPoint = new Transform2D();
            startPoint.X = Configuration.MARGIN_X;
            startPoint.Y = Configuration.MARGIN_Y;
            // after generate an array of virus index
            // create the sprite of virus and add to entity manager
            this.CreateVirusMap(virusIndexArray, startPoint);

        }
        private void CreateVirusMap(int[,] arr, Transform2D startPoint)
        {

            System.Console.WriteLine("row count: {0}", arr.GetLength(0));
            System.Console.WriteLine("Column count: {0}", arr.GetLength(1));
            Transform2D sampleTrans = new Transform2D();
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if(arr[i,j] != 0)
                        virusMapEntity.AddChild(this.CreateVirus(i, j, arr[i, j],startPoint));
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

                    arrayMap[i, j] = rnd.Next(Configuration.MAX_VIRUS_TYPES);
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
                int temp = -rnd2.Next(1, Configuration.MAX_VIRUS_TYPES);
                arrayMap[startPos[i].R, startPos[i].Q] = temp;
            }

            return arrayMap;
        }

        // Create new Virus and Draw (add to EntityManager)
        private Entity CreateVirus(int row, int collumn, int color, Transform2D startPoint)
        {
            var startX = startPoint.X;
            var startY = startPoint.Y;
            if (color != 0)
            {
                String spriteName = "Content/VirusSprite/virus_" + color.ToString() + ".wpk";
                Entity virus = new Entity("virus" + (row*10).ToString() + collumn.ToString())
                 .AddComponent(new Transform2D()
                 {
                     X = startX + collumn * Configuration.VIRUS_SPRITE_WIDTH * 3 / 4, // Don't ask why, I just found the formula
                     Y = startY + row * Configuration.VIRUS_SPRITE_HEIGHT + (collumn % 2) * Configuration.VIRUS_SPRITE_HEIGHT / 2, //  Don't ask why, I just found the formula
                     DrawOrder = Configuration.VIRUS_DRAW_ORDER
                 })
                .AddComponent(new Sprite(spriteName))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                ;
                virusEntityArray[row, collumn] = virus;

                //this.PrintVirusCoordAndId(row, collumn, color);
                return virus;

            }
            return null;
        }

        // Show Button to choose virus color in mobile
        private void CreateVirusButton()
        {
            sideBarBackground = new Entity("buttonBackground")
            .AddComponent(new Transform2D()
            {

            })
            .AddComponent(new Sprite("Content/sidebar-background.wpk"))
            .AddComponent(new SpriteRenderer(DefaultLayers.Additive))
            ;
            EntityManager.Add(sideBarBackground);
            for (int i = 0; i < Configuration.MAX_VIRUS_TYPES; i++)
            {
                String spriteName = "Content/VirusSprite/virus_" + (i + 1).ToString() + ".wpk";
                Entity virusButton = new Entity("virusButton" + (i + 1).ToString())
                .AddComponent(new Transform2D()
                {
                    X = 35,
                    Y = 42 + Configuration.VIRUS_SPRITE_HEIGHT * (i),
                    Scale = new Vector2(1.0f, 1.0f)
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
                sideBarBackground.AddChild(virusButton);
                // Add event click
                var virusButtonBehavior = virusButton.FindComponent<VirusButtonBehavior>();
                virusButtonBehavior.click += this.PlayWithColor;
            }
            //this.TestSaveLoadButton();
            helpButton = new Entity("helpButton")
            .AddComponent(new Transform2D()
            {

            })
            .AddComponent(new Sprite("Content/menus/icon-giup-do.wpk"))
        }
        #endregion

        #region Main Processor
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
            //System.Console.WriteLine(color.ToString());
            moves++;
            this.seletedColor = color;
            System.Console.WriteLine("{0}, {1}", color, moves);

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
            // List of Virus will be check in next loop
            List<VirusCoord> tempSelectedVirusList = new List<VirusCoord>();

            // List of Virus (that all it's neighbor is selected) will not use to find
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="color"></param>
        private void UpdateSpriteOfVirus(int row, int column, int color)
        {
            String spriteName = "Content/VirusSprite/virus_" + color.ToString() + ".wpk";
            Entity virus = virusEntityArray[row, column];
            virus.RemoveComponent<Sprite>();
            virus.RemoveComponent<SpriteRenderer>();
            virus.AddComponent(new Sprite(spriteName))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));
        }
        #endregion

        #region Load&Save Map
        private void SavePlayerPref()
        {
            MadVirusStorageClass storage = new MadVirusStorageClass();
            storage.level = 10;
            storage.moves = this.moves;
            storage.score = 4000;
            storage.curentColor = this.seletedColor;
            storage.virusIndexArray = this.virusIndexArray;

            
            string json = JsonConvert.SerializeObject(storage);
            Console.WriteLine(json);
            //WaveServices.Storage.Write<MadVirusStorageClass>(storage);
            using(var stream = WaveServices.Storage.OpenStorageFile("myFile", WaveEngine.Common.IO.FileMode.OpenOrCreate))
            {
                var sw = new System.IO.StreamWriter(stream);
                sw.WriteLine(json);
                sw.Close();
            }
        }

        private void LoadPlayerPref()
        {
            MadVirusStorageClass storage = new MadVirusStorageClass();
            string json = "";
            using(var stream = WaveServices.Storage.OpenStorageFile("myFile", WaveEngine.Common.IO.FileMode.Open))
            {
                var sr = new System.IO.StreamReader(stream);
                json = sr.ReadLine();
                Console.WriteLine(json);
            }
            storage = JsonConvert.DeserializeObject<MadVirusStorageClass>(json);
            this.level = storage.level;
            this.moves = storage.moves;
            this.seletedColor = storage.curentColor;
            this.virusIndexArray = storage.virusIndexArray;
            // Reset Virus map entity
            int row = virusIndexArray.GetLength(0);
            int column = virusIndexArray.GetLength(1);
            virusEntityArray = new Entity[virusIndexArray.GetLength(0), virusIndexArray.GetLength(1)];
            float mapScaleH = Configuration.BACKGROUND_GAMEPLAY_WIDTH / ((column * 5 / 6) * Configuration.VIRUS_SPRITE_WIDTH);
            float mapScaleV = Configuration.BACKGROUND_GAMEPLAY_HEIGHT / (row * Configuration.VIRUS_SPRITE_HEIGHT);
            gamePlayBackground.RemoveChild("mapBackgroundEntity");
            this.virusMapEntity = new Entity("mapBackgroundEntity")
            .AddComponent(new Transform2D()
            {
                X = Configuration.VIRUS_MAP_ENTITY_X,
                Y = Configuration.VIRUS_MAP_ENTITY_Y,
                Scale = new Vector2(mapScaleH, mapScaleV)
            })
            ;
            // add virus map entity to background gameplay
            // When move the background gameplay, we can move all the virus
            gamePlayBackground.AddChild(virusMapEntity);
            // Start draw virus from this coordinate
            Transform2D startPoint = new Transform2D();
            startPoint.X = Configuration.MARGIN_X;
            startPoint.Y = Configuration.MARGIN_Y;
            // after generate an array of virus index
            // create the sprite of virus and add to entity manager
            this.CreateVirusMapFromOldArray(virusIndexArray, startPoint, storage.curentColor);

        }

        private void LoadOldGamePlay()
        {
            MadVirusStorageClass storage = new MadVirusStorageClass();
            string json = "";
            using (var stream = WaveServices.Storage.OpenStorageFile("myFile", WaveEngine.Common.IO.FileMode.Open))
            {
                var sr = new System.IO.StreamReader(stream);
                json = sr.ReadLine();
                Console.WriteLine(json);
            }
            storage = JsonConvert.DeserializeObject<MadVirusStorageClass>(json);
            this.level = storage.level;
            this.moves = storage.moves;
            this.seletedColor = storage.curentColor;
            this.virusIndexArray = storage.virusIndexArray;
            // Reset Virus map entity
            int row = virusIndexArray.GetLength(0);
            int column = virusIndexArray.GetLength(1);
            virusEntityArray = new Entity[virusIndexArray.GetLength(0), virusIndexArray.GetLength(1)];
            float mapScaleH = Configuration.BACKGROUND_GAMEPLAY_WIDTH / ((column * 5 / 6) * Configuration.VIRUS_SPRITE_WIDTH);
            float mapScaleV = Configuration.BACKGROUND_GAMEPLAY_HEIGHT / (row * Configuration.VIRUS_SPRITE_HEIGHT);
            this.virusMapEntity = new Entity("mapBackgroundEntity")
            .AddComponent(new Transform2D()
            {
                X = Configuration.VIRUS_MAP_ENTITY_X,
                Y = Configuration.VIRUS_MAP_ENTITY_Y,
                Scale = new Vector2(mapScaleH, mapScaleV)
            })
            ;
            // add virus map entity to background gameplay
            // When move the background gameplay, we can move all the virus
            gamePlayBackground.AddChild(virusMapEntity);
            // Start draw virus from this coordinate
            Transform2D startPoint = new Transform2D();
            startPoint.X = Configuration.MARGIN_X;
            startPoint.Y = Configuration.MARGIN_Y;
            // after generate an array of virus index
            // create the sprite of virus and add to entity manager
            this.CreateVirusMapFromOldArray(virusIndexArray, startPoint, storage.curentColor);
        }

        private void CreateVirusMapFromOldArray(int[,] arr, Transform2D startPoint, int selectedColor)
        {
            Transform2D sampleTrans = new Transform2D();
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if(arr[i,j] > 0)
                        virusMapEntity.AddChild(this.CreateVirus(i, j, arr[i, j],startPoint));
                    if(arr[i,j] < 0)
                        virusMapEntity.AddChild(this.CreateVirus(i, j, -selectedColor, startPoint));
                }
            }
        }

        private void TestSaveLoadButton()
        {
            // Test Save map
            Entity virusButtonSave = new Entity("virusButtonSave")
            .AddComponent(new Transform2D()
            {
                X = 35,
                Y = 42 + Configuration.VIRUS_SPRITE_HEIGHT * 6,
                Scale = new Vector2(1.0f, 1.0f)
            })
            .AddComponent(new Sprite("Content/VirusSprite/virus_-1.wpk"))
            .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            .AddComponent(new RectangleCollider())
            .AddComponent(new TouchGestures()
            {
                EnabledGestures = SupportedGesture.Translation
            })
            .AddComponent(new StorageButtonBehavior())
            ;
            var storageBtnBehav = virusButtonSave.FindComponent<StorageButtonBehavior>();
            storageBtnBehav.clickSave += SavePlayerPref;
            sideBarBackground.AddChild(virusButtonSave);

            // Test Load map
            Entity virusButtonLoad = new Entity("virusButtonLoad")
            .AddComponent(new Transform2D()
            {
                X = 35,
                Y = 42 + Configuration.VIRUS_SPRITE_HEIGHT * 7,
                Scale = new Vector2(1.0f, 1.0f)
            })
            .AddComponent(new Sprite("Content/VirusSprite/virus_-2.wpk"))
            .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
            .AddComponent(new RectangleCollider())
            .AddComponent(new TouchGestures()
            {
                EnabledGestures = SupportedGesture.Translation
            })
            .AddComponent(new StorageBtnBehavior())
            ;
            var storage = virusButtonLoad.FindComponent<StorageBtnBehavior>();
            storage.clickLoad += LoadPlayerPref;
            sideBarBackground.AddChild(virusButtonLoad);
        }
        #endregion

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
                Margin = new Thickness(q * Configuration.VIRUS_SPRITE_WIDTH * 3 / 4 + 20 + 160 + 10,
                    r * Configuration.VIRUS_SPRITE_HEIGHT + (q % 2) * 74 / 2 + 20 + 50 + 10,
                    0, 0)
            };
            EntityManager.Add(title);
        }
        #endregion

    }
}
