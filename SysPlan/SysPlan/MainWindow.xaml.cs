using System;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Ink;
using System.Drawing.Imaging;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

using static SysPlan.PrologComm;

namespace SysPlan
{
    public partial class MainWindow : Window
    {
        private class Step
        {
            public Coords c1;
            public Coords c2;
            public Line line;
            public Step(Coords c1, Coords c2, Line line)
            {
                this.c1 = c1;
                this.c2 = c2;
                this.line = line;
            }
        }

        private TextBox digit1_tb;
        private TextBox digit2_tb;
        private InkCanvas digit1_IC;
        private InkCanvas digit2_IC;
        private string filePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        private string floorImgPath;
        private string wallImgPath;
        private string robotImgPath;
        private string obstacleImgPath;
        private Grid mapGrid;
        private Canvas mapCanvas;
        private int[] chosenDigits = new int[2];
        private int[] xyMax;
        private TextBlock shownChosenDigits;
        private bool isRobotPlaced = false;
        private Coords[] walls;
        private Coords[] path;
        private Coords robotLocation;
        private Coords destinyLocation;
        private List<Coords> obstacles = new List<Coords>();
        private NeuralNetwork neuralNet;
        private double ElemLength = 30;
        private List<Step> steps = new List<Step>();
        public MainWindow()
        {
            floorImgPath = filePath.Replace(@"\SysPlan.exe", @"\GraphicalData\floor.png");
            wallImgPath = filePath.Replace(@"\SysPlan.exe", @"\GraphicalData\wall.png");
            robotImgPath = filePath.Replace(@"\SysPlan.exe", @"\GraphicalData\robot.png");
            obstacleImgPath = filePath.Replace(@"\SysPlan.exe", @"\GraphicalData\obstacle.png");

            InitializeComponent();

            Loaded += OnLoaded;
            Closing += MainWindow_Closing;

        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Begin();
            string savePath = @".\NeuroNetData\neuro net";
            neuralNet = NeuralNetwork.LoadNeuroNet(784, 200, 10, 0.1, savePath);

            BuildDigitReadSection(1, 1, ref digit1_tb, button1_Click, ref digit1_IC, clear1_Click, undo1_Click, 0);
            BuildDigitReadSection(1, 7, ref digit2_tb, button2_Click, ref digit2_IC, clear2_Click, undo2_Click, 1);
            xyMax = GetXYmax();
            BuildMapGrid(xyMax[0] + 1, xyMax[1] + 1);
            ShowChosenDigits(1, 13);
            BuildActionButtons(1, 14);

        }

        private void BuildActionButtons(int col, int row)
        {
            Border b = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Margin = new Thickness(5)
            };
            Grid.SetColumn(b, col);
            Grid.SetRow(b, row);
            Grid.SetColumnSpan(b, 2);

            Grid buttonGrid = new Grid();
            ColumnDefinition c1 = new ColumnDefinition
            {
                Width = GridLength.Auto
            };
            buttonGrid.ColumnDefinitions.Add(c1);
            ColumnDefinition c2 = new ColumnDefinition
            {
                Width = GridLength.Auto
            };
            buttonGrid.ColumnDefinitions.Add(c2);
            RowDefinition r1 = new RowDefinition
            {
                Height = GridLength.Auto
            };
            buttonGrid.RowDefinitions.Add(r1);

            Button FindPath = new Button()
            {
                Content = "Find Path",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 3, 10, 3)
            };
            FindPath.Click += FindPath_Click;
            Grid.SetColumn(FindPath, 0);
            Grid.SetRow(FindPath, 0);
            buttonGrid.Children.Add(FindPath);

