using Contract;
using Microsoft.Win32;
using MyPaint.Adorners;
using ProjectPaint.Adorners;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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

namespace ProjectPaint
{
    class StyleShape
    {
        public Color outlineColor { get; set; }
        public int widthStroke { get; set; }
        public string typeStroke { get; set; }

        public StyleShape()
        {
            this.outlineColor = Colors.Black;
            this.widthStroke = 1;
            this.typeStroke = "Solid";
        }
    }
    class StyleText
    {
        public FontFamily font { get; set; }
        public int size { get; set; }
        public TextStyle style { get; set; }

        public StyleText(FontFamily font)
        {
            this.font = font;
            this.size = 13;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            setFont();
        }

        private void setFont()
        {
            fontTextCombobox.ItemsSource = Fonts.SystemFontFamilies;
            fontTextCombobox.SelectedIndex = 211;
        }

        bool _isDrawing = false;
        List<IShape> _shapes = new List<IShape>();
        List<object> _buttons = new List<object>();
        IShape _preview;
        string _selectedShapeName = "";
        Dictionary<string, IShape> _prototypes =
            new Dictionary<string, IShape>();

        private AdornerLayer AddLayer;
        private CanvasAdorner AddCanvas;
        StyleShape style = new StyleShape();
        StyleText styleText = new StyleText(new FontFamily("Times New Roman"));
        DrawType drawType;
        Image copyImage = new Image();
        bool checkPasted = false;
        Adorner copyAdorner;

        private void canvas_MouseDown(object sender,
            MouseButtonEventArgs e)
        {
            Point startPoint = e.GetPosition(canvas);
            switch (drawType)
            {
                case DrawType.CShape:
                case DrawType.CSelect:
                case DrawType.CText:
                    DrawShape_MouseDown(startPoint);
                    break;
                case DrawType.CFill:
                    break;
                default:
                    break;
            }

        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(canvas);
            switch (drawType)
            {
                case DrawType.CShape:
                case DrawType.CSelect:
                case DrawType.CText:
                    DrawShape_MouseMove(pos);
                    break;
                case DrawType.CFill:
                    break;
                default:
                    break;
            }
        }



        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(canvas);
            switch (drawType)
            {
                case DrawType.CShape:
                    DrawShape_MouseUp(pos);
                    break;
                case DrawType.CText:
                    DrawText_MouseUp(pos);
                    break;
                case DrawType.CSelect:
                    DrawSelect_MouseUp(pos);
                    break;
                case DrawType.CFill:
                    break;
                default:
                    break;
            }
        }



        private void DrawShape_MouseDown(Point pos)
        {
            _isDrawing = true;

            _preview = _prototypes[_selectedShapeName].Clone();
            _preview.HandleStart(pos.X, pos.Y);

            _preview.setColor(style.outlineColor);
            _preview.setWidth(style.widthStroke);
            _preview.setStyle(style.typeStroke);

            if (drawType == DrawType.CText)
            {
                if (textBox != null)
                {
                    textBox.BorderBrush = Brushes.Transparent;
                    textBox.Focusable = false;
                    if (AddLayer != null)
                    {
                        Adorner[] adorners = AddLayer.GetAdorners(textBox);
                        if (adorners != null)
                        {
                            foreach (var adorner in adorners)
                                AddLayer.Remove(adorner);
                        }
                    }
                }

            }
        }
        private void DrawShape_MouseMove(Point pos)
        {
            if (_isDrawing)
            {
                _preview.HandleEnd(pos.X, pos.Y);

                _preview.DrawMove(canvas);
            }
        }
        private void DrawShape_MouseUp(Point pos)
        {
            _isDrawing = false;
            _preview.HandleEnd(pos.X, pos.Y);

            // Add IShape into _shapes
            _shapes.Add(_preview);
            //Draw again
            _preview.DrawMove(canvas);

        }

