// <copyright file="TileServer.cs" company="shrpr.io">
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
    /// Represents a Slippy Map Tile Server
    /// </summary> 
    /// <remarks>
    /// A Tile Server serves up SlippyMap Tiles by using a server specific URL Template that generally 
    /// follows a certain style <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Tile_servers" />
    /// as defined in the TMS (Tile Map Service) protocol <see href="http://wiki.openstreetmap.org/wiki/TMS" />
    /// </remarks> 
				
    public class TileServer
    {
        /// <summary>
        /// Internal stores for public properties of this class.
        /// Naming convention: lowercase first letter of the equivalent property.
        /// </summary>
        private string _uriFormat;
        private char[] _subdomains;
        private ushort _maxZoom;
        private string _imgFormat;

        /// <summary>
        /// The class constructor. 
        /// </summary>
        /// <param name="uriFormat"> 
        /// The URI format of the Tile Server, generally:
        /// http://{servernode}.tile.server.org/{zoomlevel}/{x}/{y}.{imageformat}
        /// <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Tile_servers" />
        /// </param>
        /// <param name="subdomains">
        /// A character array for the Tile Server's subdomains as generally, several subdomains (server names) 
        /// are used by the provider to get around browser limitations on the number of simultaneous HTTP connections. 
        /// <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Tile_servers" />
        /// </param>
        public TileServer(string uriFormat, char[] subdomains, ushort maxZoom, string imgFormat)
        {
            this._uriFormat = uriFormat;

            this._subdomains = subdomains;
            this._maxZoom = maxZoom;
            this._imgFormat = imgFormat;
        }

        /// <summary>
        /// UriFormat property
        /// </summary>
        /// <value>
        /// The URI format of the Tile Server, generally:
        /// http://{subdomain}.tile.server.org/{zoomlevel}/{x}/{y}.{imageformat}
        /// <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Tile_servers" />
        /// </value>
        public string UriFormat
        {
            get { return _uriFormat; }
            set { _uriFormat = value; }
        }

        /// <summary>
        /// Subdomains property
        /// </summary>
        /// <value>
        /// A character array for the Tile Server's subdomains as generally, several subdomains (server names) 
        /// are used by the provider to get around browser limitations on the number of simultaneous HTTP connections. 
        /// <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Tile_servers" />
        /// </value>
        public char[] Subdomains
        {
            get { return _subdomains; }
            set { _subdomains = value; }
        }

        /// <summary>
        /// Maximum Zoom property
        /// </summary>
        /// <value>
        /// Maximum Zoom Level as an unsigned integer that this Tile Server can output
        /// <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Tile_servers" />
        /// <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Zoom_levels" />
        /// </value>
        public ushort MaxZoom
        {
            get { return _maxZoom; }
            set { _maxZoom = value; }
        }

        /// <summary>
        /// Image Format property
        /// </summary>
        /// <value>
        /// The image format (e.g. png, jpg) that this Tile Server uses.
        /// <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Tile_servers" />
        /// </value>
        public string imgFormat
        {
            get { return _imgFormat; }
            set { _imgFormat = value; }
        }


    }

}
