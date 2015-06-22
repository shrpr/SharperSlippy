using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperSlippy
{
    public struct OSMCoordinate
    {
        /// <summary> 
        /// TILE represents the structure for a single Slippy Map tile 
        /// </summary> 
        /// <remarks>
        /// Contains X (Latitude) and Y (Longitude) values.
        /// <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames" />
        /// </remarks> 
        public OSMCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary> 
        /// LATITUDE as a numerical representation with zoom factored in
        /// </summary> 
        /// <value> 
        /// Goes from 0 (left edge is 180 °W) to 2^zoom − 1 (right edge is 180 °E)
        /// zoom level is an unsigned integer between 0 (Zoomed OUT) and 19 (Zoomed IN)</value>
        public int X;

        /// <summary> 
        /// LONGITUDE as a numerical representation with zoom factored in
        /// </summary> 
        /// <value> 
        /// Goes from 0 (left edge is 180 °W) to 2^zoom − 1 (right edge is 180 °E)
        /// zoom level is an unsigned integer between 0 (Zoomed OUT) and 19 (Zoomed IN)</value>
        public int Y;
    }
}
