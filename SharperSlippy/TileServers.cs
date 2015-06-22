// <copyright file="TileServers.cs" company="shrpr.io">
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
using System.Threading.Tasks;

namespace SharperSlippy
{
    /// <summary> 
    /// Predefined Slippy Map Tile Servers
    /// </summary> 

    public static class TileServers
    {
        public static TileServer OpenStreetMap = new TileServer("http://{0}.tile.openstreetmap.org/{1}/{2}/{3}.{4}", new[] { 'a', 'b', 'c' }, 19, "png");
        public static TileServer OpenCycleMap = new TileServer("http://{0}.tile.opencyclemap.org/cycle/{1}/{2}/{3}.{4}", new[] { 'a', 'b', 'c' }, 19, "png");
        public static TileServer MapQuestOSM = new TileServer("http://otile{0}.mqcdn.com/tiles/1.0.0/map/{1}/{2}/{3}.{4}", new[] { '1', '2', '3', '4' }, 19, "jpg");
    }
}
