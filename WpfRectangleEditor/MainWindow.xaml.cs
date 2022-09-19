using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WpfRectangleEditor
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point _startPoint;

        private MyRectangle _selected = null;

        private List<MyRectangle> _rectangles = new List<MyRectangle>();

        private enum Mode
        {
            Select,
            Create,
            Edit,
            Move,
            Delete
        }

        private Mode _mode = Mode.Select;

        public MainWindow()
        {
            InitializeComponent();

            CanvasBase.MouseDown += CanvasBase_MouseDown;
            CanvasBase.MouseMove += CanvasBase_MouseMove;
            CanvasBase.MouseUp += CanvasBase_MouseUp;


        }

        private void CanvasBase_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(this.CanvasBase);

            // 既存レクタングルとの当たり判定
            _selected = JudgeSelection(_startPoint);
            if (_selected != null)
            {
                _selected.Bounds.Stroke = Brushes.Red;
            }

            // 左クリック時
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_selected == null)
                {
                    _mode = Mode.Create;

                    // 既存のレクタングルがないところ⇒新規
                    var myRect = new MyRectangle();
                    _rectangles.Add(myRect);
                    _selected = myRect;

                    SetLeftTop(_startPoint);
                    CanvasBase.Children.Add(_selected.Bounds);
                }
                else
                {
                    _mode = Mode.Edit;
                    // 既存のレクタングルあり⇒形状変更、左上座標は同一座標を利用
                    _startPoint = GetLeftTop(_selected.Bounds);
                }
            }
        }

        private void CanvasBase_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                return;
            }
            if (_selected == null)
            {
                return;
            }

            if (_mode == Mode.Create || _mode == Mode.Edit)
            {
                var pointTopLeft = CalcTopLeft(e.GetPosition(CanvasBase));
                var pointBottomRight = CalcBottomRight(e.GetPosition(CanvasBase));

                UpdateRectangle(pointTopLeft, pointBottomRight);
            }
            else if (_mode == Mode.Move)
            {
                SetLeftTop(e.GetPosition(CanvasBase));
            }
        }


        private void SetLeftTop(Point point)
        {
            Canvas.SetLeft(_selected.Bounds, point.X);
            Canvas.SetTop(_selected.Bounds, point.Y);

        }

        private MyRectangle JudgeSelection(Point p)
        {
            return _rectangles.FirstOrDefault(r => IsHit(r.Bounds, p));
        }

        private bool IsHit(Rectangle r, Point p)
        {
            var lt = GetLeftTop(r);
            var left = lt.X;
            var top = lt.Y;

            return left <= p.X && p.X <= left + r.Width &&
                top <= p.Y && p.Y <= top + r.Height;

        }

        private Point GetLeftTop(Rectangle r)
        {
            return new Point(
                Canvas.GetLeft(r),
                Canvas.GetTop(r)
                );
        }


        private void UpdateRectangle(Point pointTopLeft, Point pointBottomRight)
        {
            Canvas.SetLeft(_selected.Bounds, pointTopLeft.X);
            Canvas.SetTop(_selected.Bounds, pointTopLeft.Y);

            _selected.Bounds.Width = pointBottomRight.X - pointTopLeft.X;
            _selected.Bounds.Height = pointBottomRight.Y - pointTopLeft.Y;
        }

        private Point CalcTopLeft(Point point)
        {
            // クリック開始点とマウス位置の、より左上ある座標を採用
            return new Point(
                Math.Min(point.X, _startPoint.X),
                Math.Min(point.Y, _startPoint.Y));
        }

        private Point CalcBottomRight(Point point)
        {
            return new Point(
                Math.Max(point.X, _startPoint.X),
                Math.Max(point.Y, _startPoint.Y));
        }


        private void CanvasBase_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void ContextMenuCanvas_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null)
            {
                return;
            }

            var m = (MenuItem)sender;

            if (m.Name == "Edit")
            {
                _mode = Mode.Edit;
            }
            else if (m.Name == "Move")
            {
                _mode = Mode.Move;
            }
            else if (m.Name == "Delete")
            {
                _mode = Mode.Delete;

                if (MessageBox.Show("削除？", "削除確認", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    CanvasBase.Children.Remove(_selected.Bounds);
                    _rectangles.Remove(_selected);
                    _selected = null;
                }
                _mode = Mode.Select;
            }

        }
    }
}