        private bool CheckDrawObjectTooSmall(Point2D point1, Point2D point2)
        {
            if (Math.Abs(point1.X - point2.X) < 2 || Math.Abs(point1.Y - point2.Y) < 2)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handle Text
        /// </summary>
        /// <param name="pos"></param>
        /// 

        TextBox textBox;
        TextBlock textBlock = null;
        private void DrawText_MouseUp(Point pos)
        {

            _isDrawing = false;
            _preview.HandleEnd(pos.X, pos.Y);

            if (CheckDrawObjectTooSmall(_preview.GetPoint1(), _preview.GetPoint2()))
            {
                return;
            }

            _preview.DrawMove(canvas);

            ((TextBox2D)_preview).setStyle(styleText.style, styleText.size, styleText.font.ToString());

            textBox = ((TextBox2D)_preview).DrawTextBox(canvas);
            textBox.LostFocus += TextBox_LostFocus;

            canvas.Children.Add(textBox);
            Canvas.SetLeft(textBox, ((TextBox2D)_preview).GetPoint1().X);
            Canvas.SetTop(textBox, ((TextBox2D)_preview).GetPoint1().Y);
            Adorner txtbox_adorner = new TextBoxAdorner((TextBox)textBox);
            AddLayer.Add(txtbox_adorner);
            textBox.Focus();
            ((TextBox2D)_preview).RemoveOutline(canvas);

        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBox != null)
            {
                textBlock = new TextBlock()
                {
                    Text = textBox.Text,
                    Width = textBox.Width,
                    Height = textBox.Height,
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = textBox.FontFamily,
                    FontSize = textBox.FontSize,
                    RenderTransform = textBox.RenderTransform,
                    RenderTransformOrigin = textBox.RenderTransformOrigin,
                    FontWeight=textBox.FontWeight,
                    FontStyle=textBox.FontStyle,
                    TextDecorations=textBox.TextDecorations
                };

                Canvas.SetLeft(textBlock, Canvas.GetLeft(textBox));
                Canvas.SetTop(textBlock, Canvas.GetTop(textBox));

                canvas.Children.Add(textBlock);
                canvas.Children.Remove(textBox);

                Point2D point1 = new Point2D { X = Canvas.GetLeft(textBlock), Y = Canvas.GetTop(textBlock) };
                Point2D point2 = new Point2D { X = Canvas.GetLeft(textBlock) + textBlock.Width, Y = Canvas.GetTop(textBlock) + textBlock.Height };

                //Add textbox vao mang quan ly 
                ((TextBox2D)_preview).setNew(styleText.style, textBlock.FontSize, textBlock.FontFamily.ToString(), textBlock.Text, point1, point2);

                _shapes.Add(_preview);
            }

        }
        /// <summary>
        /// Handle Select
        /// </summary>
        /// <param name="pos"></param>
        /// 
        Image currentSelectedImage = null;
        Image finalImage = null;
        IShape newReplace;
        private void DrawSelect_MouseUp(Point pos)
        {
            _isDrawing = false;
            _preview.HandleEnd(pos.X, pos.Y);

            _preview.DrawMove(canvas);

            if (copyAdorner != null)
            {
                AddLayer.Remove(copyAdorner);
            }
            if (CheckDrawObjectTooSmall(_preview.GetPoint1(), _preview.GetPoint2()))
            {
                btnCut.IsEnabled = false;
                btnCopy.IsEnabled = false;
                checkSelect = false;
                return;
            }
            newReplace = _preview;
            if (currentSelectedImage != null)
            {
                if (AddLayer != null)
                {
                    Adorner[] adorners = AddLayer.GetAdorners(currentSelectedImage);
                    if (adorners != null)
                    {
                        foreach (var adorner in adorners)
                            AddLayer.Remove(adorner);
                    }
                }

            };
            //Remove select region Rectangle
            ((Select2D)_preview).RemoveOutline(canvas);

            currentSelectedImage = CanvasUltilities.Crop(canvas, ((Select2D)_preview).GetPoint1().X, ((Select2D)_preview).GetPoint1().Y, ((Select2D)_preview).width, ((Select2D)_preview).height);

            if (currentSelectedImage != null)
            {
                currentSelectedImage.Focusable = true;
                currentSelectedImage.LostFocus += Select_LostFocus;

                //Replace current imageSelect with White Rectangle
                ((Select2D)_preview).ReplaceNewRectangle(canvas);

                //Set image current
                currentSelectedImage.Stretch = Stretch.Fill;
                currentSelectedImage.StretchDirection = StretchDirection.Both;
                Canvas.SetLeft(currentSelectedImage, ((Select2D)_preview).GetPoint1().X);
                Canvas.SetTop(currentSelectedImage, ((Select2D)_preview).GetPoint1().Y);
                currentSelectedImage.Width = currentSelectedImage.Source.Width;
                currentSelectedImage.Height = currentSelectedImage.Source.Height;

                //Add currentImage to canvas
                canvas.Children.Add(currentSelectedImage);

                // Add layer
                Adorner image_adorner = new SelectAdorner(currentSelectedImage);
                AddLayer.Add(image_adorner);
                currentSelectedImage.Focus();
                btnCut.IsEnabled = true;
                btnCopy.IsEnabled = true;
                checkSelect = true;
            }
        }

