using Contract;
using Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        }
        List<IShape> _shapes = new List<IShape>();
        Dictionary<string, IShape> _prototypes =
            new Dictionary<string, IShape>();
        string _selectedShapeName = "";
        bool _isDrawing = false;
        IShape _preview;
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
            //System.Windows.Controls.ComboBox methodComboBox = (System.Windows.Controls.ComboBox)sender;
            KeyValuePair < string, IShape > selectedEntry
    = (KeyValuePair < string, IShape>)methodComboBox.SelectedItem;

            ////var tmp  = methodComboBox.SelectedItem as Dictionary<string, IShape>;
            _selectedShapeName = selectedEntry.Key;


            if (_selectedShapeName == null)
            {
                return;
            }
            _preview = _prototypes[_selectedShapeName];
            _isDrawing = true;

            Point pos = e.GetPosition(canvas);

            _preview.HandleStart(pos.X, pos.Y);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
          
            if (_isDrawing)
            {
                Point pos = e.GetPosition(canvas);
                _preview.HandleEnd(pos.X, pos.Y);

                // Xoá hết các hình vẽ cũ
                canvas.Children.Clear();

                // Vẽ lại các hình trước đó
                foreach (var shape in _shapes)
                {
                    UIElement element = shape.Draw();
                    canvas.Children.Add(element);
                }

                // Vẽ hình preview đè lên
                canvas.Children.Add(_preview.Draw());

                
            }
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
            _isDrawing = false;

            // Thêm đối tượng cuối cùng vào mảng quản lí
            Point pos = e.GetPosition(canvas);
            _preview.HandleEnd(pos.X, pos.Y);
            _shapes.Add(_preview);

            // Sinh ra đối tượng mẫu kế
            _preview = _prototypes[_selectedShapeName].Clone();

            // Ve lai Xoa toan bo
            canvas.Children.Clear();

            // Ve lai tat ca cac hinh
            foreach (var shape in _shapes)
            {
                var element = shape.Draw();
                canvas.Children.Add(element);
            }
        }
    }
}
