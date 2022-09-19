using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfRectangleEditor
{
    public class MyRectangle
    {
        public Rectangle Bounds { get; set; } = new Rectangle();

        public bool IsSelected { get; set; }
        
        public int Id { get; set; }


        public MyRectangle()
        {
            Bounds.Stroke = Brushes.Blue;
            Bounds.Fill = Brushes.Transparent;
        }

        public MyRectangle DeepCopy()
        {
            return (MyRectangle)MemberwiseClone();
        }
    }
}