        private void Select_LostFocus(object sender, RoutedEventArgs e)
        {
            if (currentSelectedImage != null)
            {
                finalImage = new Image();
                finalImage.Source = currentSelectedImage.Source;
                finalImage.Width = currentSelectedImage.Width;
                finalImage.Height = currentSelectedImage.Height;
                finalImage.RenderTransformOrigin = currentSelectedImage.RenderTransformOrigin;
                finalImage.RenderTransform = currentSelectedImage.RenderTransform;
                Canvas.SetLeft(finalImage, Canvas.GetLeft(currentSelectedImage));
                Canvas.SetTop(finalImage, Canvas.GetTop(currentSelectedImage));
                //Add image
                canvas.Children.Add(finalImage);
                //Add select vao mang quan ly
                ((Select2D)_preview).setImage(finalImage, Canvas.GetLeft(currentSelectedImage), Canvas.GetTop(currentSelectedImage));

                _shapes.Add(newReplace);
                _shapes.Add(_preview);

                //Remove current select
                canvas.Children.Remove(currentSelectedImage);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void prototypeButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedShapeName = (sender as Button).Tag as string;
            

            _preview = _prototypes[_selectedShapeName];
            drawType = DrawType.CShape;

            SetBackgroundButtonList(sender);


        }

        private void SetBackgroundButtonList(object sender)
        {
            foreach (object o in _buttons)
            {
                SetBackgroundButton(o, Brushes.Lavender);
            }
            SetBackgroundButton(sender, Brushes.LightGray);

            if (drawType == DrawType.CText)
            {
                textPanel.Visibility = Visibility.Visible;
                styleText.style = TextStyle.NONE;
                U.Background = Brushes.LightGray;
                I.Background = Brushes.LightGray;
                B.Background = Brushes.LightGray;


            }
            else
            {
                textPanel.Visibility = Visibility.Hidden;

            }
        }


        private void SetBackgroundButton(object sender, SolidColorBrush color)
        {
            Type t = sender.GetType();
            MethodInfo method = t.GetMethod("set_Background");
            object[] colors = new object[1];
            colors[0] = color;
            method.Invoke(sender, colors);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            _prototypes.Add("Ellipse", new Ellipse2D());
            _prototypes.Add("Circle", new Circle2D());
            _prototypes.Add("Rectangle", new Rectangle2D());
            _prototypes.Add("Square", new Square2D());
            _prototypes.Add("Line", new Line2D());

            btnStyles.Add(B);
            btnStyles.Add(I);
            btnStyles.Add(U);


            //btnTool.Add(btnText);
            //btnTool.Add(btnCut);
            //btnTool.Add(btnCopy);
            //btnTool.Add(btnPaste);

            btnCut.IsEnabled = false;
            btnCopy.IsEnabled = false;
            btnPaste.IsEnabled = false;
            borderText.IsEnabled = false;

            //LOAD DLL
            //var exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            //var dlls = new DirectoryInfo(exeFolder).GetFiles("*.dll");

            //foreach (var dll in dlls)
            //{
            //    var assembly = Assembly.LoadFile(dll.FullName);
            //    var types = assembly.GetTypes();

            //    foreach (var type in types)
            //    {
            //        if (type.IsClass)
            //        {
            //            if (typeof(IShape).IsAssignableFrom(type))
            //            {
            //                var shape = Activator.CreateInstance(type) as IShape;
            //                _prototypes.Add(shape.Name, shape);
            //            }
            //        }
            //    }
            //}

            // Tạo ra các nút bấm hàng mẫu
            foreach (var item in _prototypes)
            {
                var shape = item.Value as IShape;
                var button = new Button()
                {
                    Content = shape.Name,
                    Width = 80,
                    Height = 25,
                    Margin = new Thickness(2, 2, 2, 2),
                    Tag = shape.Name,
                    Name = shape.Name,
                    Background = Brushes.Lavender
                };
                button.Click += prototypeButton_Click;
                prototypeShape.Children.Add(button);
                _buttons.Add(button);
            }

            _prototypes.Add("Textbox", new TextBox2D());
            _prototypes.Add("Select", new Select2D());

            _selectedShapeName = _prototypes.First().Value.Name;
            _preview = _prototypes[_selectedShapeName].Clone();
            SetBackgroundButton(_buttons[0], Brushes.LightGray);
            drawType = DrawType.CShape;
            _buttons.Add(btnSelect);
            _buttons.Add(btnText);
        }

        private void SaveShape_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveShape = new Microsoft.Win32.SaveFileDialog();
            saveShape.FileName = "Shape";
            saveShape.DefaultExt = ".bin";
            saveShape.Filter = "Binary File (*.bin)|*.bin |Png Image (.png)|*.png";
            Nullable<bool> result = saveShape.ShowDialog();
            string fileName = saveShape.FileName;
            if (fileName != "")
            {
                if (System.IO.Path.GetExtension(fileName).Contains("bin"))
                {
                    saveShapeBinary(fileName);
                }
                else
                {
                    SaveShapePng(fileName);
                }
            }

        }