            Button PlaceRobot = new Button()
            {
                Content = "Place Robot",
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 3, 10, 3)
            };
            PlaceRobot.Click += PlaceRobot_Click;
            Grid.SetColumn(PlaceRobot, 1);
            Grid.SetRow(PlaceRobot, 0);
            buttonGrid.Children.Add(PlaceRobot);

            b.Child = buttonGrid;
            MainGrid.Children.Add(b);
        }

        private void ShowChosenDigits(int col, int row)
        {
            Border b = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Margin = new Thickness(5)
            };
            Grid.SetColumn(b, col);
            Grid.SetRow(b, row);
            Grid.SetColumnSpan(b, 2);
            TextBlock tb = new TextBlock()
            {
                Text = $"Chosen Digits:  - / -",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 22,
                Margin = new Thickness(5,2,5,2)
            };
            chosenDigits[0] = -1;
            chosenDigits[1] = -1;
            b.Child = tb;
            MainGrid.Children.Add(b);
            shownChosenDigits = tb;
        }
        private void BuildMapGrid(int col, int row)
        {
            mapGrid = new Grid();
            mapCanvas = new Canvas();

            Grid.SetColumn(mapGrid, 3);
            Grid.SetRow(mapGrid, 1);
            Grid.SetColumnSpan(mapGrid, 1);
            Grid.SetRowSpan(mapGrid, 11);
            mapGrid.VerticalAlignment = VerticalAlignment.Top;
            mapGrid.HorizontalAlignment = HorizontalAlignment.Left;

            Grid.SetColumn(mapCanvas, 3);
            Grid.SetRow(mapCanvas, 1);
            Grid.SetColumnSpan(mapCanvas, 1);
            Grid.SetRowSpan(mapCanvas, 11);
            mapCanvas.VerticalAlignment = VerticalAlignment.Top;
            mapCanvas.HorizontalAlignment = HorizontalAlignment.Left;


            for (int i = 0; i <= col; i++)
            {
                ColumnDefinition c = new ColumnDefinition
                {
                    Width = GridLength.Auto
                };
                mapGrid.ColumnDefinitions.Add(c);
            }
            for (int i = 0; i <= row; i++)
            {
                RowDefinition r = new RowDefinition
                {
                    Height = GridLength.Auto
                };
                mapGrid.RowDefinitions.Add(r);
            }

            walls = GetWallInfo();
            for (int i = 0; i <= row; i++)
            {
                for (int j = 0; j <= col; j++)
                {
                    if (i == 0 && j > 0)
                        PlaceNumber(j, i, j - 1);
                    else if (j == 0 && i > 0)
                        PlaceNumber(j, i, i - 1);
                    else if (i != 0 && j != 0)
                        PlaceImg(floorImgPath, j, i);
                }
            }
            foreach (Coords wall in walls)
            {
                PlaceImg(wallImgPath, wall.x + 1, wall.y + 1);
            }

            _ = MainGrid.Children.Add(mapGrid);
            _ = MainGrid.Children.Add(mapCanvas);
        }
        private void PlaceNumber(int x, int y, int number)
        {
            TextBlock tblock = new TextBlock()
            {
                Text = number + "",
                Margin = new Thickness(8, 7, 8, 7),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(tblock, x);
            Grid.SetRow(tblock, y);

            _ = mapGrid.Children.Add(tblock);
        }
        public void PlaceImg(string imgPath, int x, int y)
        {
            Image img = new Image
            {
                Width = ElemLength,
                Height = ElemLength,
            };
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imgPath);
            bitmap.EndInit();

            img.Source = bitmap;
            Grid.SetColumn(img, x);
            Grid.SetRow(img, y);

            _ = mapGrid.Children.Add(img);

            Coords c = new Coords(x - 1, y - 1);
            if (!isWall(c)) { 
                Button placeObstacleBttn = new Button()
                {
                    Background = Brushes.Transparent,
                    Width = ElemLength,
                    Height = ElemLength,
                };
                placeObstacleBttn.Click += PlaceObstacle(c);
                Grid.SetColumn(placeObstacleBttn, x);
                Grid.SetRow(placeObstacleBttn, y);
                _ = mapGrid.Children.Add(placeObstacleBttn);
            }                       
        }

        private bool isWall(Coords c)
        {
            foreach(Coords wall in walls)
            {
                if (wall.Equals(c)) return true;
            }
            return false;
        }

        private void RefreshPath()
        {
            ResetMapCanvas(xyMax[0], xyMax[1]);
            if (destinyLocation != null)
            {
                try
                {
                    path = GetPath(robotLocation);
                }
                catch (SbsSW.SwiPlCs.Exceptions.PlException _)
                {
                    End();
                    Begin();
                    foreach (Coords c1 in obstacles)
                    {
                        AddObstacle(c1);
                    }
                    SetGoal(destinyLocation);
                    path = null;
                }
                ShowPath(path);
            }
        }

        private RoutedEventHandler PlaceObstacle(Coords c)
        {
            return (object sender, RoutedEventArgs e) => {                
                Step partOfPath = steps.Find((st) => {
                    return st.c2.x == c.x && st.c2.y == c.y;
                });                                           
                if (isObstacle(c.x, c.y))
                {                    
                    if (!obstacles.Remove(obstacles.Find((o) => { return o.Equals(c); })))
                    {
                        MessageBox.Show("Failed to Remove Obstacle");
                        return;
                    }
                    RemoveObstacle(c);
                    PlaceImg(floorImgPath, c.x+1, c.y+1);                                        
                    RefreshPath();
                }
                else if(!isWall(c.x, c.y))
                {
                    if (robotLocation != null && robotLocation.Equals(c))
                    {
                        MessageBox.Show("Cannot place obstacle on top of Robot!");
                        return;
                    }
                    if (destinyLocation != null && destinyLocation.Equals(c))
                    {
                        MessageBox.Show("Cannot place obstacle on top of destiny!");
                        return;
                    }
                    AddObstacle(c);
                    obstacles.Add(c);
                    PlaceImg(obstacleImgPath, c.x + 1, c.y + 1);
                    if (partOfPath != null)
                    {
                        mapCanvas.Children.Clear();
                        RefreshPath();
                    }
                }
                
            };
        }

        private void BuildDigitReadSection(int col, int row, ref TextBox tbx, RoutedEventHandler clickMethod, ref InkCanvas inkC, RoutedEventHandler clearMethod, RoutedEventHandler undoMethod, int digit)
        {
            Border b = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Margin = new Thickness(5),
                Height = 105,
                Width = 105
            };
            Grid.SetColumn(b, col);
            Grid.SetRow(b, row);
            Grid.SetRowSpan(b, 3);
            Canvas cnv = new Canvas()
            {
                Background = Brushes.Transparent,
                Margin = new Thickness(5),
                Height = 100,
                Width = 100
            };
            Grid.SetColumn(cnv, col);
            Grid.SetRow(cnv, row);
            Grid.SetRowSpan(cnv, 3);
            DrawingAttributes inkDA = new DrawingAttributes();
            inkDA.Color = Colors.Black;
            inkDA.Height = 6;
            inkDA.Width = 6;
            inkDA.FitToCurve = false;
            inkDA.StylusTip = StylusTip.Ellipse;
            InkCanvas ic = new InkCanvas()
            {
                Height = 100,
                Width = 100,
                DefaultDrawingAttributes = inkDA
            };
            inkC = ic;
            cnv.Children.Add(ic);

            Button clear = new Button()
            {
                Content = "Clear",
                Width = 35,
                Height = 20
            };
            clear.Click += clearMethod;
            Grid.SetColumn(clear, col + 1);
            Grid.SetRow(clear, row + 1);

            Button undo = new Button()
            {
                Content = "Undo",
                Width = 35,
                Height = 20
            };
            undo.Click += undoMethod;
            Grid.SetColumn(undo, col + 1);
            Grid.SetRow(undo, row);

            string s = digit == 0 ? "Horizontal Coordinate: " : "Vertical Coordinate: ";
            TextBlock tblock = new TextBlock()
            {
                Text = s
            };
            Grid.SetColumn(tblock, col);
            Grid.SetRow(tblock, row + 4);
            TextBox tbox = new TextBox()
            {
                Width = 30,
                Height = 20,
                FontSize = 16,
                Margin = new Thickness(24, 0, 19, 0)
            };
            tbx = tbox;
            Grid.SetColumn(tbox, col + 1);
            Grid.SetRow(tbox, row + 4);
            Button bttn = new Button()
            {
                Content = "Confirm",
                Margin = new Thickness(5),
                Height = 30
            };
            bttn.Click += clickMethod;
            Grid.SetColumn(bttn, col);
            Grid.SetRow(bttn, row + 5);
            Grid.SetColumnSpan(bttn, 2);

            _ = MainGrid.Children.Add(b);
            _ = MainGrid.Children.Add(cnv);
            _ = MainGrid.Children.Add(undo);
            _ = MainGrid.Children.Add(clear);
            _ = MainGrid.Children.Add(tblock);
            _ = MainGrid.Children.Add(tbox);
            _ = MainGrid.Children.Add(bttn);
        }
        private void ConfirmWritten(string s, int Digit)
        {
            try
            {
                int number = int.Parse(s);
                if (number > xyMax[Digit] || number < 0)
                    throw new Exception("Invalid Digit Provided");
                chosenDigits[Digit] = number;
                UpdateDigitShown(Digit);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ConfirmDrawn(string imglocation, int Digit, ref InkCanvas digit)
        {
            try
            {
                FileStream imgStream = new FileStream(imglocation, FileMode.OpenOrCreate);
                ConvertInkCanvasToImage(ref digit, imgStream);
                imgStream.Close();
                System.Drawing.Bitmap img = FormatDigitImage(imglocation);
                int number = ReadNumber(img);
                if (number > xyMax[Digit] || number < 0)
                    throw new Exception("Invalid Digit Provided");
                chosenDigits[Digit] = number;
                UpdateDigitShown(Digit);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void UpdateDigitShown(int Digit)
        {
            if (Digit == 0)
            {
                string s1 = chosenDigits[1] == -1 ? "-" : chosenDigits[1] + "";
                string s0 = chosenDigits[0] == -1 ? "-" : chosenDigits[0] + "";
                shownChosenDigits.Text = $"Chosen Digits:  {s0} / {s1}";
            }
            else
            {
                string s0 = chosenDigits[0] == -1 ? "-" : chosenDigits[0] + "";
                string s1 = chosenDigits[1] == -1 ? "-" : chosenDigits[1] + "";
                shownChosenDigits.Text = $"Chosen Digits:  {s0} / {s1}";
            }
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (digit1_tb.Text != "")
            {
                ConfirmWritten(digit1_tb.Text, 0);
            }
            else
            {
                ConfirmDrawn("./DrawnDigits/digit1.png", 0, ref digit1_IC);
            }
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (digit2_tb.Text != "")
            {
                ConfirmWritten(digit2_tb.Text, 1);
            }
            else
            {
                ConfirmDrawn("./DrawnDigits/digit2.png", 1, ref digit2_IC);
            }
        }

        private int ReadNumber(System.Drawing.Bitmap img)
        {
            return MNISTHandler.IndexOfMaxValue(neuralNet.Query(ConvertImgToArray(img)));
        }

        private double[] ConvertImgToArray(System.Drawing.Bitmap img)
        {
            double[] ret = new double[img.Height * img.Width];
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    ret[j + (i * 28)] = MadeDigitHandler.ConvertBrightnessToIdxVal(img, j, i);
                }
            }
            return ret;
        }

        private System.Drawing.Bitmap FormatDigitImage(string imgLocation)
        {
            System.Drawing.Bitmap img = new System.Drawing.Bitmap(imgLocation);
            img = Utils.ResizeImage(img, 20, 20);
            FileStream imgStream = new FileStream(imgLocation.Replace(".png", "R.png"), FileMode.OpenOrCreate);
            img.Save(imgStream, ImageFormat.Bmp);
            imgStream.Close();
            Coords center = FindImageCenterofMass(img);
            img = ResizeAndCenterImage(img, center);
            imgStream = new FileStream(imgLocation.Replace(".png", "RC.png"), FileMode.OpenOrCreate);
            img.Save(imgStream, ImageFormat.Bmp);
            imgStream.Close();
            return img;
        }

        private System.Drawing.Bitmap ResizeAndCenterImage(System.Drawing.Bitmap img, Coords center)
        {
            //Console.WriteLine($"20x20 center: {center}");
            System.Drawing.Bitmap newImg = new System.Drawing.Bitmap(28, 28);
            Coords newCenter = new Coords((center.x * 13) / 9, (center.y * 13) / 9);
            //Console.WriteLine($"28x28 center: {newCenter}");
            int xAdjust = 9 - center.x;
            int yAdjust = 9 - center.y;
            //Console.WriteLine($"Adjust Before Limit: x: {xAdjust}, y:{yAdjust}");
            xAdjust = LimitAbsVal(xAdjust, 4);
            yAdjust = LimitAbsVal(yAdjust, 4);
            //Console.WriteLine($"Adjust After Limit: x: {xAdjust}, y:{yAdjust}");
            for (int i = 0; i < newImg.Height; i++)
            {
                for (int j = 0; j < newImg.Width; j++)
                {
                    if (j >= 4 + xAdjust && j < 4 + xAdjust + 20 && i >= 4 + yAdjust && i < 4 + yAdjust + 20)
                        newImg.SetPixel(j, i, img.GetPixel(j - xAdjust - 4, i - yAdjust - 4));
                    else
                        newImg.SetPixel(j, i, System.Drawing.Color.White);
                }
            }
            return newImg;
        }

        private int LimitAbsVal(int value, int limit)
        {
            if (Math.Abs(value) > limit)
            {
                if (value >= 0)
                {
                    return limit;
                } else
                {
                    return -limit;
                }
            } else
            {
                return value;
            }
        }

        private Coords FindImageCenterofMass(System.Drawing.Bitmap img)
        {
            float xSum = 0;
            float ySum = 0;
            float points = 0;
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    float pixelVal = img.GetPixel(j, i).GetBrightness(); //0-Black 1-White
                    pixelVal -= 1;
                    if (pixelVal != 0.0) pixelVal *= -1;//1-black 0-white                    
                    if (pixelVal != 0)
                    {
                        xSum += j * pixelVal;
                        ySum += i * pixelVal;
                        points += pixelVal;
                    }
                }
            }
            return new Coords((int)Math.Round(xSum / points), (int)Math.Round(ySum / points));
        }

        private void clear1_Click(object sender, RoutedEventArgs e)
        {
            digit1_IC.Strokes.Clear();
            chosenDigits[0] = -1;
            UpdateDigitShown(0);
        }
        private void undo1_Click(object sender, RoutedEventArgs e)
        {
            if (digit1_IC.Strokes.Count > 0) {
                digit1_IC.Strokes.RemoveAt(digit1_IC.Strokes.Count - 1);
                if (digit1_IC.Strokes.Count == 0)
                {
                    chosenDigits[0] = -1;
                    UpdateDigitShown(0);
                }
            }            

        }
        private void clear2_Click(object sender, RoutedEventArgs e)
        {
            digit2_IC.Strokes.Clear();
            chosenDigits[1] = -1;
            UpdateDigitShown(1);
        }
        private void undo2_Click(object sender, RoutedEventArgs e)
        {            
            if (digit2_IC.Strokes.Count > 0)
            {
                digit2_IC.Strokes.RemoveAt(digit2_IC.Strokes.Count - 1);
                if (digit2_IC.Strokes.Count == 0)
                {
                    chosenDigits[1] = -1;
                    UpdateDigitShown(1);
                }
            }
        }
        private void FindPath_Click(object sender, RoutedEventArgs e)
        {
            if (isRobotPlaced)
            {
                if (isValid(chosenDigits[0], chosenDigits[1]))
                {
                    destinyLocation = new Coords(chosenDigits[0], chosenDigits[1]);
                    SetGoal(destinyLocation);
                    RefreshPath();
                }
                else
                {
                    MessageBox.Show("Destiny location is invalid!");
                }
            } else
            {
                MessageBox.Show("Robot must be placed in valid coordinates to find Path!");
            }
        }

             
        private void ShowPath(Coords[] path)
        {
            steps.Clear();
            if (path == null)
                MessageBox.Show("There is no Possible Path!");
            else 
                for(int i = path.Length - 1; i > 0; i--)
                {                
                    steps.Add(CreatelineBetween(path[i - 1], path[i]));                
                }            
        }

        private Step CreatelineBetween(Coords cord1, Coords cord2)
        {

            // Center the line horizontally and vertically.
            // Get the positions of the controls that should be connected by a line.           
            double length = ElemLength / 2;
            double xCorrection = length * 1.5;
            double yCorrection = length * 2;
            Point centeredArrowStartPosition = new Point(length + (cord1.x * ElemLength) + xCorrection, length + (cord1.y * ElemLength) + yCorrection);
            Point centeredArrowEndPosition = new Point(length + (cord2.x * ElemLength) + xCorrection, length + (cord2.y * ElemLength) + yCorrection);

            // Draw the line between two controls
            var line = new Line()
            {                
                Stroke = Brushes.LightYellow,
                StrokeThickness = 4,
                X1 = centeredArrowStartPosition.X,
                Y1 = centeredArrowStartPosition.Y,
                X2 = centeredArrowEndPosition.X,
                Y2 = centeredArrowEndPosition.Y

            };            
            mapCanvas.Children.Add(line);
            return new Step(cord1, cord2, line);
        }        

        private bool isObstacle(int x, int y)
        {
            return obstacles.Find((c) => { return c.Equals(new Coords(x, y)); }) != null;
        }

        private void PlaceRobot_Click(object sender, RoutedEventArgs e)
        {
            if (isValid(chosenDigits[0], chosenDigits[1]))                    
            {
                if (isRobotPlaced)
                {
                    ResetMapCanvas(xyMax[0] + 1, xyMax[1] + 1);
                    PlaceImg(floorImgPath, robotLocation.x + 1, robotLocation.y + 1);
                }
                PlaceImg(robotImgPath, chosenDigits[0] + 1, chosenDigits[1] + 1);
                if(!isRobotPlaced) isRobotPlaced = true;
                robotLocation = new Coords(chosenDigits[0], chosenDigits[1]);
            }                
            else
                MessageBox.Show("Coordenates chosen are not Valid!");
        }

        private bool isValid(int v1, int v2)
        {
            return v1 != -1 && v2 != -1 && !isWall(v1, v2) && !isObstacle(v1, v2);
        }

        private void ResetMapCanvas(int col, int row)
        {
            if (steps != null) 
                mapCanvas.Children.Clear();
            
        }

        private bool isWall(int x, int y)
        {                            
            foreach (Coords wall in walls)
            {
                if (wall.x == x && wall.y == y)
                    return true;
            }
            return false;            
        }
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            End();
            MessageBox.Show("Exiting!");
        }
        public static void ConvertInkCanvasToImage(ref InkCanvas digit_IC, Stream stream)
        {

            int margin = (int)digit_IC.Margin.Left;
            int width = (int)digit_IC.ActualWidth - margin;
            int height = (int)digit_IC.ActualHeight - margin;
            //render ink to bitmap
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Default);
            renderBitmap.Render(digit_IC);
            //save the ink to a memory stream
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            encoder.Save(stream);
        }
    }
}
