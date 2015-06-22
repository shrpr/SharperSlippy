// <copyright file="Tile.cs" company="shrpr.io">
// Copyright (c) 2015 All Rights Reserved
// <author>#R (GitHub: shrpr | Twitter: _shrpr | Stackoverflow: shrpr [4935710])</author>
// </copyright>
// 
// This file is part of SharperSlippy.
// 
// SharperSlippy is free software: you can redistribute it and/or modify
// it under the terms of the Affero GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SharperSlippy is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// Affero GNU General Public License for more details.
// 
// You should have received a copy of the Affero GNU General Public License
// along with SharperSlippy.  If not, see <http://www.gnu.org/licenses/>.
				
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharperSlippy
{
    /// <summary> 
    /// Represents a single Slippy Map tile 
    /// </summary> 
    /// <remarks>
    /// Slippy Map is, in general, a term referring to modern web maps which lets 
    /// you zoom and pan around (the map slips around when you drag around).
    /// <see href="http://wiki.openstreetmap.org/wiki/Slippy_Map"/> 
    /// </remarks> 

    public struct TilePoint
    {
        /// <summary> 
        /// TILE represents the structure for a single Slippy Map tile 
        /// </summary> 
        /// <remarks>
        /// Contains X (Latitude) and Y (Longitude) values.
        /// <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames" />
        /// </remarks> 
        public TilePoint(int x, int y)
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