        private void saveShapeBinary(string fileName)
        {
            using (var stream = File.OpenWrite(fileName))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, _shapes);
            }
        }

        private void LoadShape_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog loadShapeDialog = new OpenFileDialog();
            loadShapeDialog.Filter = "Binary File (*.bin)|*.bin|Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...";
            loadShapeDialog.Title = "Chọn file Shape";
            loadShapeDialog.ShowDialog();
            string fileName = loadShapeDialog.FileName;
            if (fileName != "")
            {
                if (System.IO.Path.GetExtension(fileName).Contains("bin"))
                {
                    loadShapeBinary(fileName);
                }
                else
                {
                    LoadShapePng(fileName);
                }
            }

        }

        private void loadShapeBinary(string fileName)
        {
            canvas.Children.Clear();
            using (var stream = File.OpenRead(fileName))
            {
                var formatter = new BinaryFormatter();
                _shapes = formatter.Deserialize(stream) as List<IShape>;

            }
            foreach (var shape in _shapes)
            {
                canvas.Children.Add(shape.Draw());
            }
        }

        private void _colorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            style.outlineColor = (Color)_colorPicker.SelectedColor;
            Color color = style.outlineColor;
        }


        private void myUpDownControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            style.widthStroke = (int)myUpDownControl.Value;
        }

        private void comboBoxStroke_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem cbi = (ComboBoxItem)comboBoxStroke.SelectedItem;
            style.typeStroke = cbi.Tag.ToString();
        }

        private void SaveShapePng(string path)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width,
    (int)canvas.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(canvas);
            var crop = new CroppedBitmap(rtb, new Int32Rect(0, 0, (int)canvas.RenderSize.Width, (int)canvas.RenderSize.Height));

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(crop));
            try
            {
                // Delete the file if it exists.
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                FileStream fs = File.Create(path);
                fs.Close();
                using (var fss = System.IO.File.OpenWrite(path))
                {
                    pngEncoder.Save(fss);
                    fss.Close();
                }

            }
            catch
            {
                Debug.WriteLine("Writing File " + path + " happens errors");
            }
        }

        private void LoadShapePng(string path)
        {
            canvas.Children.Clear();
            _shapes.Clear();

            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(path, UriKind.Relative));
            canvas.Width = brush.ImageSource.Width;
            canvas.Height = brush.ImageSource.Height;
            border.Width = canvas.Width;
            border.Height = canvas.Height;
            canvas.Background = brush;
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            AddLayer = AdornerLayer.GetAdornerLayer(sender as Canvas);
            AddCanvas = new CanvasAdorner(canvas, border);
            AddLayer.Add(AddCanvas);
        }

        List<object> btnStyles = new List<object>();

        private void myUpDownControlText_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            styleText.size = (int)myUpDownControlText.Value;
        }

        private void SelelectFont_Changed(object sender, SelectionChangedEventArgs e)
        {
            
            FontFamily font = (FontFamily)fontTextCombobox.SelectedItem;
            styleText.font = font;
        }
        private void Text_Click(object sender, RoutedEventArgs e)
        {
            borderText.IsEnabled = true;
            drawType = DrawType.CText;
            _selectedShapeName = "Textbox";
            SetBackgroundButtonList(sender);

        }

        private void UnderLineText_Click(object sender, RoutedEventArgs e)
        {
            SetBackgroundButtonStyle(U, btnStyles);
            if (styleText.style == TextStyle.UNDERLINE && styleText.style != TextStyle.NONE)
            {
                styleText.style = TextStyle.NONE;
            }
            else
            {
                styleText.style = TextStyle.UNDERLINE;
            }
        }

        private void SetBackgroundButtonStyle(Button u, List<object> btnStyles)
        {
            foreach (object o in btnStyles)
            {
                SetBackgroundButton(o, Brushes.Lavender);
            }
            u.Background = Brushes.LightGray;
            if ((u.Tag.ToString() == "Bold" && styleText.style == TextStyle.BOLD) ||
                (u.Tag.ToString() == "Italic" && styleText.style == TextStyle.ITALIC) ||
                (u.Tag.ToString() == "Underline" && styleText.style == TextStyle.UNDERLINE))
            {
                u.Background = Brushes.Lavender;
            }
        }

        private void BoldText_Click(object sender, RoutedEventArgs e)
        {
            SetBackgroundButtonStyle(B, btnStyles);
            if (styleText.style == TextStyle.BOLD && styleText.style != TextStyle.NONE)
            {
                styleText.style = TextStyle.NONE;
            }
            else
            {
                styleText.style = TextStyle.BOLD;
            }
        }

        private void ItalicText_Click(object sender, RoutedEventArgs e)
        {
            SetBackgroundButtonStyle(I, btnStyles);
            if (styleText.style == TextStyle.ITALIC && styleText.style != TextStyle.NONE)
            {
                styleText.style = TextStyle.NONE;
            }
            else
            {
                styleText.style = TextStyle.ITALIC;
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
           
            drawType = DrawType.CSelect;
            SetBackgroundButtonList(sender);

            _selectedShapeName = "Select";
            if (copyAdorner != null)
            {
                AddLayer.Remove(copyAdorner);
            }
          
            
        }

        Boolean checkSelect = false;
        private void btnCut_Click(object sender, RoutedEventArgs e)
        {

            if (copyImage != null && checkSelect==false)
            {
                canvas.Children.Remove(copyImage);
            }
            if (copyAdorner != null)
            {
                AddLayer.Remove(copyAdorner);
            }
            if (currentSelectedImage == null) return;
            copyImage = currentSelectedImage;
            copyAdorner = new SelectAdorner(currentSelectedImage);
            AddLayer.Add(copyAdorner);
            canvas.Children.Remove(finalImage);
            canvas.Children.Remove(currentSelectedImage);
            checkPasted = false;
            btnPaste.IsEnabled = true;
            btnCopy.IsEnabled = false;
            btnCut.IsEnabled = false;
            checkSelect = false;
        }


        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (copyAdorner!=null)
            {
                AddLayer.Remove(copyAdorner);
            }
            if (currentSelectedImage == null) return;
            copyImage = currentSelectedImage;
            copyAdorner = new SelectAdorner(copyImage);
            AddLayer.Add(copyAdorner);
            checkPasted = false;
            canvas.Children.Remove(currentSelectedImage);
            currentSelectedImage = null;
            btnPaste.IsEnabled = true;
            btnCut.IsEnabled = false;
            checkSelect = false;
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            if (checkPasted == true) return;
            if (currentSelectedImage != null)
            {
                if (AddLayer != null)
                {
                    Adorner[] adorners = AddLayer.GetAdorners(currentSelectedImage);
                    if (adorners != null)
                    {
                        foreach (var adorner in adorners)
                            AddLayer.Remove(adorner);
                    }
                }

                currentSelectedImage = null;
            }
            if (copyImage == null) return;
            {
                Canvas.SetTop(copyImage, 0);
                Canvas.SetLeft(copyImage, 0);
                canvas.Children.Add(copyImage);
                copyAdorner = new SelectAdorner(copyImage);
                AddLayer.Add(copyAdorner);
            }
            canvas.Children.Remove(currentSelectedImage);
            currentSelectedImage = new Image();
            currentSelectedImage.Source = copyImage.Source;
            checkPasted = true;
            btnPaste.IsEnabled = false;
            btnCopy.IsEnabled = true;
            btnCut.IsEnabled = true;
            checkSelect = true;

        } 
        //private void setBackgroundButtonSelect(Button button,List<object> list)
        //{
        //    foreach (object o in listpai)
        //    {
        //        SetBackgroundButton(o, Brushes.Lavender);
        //    }
        //    if (button != null)
        //    {
        //        SetBackgroundButton(button, Brushes.LightGray);
        //    }
        //}
    }
}