using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace cf.Entities
{
    public class MapItemLine : MapItem
    {
        private Collection<Point> _line;
        
        public MapItemLine(Point[] line)
        {
            _line = new Collection<Point>(line);
            GT = "Line"; 
            C = GetSequenceOfPoints(_line);
        }
    }
}
