using Contract;
using Fluent;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimplePaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            KeyDown += new KeyEventHandler(OnButtonKeyDown);
            KeyUp += new KeyEventHandler(OnButtonKeyUp);
        }
        bool shiftMode;
        bool ctrlMode;

        private void OnButtonKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                shiftMode = false;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                ctrlMode = false;
            }
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                shiftMode = true;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                ctrlMode = true;
            }
            else
          if (e.Key == Key.Z && ctrlMode)
            {
                if (_shapes.Count > 0)
                {
                    _redos.Add(_shapes[_shapes.Count - 1]);
                    _shapes.RemoveAt(_shapes.Count - 1);
                }
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
            }
            else if (e.Key == Key.Y && ctrlMode)
            {
                if (_redos.Count > 0)
                {
                    _shapes.Add(_redos[_redos.Count - 1]);
                    canvas.Children.Add(_shapes[_shapes.Count - 1].Draw());
                    _redos.RemoveAt(_redos.Count - 1);
                }
            }
        }

        List<IShape> _shapes = new List<IShape>();
        List<IShape> _redos = new List<IShape>();
        Dictionary<string, IShape> _prototypes = new Dictionary<string, IShape>();
        Dictionary<int, List<Image>> images = new Dictionary<int, List<Image>>();
        string _selectedShapeName = "";
        bool _isDrawing = false;
        IShape _preview;
        System.Drawing.Color colorPen = System.Drawing.Color.FromName("Black"); 
        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            { 

                string exePath = Assembly.GetExecutingAssembly().Location;
                string folder = System.IO.Path.GetDirectoryName(exePath);
                var dlls = new DirectoryInfo(folder).GetFiles("*.dll");

                foreach (var dll in dlls)
                {
                    //if(dll.FullName== @$"{folder}\ControlzEx.dll")
                    //{
                    //    //var domain = AppDomain.CurrentDomain;
                    //    //Assembly assembly1 = domain.Load(AssemblyName.GetAssemblyName(dll.FullName));
                    //    continue;
                    //}

                    //var domain = AppDomain.CurrentDomain;
                    //Assembly assembly = domain.Load(AssemblyName.GetAssemblyName(dll.FullName));
                    //var types = assembly.GetTypes();

                    var assembly = Assembly.LoadFrom(dll.FullName);
                    var types = assembly.GetTypes();

                    foreach (var type in types)
                    {
                        if (type.IsClass)
                        {
                            if (typeof(IShape).IsAssignableFrom(type))
                            {
                                var shape = Activator.CreateInstance(type) as IShape;
                                _prototypes.Add(shape.Name, shape);
                            }
                        }
                    }
                    _prototypes.Remove("Point");
                    _preview = _prototypes.First().Value.Clone();
                    methodComboBox.ItemsSource = _prototypes;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
       
        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            _isDrawing = true;

            Point pos = e.GetPosition(canvas);
            if (_preview != null)
            {
                //System.Drawing.Color getColor = System.Drawing.Color.FromName(colorComboBox.Text);
                //_preview.Color = Color.FromArgb(getColor.A, getColor.R, getColor.G, getColor.B);
                double size = double.Parse(sizeComboBox.Text);
                double border = double.Parse(borderComboBox.Text);
                _preview.setValue(Color.FromArgb(colorPen.A, colorPen.R, colorPen.G, colorPen.B), size, border);
                _preview.HandleStart(pos.X, pos.Y);
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
          
            if (_isDrawing)
            {
                if (_preview != null)
                {
                    Point pos = e.GetPosition(canvas);
                    _preview.HandleEnd(pos.X, pos.Y);

                    // Xoá hết các hình vẽ cũ
                    canvas.Children.Clear();

                    // Vẽ lại các hình trước đó
                    for (int i = 0; i < _shapes.Count; i++)
                    {
                        if (images.ContainsKey(i))
                        {
                            foreach (var image in images[i])
                            {
                                canvas.Children.Add(image);
                            }
                        }

                        var element = _shapes[i].Draw();
                        canvas.Children.Add(element);
                    }

                    // Vẽ hình preview đè lên
                    canvas.Children.Add(_preview.Draw());

                }
            }
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
            _isDrawing = false;
            
            // Thêm đối tượng cuối cùng vào mảng quản lí
            Point pos = e.GetPosition(canvas);
            if (_preview != null)
            {
                _preview.HandleEnd(pos.X, pos.Y);

                _shapes.Add(_preview);
                if (_selectedShapeName == "")
                {
                    return;
                }
                // Sinh ra đối tượng mẫu kế
                _preview = _prototypes[_selectedShapeName].Clone();

                // Ve lai Xoa toan bo
                canvas.Children.Clear();

                // Ve lai tat ca cac hinh
                for (int i = 0; i < _shapes.Count; i++)
                {
                    if (images.ContainsKey(i))
                    {
                        foreach (var image in images[i])
                        {
                            canvas.Children.Add(image);
                        }
                    }
                    var element = _shapes[i].Draw();
                    canvas.Children.Add(element);
                }
            }
        }
        private int index = -1;
        private void methodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (methodComboBox.SelectedIndex != index)
            {

                KeyValuePair<string, IShape> selectedEntry = (KeyValuePair<string, IShape>)methodComboBox.SelectedItem;


                _selectedShapeName = selectedEntry.Key;


                if (_selectedShapeName == null)
                {
                    return;
                }
                _preview = _prototypes[_selectedShapeName];
                index = methodComboBox.SelectedIndex;
            }
        }

        private void LoadImages_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Load image";
            openFileDialog.Filter = "Images|*.png;*.bmp;*.jpg";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                _preview = null;
                CreateLoadBitmap(ref canvas, openFileDialog.FileName);
            };
        }

        private void SaveImages_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            
            saveFileDialog.Filter = "Images|*.png";
            saveFileDialog.Title = "Save as PNG";
            saveFileDialog.RestoreDirectory = true;
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                String fileName = saveFileDialog.FileName;
                CreateSaveBitmap(canvas, fileName);
            }
        }
        private void CreateLoadBitmap(ref Canvas canvas, string filename)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filename, UriKind.Absolute);
            bitmap.EndInit();

            Image image = new Image();
            image.Source = bitmap;
            image.Width = bitmap.Width;
            image.Height = bitmap.Height;
            if (bitmap.Width > canvas.Width || double.IsNaN(canvas.Width))
            {
                canvas.Width = bitmap.Width > canvas.ActualWidth ? bitmap.Width : double.NaN;
            }
            if (bitmap.Height > canvas.Height || double.IsNaN(canvas.Height))
            {
                canvas.Height = bitmap.Height > canvas.ActualHeight ? bitmap.Height : double.NaN;
            }
            if (!images.ContainsKey(_shapes.Count))
            {
                images[_shapes.Count] = new List<Image>();
            }
            images[_shapes.Count].Add(image);
            canvas.Children.Add(image);
        }
        private void CreateSaveBitmap(Canvas canvas, string filename)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
            canvas.Measure(new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight));
            canvas.Arrange(new Rect(new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight)));

            renderBitmap.Render(canvas);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (FileStream file = File.Create(filename))
            {
                encoder.Save(file);
            }
        }

        

        

        private void NewFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Load image";
            openFileDialog.Filter = "Images|*.png;*.bmp;*.jpg";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                _preview = null;
                CreateLoadBitmap(ref canvas, openFileDialog.FileName);
            };
            
        }

        private void SaveFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();

            saveFileDialog.Filter = "Images|*.png";
            saveFileDialog.Title = "Save as PNG";
            saveFileDialog.RestoreDirectory = true;
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                String fileName = saveFileDialog.FileName;
                CreateSaveBitmap(canvas, fileName);
            }
        }

        private void colorBlack_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("Black");
            penColor.Background= Brushes.Black;
        }

        private void colorWhite_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("White");
            penColor.Background = Brushes.White;
        }

        private void colorGray_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("Gray");
            penColor.Background = Brushes.Gray;
        }

        private void colorSilver_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("Silver");
            penColor.Background = Brushes.Silver;
        }

        private void colorMaroon_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("Maroon");
            penColor.Background = Brushes.Maroon;
        }

        private void colorOlive_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("Olive");
            penColor.Background = Brushes.Olive;
        }

        private void colorRed_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("Red");
            penColor.Background = Brushes.Red;
        }

        private void colorMagenta_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("Magenta");
            penColor.Background = Brushes.Magenta;
        }

        private void colorOrange_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("Orange");
            penColor.Background = Brushes.Orange;
        }

        private void colorCorale_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("Corale");
            penColor.Background = Brushes.Coral;
        }

        private void colorYellow_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("Yellow");
            penColor.Background = Brushes.Yellow;
        }

        private void colorLightYellow_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("LightYellow");
            penColor.Background = Brushes.LightYellow;
        }

        private void colorGreen_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("Green");
            penColor.Background = Brushes.Green;
        }

        private void colorLightGreen_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("LightGreen");
            penColor.Background = Brushes.LightGreen;
        }

        private void colorDarkCyan_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("DarkCyan");
            penColor.Background = Brushes.DarkCyan;
        }

        private void colorCyan_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("Cyan");
            penColor.Background = Brushes.Cyan;
        }

        private void colorPurple_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("Purple");
            penColor.Background = Brushes.Purple;
        }

        private void colorLightCoral_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("LightCoral");
            penColor.Background = Brushes.LightCoral;
        }

        private void colorBlue_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("Blue");
            penColor.Background = Brushes.Blue;
        }

        private void colorLightBlue_Click(object sender, RoutedEventArgs e)
        {
            colorPen = System.Drawing.Color.FromName("LightBlue");
            penColor.Background = Brushes.LightBlue;
        }

        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            List<Data> data= new List<Data>();
            for (int i = 0; i < _shapes.Count; i++)
            {
                Data newData = new Data();
                newData.Name = _shapes[i].Name;
                _shapes[i].getValueSave(ref newData.Color, ref newData._leftTop, ref newData._rightBottom, ref newData.StrokeThickness, ref newData.Border);
                data.Add(newData);
            }
            //string jsonShape = JsonSerializer.Serialize(data);
            string jsonShape = JsonConvert.SerializeObject(data.ToArray());
            File.WriteAllText("shape.json", jsonShape);

            List<Data> dataRedos = new List<Data>();
            for (int i = 0; i < _redos.Count; i++)
            {
                Data newData = new Data();
                newData.Name = _redos[i].Name;
                _redos[i].getValueSave(ref newData.Color, ref newData._leftTop, ref newData._rightBottom, ref newData.StrokeThickness, ref newData.Border);
                dataRedos.Add(newData);
            }

            string jsonRedos = JsonConvert.SerializeObject(dataRedos.ToArray());
            File.WriteAllText("redos.json", jsonRedos);
            //string jsonRedos = JsonSerializer.Serialize(_redos);
            //File.WriteAllText("redos.json", jsonRedos);
        }

        private void RibbonWindow_SourceInitialized(object sender, EventArgs e)
        {
            List<Data> dataShape = new List<Data>();
            List<Data> dataRedos = new List<Data>();
            try
            {
                using (StreamReader r = new StreamReader("shape.json"))
                {
                    string json = r.ReadToEnd();
                    dataShape = JsonConvert.DeserializeObject<List<Data>>(json);
                }
            }
            catch (Exception)
            { }
            try
            {
                using (StreamReader r = new StreamReader("redos.json"))
                {
                    string json = r.ReadToEnd();
                    dataRedos = JsonConvert.DeserializeObject<List<Data>>(json);
                }
            }
            catch (Exception)
            { }
            

            for(int i =0;i< dataShape.Count; i++)
            {
                IShape tmp = _prototypes[dataShape[i].Name].Clone();
                tmp.setValueSave(ref dataShape[i].Color, ref dataShape[i]._leftTop, ref dataShape[i]._rightBottom, ref dataShape[i].StrokeThickness, ref dataShape[i].Border);
                _shapes.Add(tmp);
            }
            // Ve lai tat ca cac hinh
            for (int i = 0; i < _shapes.Count; i++)
            {
                if (images.ContainsKey(i))
                {
                    foreach (var image in images[i])
                    {
                        canvas.Children.Add(image);
                    }
                }
                var element = _shapes[i].Draw();
                canvas.Children.Add(element);
            }
            if (dataRedos == null)
            {
                return;
            }
            for (int i = 0; i < dataRedos.Count; i++)
            {
                IShape tmp = _prototypes[dataRedos[i].Name].Clone();
                tmp.setValueSave(ref dataRedos[i].Color, ref dataRedos[i]._leftTop, ref dataRedos[i]._rightBottom, ref dataRedos[i].StrokeThickness, ref dataRedos[i].Border);
                _redos.Add(tmp);
            }
        }

        private void btnUndot_Click(object sender, RoutedEventArgs e)
        {

            if (_shapes.Count > 0)
            {
                _redos.Add(_shapes[_shapes.Count - 1]);
                _shapes.RemoveAt(_shapes.Count - 1);
            }
            try
            {
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            _shapes.Clear();
            _redos.Clear();
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            if (_redos.Count > 0)
            {
                _shapes.Add(_redos[_redos.Count - 1]);
                canvas.Children.Add(_shapes[_shapes.Count - 1].Draw());
                _redos.RemoveAt(_redos.Count - 1);
            }
        }

    }
}
