// <copyright file="TileUtility.cs" company="shrpr.io">
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using SQLite;

namespace SharperSlippy
{
    /// <summary> 
    /// Utility Class to do common Slippy Map Tile operations. Built 
    /// primarily to generate MBTiles files.
    /// </summary> 

    public static class TileHelper
    {

        /// <summary>
        /// Creates a Tile Cache in MBTiles v1.1 Format <see href="https://github.com/mapbox/mbtiles-spec" />
        /// from a given Bounding Box (SouthEast Lat/Lon, NorthWest Lat/Lon, Minimum Zoom and Maximum Zoom) 
        /// </summary>
        /// <param name = "SourceTileServer">The source Tile Server where Tile Images would be grabbed from <see cref="TileServer"/></param>
        /// <param name = "SqlConnection">A valid SQLiteConnection based on the SQLite PCL-Net</param>
        /// <param name = "SELat">Latitude of SouthEast corner of map extent</param>
        /// <param name = "SELon">Longitude of SouthEast corner of map extent</param>
        /// <param name = "NWLat">Latitude of NorthWest corner of map extent</param>
        /// <param name = "NWLon">Longitude of NorthWest corner of map extent</param>
        /// <param name = "MinZoom">Minimum map Zoom Level</param>
        /// <param name = "MaxZoom">(Optional) Maximum map Zoom Level</param>
        /// <returns>Generates an MBTiles File in the specified SQLiteConnection</returns> 
        public static void CreateTileCache(TileServer SourceTileServer, SQLiteConnection SqlConnection, 
                                           float SELat, float SELon, float NWLat, float NWLon, 
                                           ushort MinZoom, ushort MaxZoom = 100)
        {
            // Calculate Maximum Zoom Level
            if (MaxZoom > SourceTileServer.MaxZoom) { MaxZoom = SourceTileServer.MaxZoom; }
            
            // Check for valid Zoom Level
            if (MinZoom > MaxZoom) {throw new Exception ("Minimum Zoom should be less than Maximum Zoom");}
            
            // Get number of subdomains for given Tile Server
            int s = 0; // Define starting subdomain
            int sMax = SourceTileServer.Subdomains.Count() - 1;

            // Setup SQLite Connection
            SQLiteCommand sqlCommand;
            string sql;

            // Create Metadata Table  
            sql = "CREATE TABLE metadata (name text, value text)";
            sqlCommand = SqlConnection.CreateCommand(sql);
            sqlCommand.ExecuteNonQuery();

            // Create Tiles Table
            sql = "CREATE TABLE tiles (zoom_level integer, tile_column integer, tile_row integer, tile_data blob)";
            sqlCommand = SqlConnection.CreateCommand(sql);
            sqlCommand.ExecuteNonQuery();

            // Insert Required Metadata
            sql = "INSERT INTO metadata VALUES ('name','Test')";
            sqlCommand = SqlConnection.CreateCommand(sql);
            sqlCommand.ExecuteNonQuery();

            sql = "INSERT INTO metadata VALUES ('type','baselayer')";
            sqlCommand = SqlConnection.CreateCommand(sql);
            sqlCommand.ExecuteNonQuery();

            sql = "INSERT INTO metadata VALUES ('version','1')";
            sqlCommand = SqlConnection.CreateCommand(sql);
            sqlCommand.ExecuteNonQuery();

            sql = "INSERT INTO metadata VALUES ('description','Test Tileset')";
            sqlCommand = SqlConnection.CreateCommand(sql);
            sqlCommand.ExecuteNonQuery();

            sql = string.Format("INSERT INTO metadata VALUES ('format','{0}')", SourceTileServer.imgFormat);
            sqlCommand = SqlConnection.CreateCommand(sql);
            sqlCommand.ExecuteNonQuery();

            sql = string.Format("INSERT INTO metadata VALUES ('bounds','{0},{1},{2},{3}')", float.Parse(SELon.ToString(), CultureInfo.InvariantCulture),
                                                                                            float.Parse(SELat.ToString(), CultureInfo.InvariantCulture),
                                                                                            float.Parse(NWLon.ToString(), CultureInfo.InvariantCulture),
                                                                                            float.Parse(NWLat.ToString(), CultureInfo.InvariantCulture));
            sqlCommand = SqlConnection.CreateCommand(sql);
            sqlCommand.ExecuteNonQuery();

            // Iteration Sequence: Bottom-Left (SE) to Top-Right (NW), from lowest Zoom Level to Highest Zoom Level
            // Iterate through all Zoom Levels (from lowest/zoomed-out to highest/zoomed-in)
            int zoomLevels = MaxZoom - MinZoom;
            int zoomLevel = MinZoom;
            for (int z = 0; z <= zoomLevels; z++)
            {
                // Set Zoom
                zoomLevel = MinZoom + z;

                System.Diagnostics.Debug.WriteLine("Zoom: " + zoomLevel.ToString());

                // Get minimum Tile X, Y values
                OSMCoordinate MinTMS = GetOSMCoordinates(SELat, SELon, zoomLevel);

                // Get maximum Tile X, Y values
                OSMCoordinate MaxTMS = GetOSMCoordinates(NWLat, NWLon, zoomLevel);


                // Iterate through all Rows (Y-Axis)  
                int numOfRows = MinTMS.Y - MaxTMS.Y;
                for (int y = 0; y <= numOfRows; y++)
                {
                    System.Diagnostics.Debug.   WriteLine("Row: " + (MinTMS.Y - y).ToString());

                    // Retrieve tiles from Left to Right (X-Axis)
                    int numOfTiles = MaxTMS.X - MinTMS.X;
                    for (int x = 0; x <= numOfTiles; x++)
                    {
                        // Fetch Tile    
                        System.Diagnostics.Debug.WriteLine("X: {0}  Y: {1}  Zoom: {2}",
                           (MinTMS.X + x).ToString(), (MinTMS.Y - y).ToString(), zoomLevel.ToString());

                        // Check subdomain to retrieve from
                        if (s > sMax) {s = 0;}

                        Tile tile = new Tile(MinTMS.X + x, MinTMS.Y - y);
                        byte[] tileBytes = FetchTile(tile, SourceTileServer, SourceTileServer.Subdomains[s], zoomLevel);
                        s++;

                        // Insert into database
                        sqlCommand.CommandText = "INSERT INTO tiles (zoom_level, tile_column, tile_row, tile_data) VALUES (@zoom,@x,@y,@image)";
                        sqlCommand.Bind("@zoom", zoomLevel);
                        sqlCommand.Bind("@x", MinTMS.X + x);
                        sqlCommand.Bind("@y", ((1 << zoomLevel) - (MinTMS.Y - y) - 1));
                        sqlCommand.Bind("@image", tileBytes);
                        
                        sqlCommand.ExecuteNonQuery();
                        
                    }
                }                
            }
            
                  
            // Close SQL Connection
            SqlConnection.Close();

        }


        /// <summary>
        /// Returns the X and Y Tile Numbers based on the OSM numbering format  <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames" /> 
        /// </summary>
        /// <param name = "Lat">Latitude in degrees/radians</param>
        /// <param name = "Lon">Longitude in degrees/radians</param>
        /// <param name = "Zoom">Zoom Level <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Zoom_levels" /> </param>
        /// <returns>Generates an MBTiles File in the specified SQLiteConnection</returns> 
        public static OSMCoordinate GetOSMCoordinates(float Lat, float Lon, int Zoom)
        {
           int X = (int)((Lon + 180.0f) / 360.0f * (1 << Zoom));
           int Y = (int)((1.0 - Math.Log(Math.Tan(Lat * Math.PI / 180.0) +
                1.0 / Math.Cos(Lat * Math.PI / 180.0f)) / Math.PI) / 2.0f * (1 << Zoom));

           return new OSMCoordinate (X,Y);
        }

        /// <summary>
        /// Fetch the Image Tile from given Tile Server  <see cref="TileServer" />
        /// </summary>
        /// <param name = "SourceTile">Source Tile <see cref="Tile"/></param>
        /// <param name = "SourceTileServer">The source Tile Server where Tile Images would be grabbed from <see cref="TileServer"/></param>
        /// <param name = "Subdomain">Subdomain of Source Tile Server to get the image from <see cref="TileServer" /></param>
        /// <param name = "Zoom">Zoom Level <see href="http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Zoom_levels" /> </param>
        /// <param name = "XOffset">(Optional) Image Offest in the X-Axis (e.g. Use 1 if you want to retrieve the tile image above the Source Tile <see cref="Tile"/></param>
        /// <param name = "YOffset">(Optional) Image Offest in the Y-Axis (e.g. Use 1 if you want to retrieve the tile image to the right of the Source Tile <see cref="Tile"/> </param>
        /// <param name = "Timeout">(Optional) Optional timeout in milliseconds /> </param>
        /// <returns>Tile Image as a Byte Array</returns> 
        public static byte[] FetchTile(Tile SourceTile, TileServer SourceTileServer, char Subdomain,
                                       int Zoom = 0, short XOffset = 0, short YOffset = 0, int Timeout = 10000)
        {
            var tileUri = GetTileUri(SourceTileServer, Subdomain, Zoom, (int)(SourceTile.X) + XOffset, (int)(SourceTile.Y) + YOffset);
            var tileRequest = (HttpWebRequest)WebRequest.Create(tileUri);
            tileRequest.UseDefaultCredentials = true;
            System.Diagnostics.Debug.WriteLine(tileRequest.RequestUri.ToString());
            try
            {
                using (var serverResponse = GetSyncResponse(tileRequest, Timeout))
                {
                    if (serverResponse == null)
                    {
                        throw (new WebException("Error fetching tile from: " + tileUri.OriginalString, null));
                    }
                    else
                    {
                        if (serverResponse.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                        {
                            using (Stream responseStream = serverResponse.GetResponseStream())
                            {
                                return ReadFully(responseStream);
                            }
                        }
                        else
                        {
                            throw (new WebException("Error fetching tile from: " + tileUri.OriginalString + Environment.NewLine
                                                    + "Instead of an image tile, the following content type " + Environment.NewLine
                                                    + "was returned instead: " + serverResponse.ContentType + Environment.NewLine
                                                    + (serverResponse.ContentType.StartsWith("text", StringComparison.OrdinalIgnoreCase) ?
                                                        ReadAllText(serverResponse.GetResponseStream()) : "")
                                                     , null));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                   
                throw (new WebException("Error fetching tile from: " + tileUri.OriginalString + Environment.NewLine +
                                        ex.Message, null));
            }
        }


        /// <summary>
        /// Creates a Tile <see cref="Tile"/>  
        /// from Longitude (X axis), Latitude (Y axis) and optional Zoom values.
        /// </summary>
        /// <param name = "lon">The longitude in degrees (radians)</param>
        /// <param name = "lat">The latitude in degrees (radians)</param>
        /// <param name = "zoom">The zoom level (0-19, 0:world, 19:street structure)</param>
        /// <returns>A Slippy Map Tile <see cref="Tile"/> </returns> 
        public static Tile CreateTileFromLatLon(float lat, float lon, ushort zoom = 0)
        {
            var tile = new Tile();
            
            tile.X = ((lon + 180.0f) / 360.0f * (1 << zoom));
            tile.Y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
                1.0 / Math.Cos(lat * Math.PI / 180.0f)) / Math.PI) / 2.0f * (1 << zoom));

            return tile;
        }


        /// <summary>
        /// Utility method that reads data from a stream until the end is reached. 
        /// The data is returned as a byte array. An IOException is thrown if any of the underlying IO calls fail. 
        /// Code from: <see href="http://www.yoda.arachsys.com/csharp/readbinary.html" />  
        /// </summary>
        /// <param name = "stream">The stream to read data from</param>
        /// <returns>A byte array <see cref="Tile"/> </returns> 
        public static byte[] ReadFully(Stream stream)
        {
            
            var buffer = new byte[32768];
            using (var ms = new MemoryStream())
            {
                while (true)
                {
                    var read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                    {
                        return ms.ToArray();
                    }
                    ms.Write(buffer, 0, read);
                }
            }
        }

        private static string ReadAllText(Stream responseStream)
        {
            using (var streamReader = new StreamReader(responseStream, true))
            {
                using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
                {
                    stringWriter.Write(streamReader.ReadToEnd());
                    return stringWriter.ToString();
                }
            }
        }

        /// <summary>
        /// Creates the URI for a requested Tile
        /// </summary>
        /// <param name="Tile">The Slippy Map Tile<see cref="Tile"/></param>
        /// <returns>The URI that will be used for the web request.</returns>
        public static Uri GetTileUri(TileServer tileServer, char subdomain, int zoom, int x, int y)
        {
            string url = string.Format(tileServer.UriFormat, subdomain, zoom.ToString(), x.ToString(), y.ToString(), tileServer.imgFormat);
            return new Uri(url.ToString());
        }

        /// <summary>
        /// A blocking operation that does not continue until a response has been
        /// received for a given <see cref="HttpWebRequest"/>, or the request
        /// timed out.
        /// </summary>
        /// <param name="request">The request to be sent.</param>
        /// <param name="timeout">An optional timeout.</param>
        /// <returns>The response that was received for the request.</returns>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/>
        /// parameter was set, and no response was received within the specified
        /// time.</exception>
        /// <remarks>
        /// This particular extension method (GetSyncResponse) was originally developed by Philipp Sumi under MS-PL with permission 
        /// to convert to Apache <see href="http://www.hardcodet.net/2010/02/blocking-httpwebrequest-getresponse-for-silverlight"/>
        /// (URL content as of 25-May-2015 11:28am GMT+2)
        /// 
        /// Now converted to AGPL from Apache Licensed RestSharp <see href="https://github.com/restsharp/RestSharp" />
        /// The invalidity or unenforceability of this license conversion for this block of code within method: GetSyncResponse
        /// shall not affect the validity or enforceability of the applied license on the rest of the code in this software 
        /// which shall remain in full force and effect.
        /// </remarks>
        public static HttpWebResponse GetSyncResponse(this HttpWebRequest request,
                                          int? timeout)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var waitHandle = new AutoResetEvent(false);
            HttpWebResponse response = null;
            Exception exception = null;

            AsyncCallback callback = ar =>
            {
                try
                {
                    //get the response
                    response = (HttpWebResponse)request.EndGetResponse(ar);
                }
                catch (WebException we)
                {
                    if (we.Status != WebExceptionStatus.RequestCanceled)
                    {
                        exception = we;
                    }
                }
                catch (Exception e)
                {
                    exception = e;
                }
                finally
                {
                    //setting the handle unblocks the loop below
                    waitHandle.Set();
                }
            };

            //request response async
            var asyncResult = request.BeginGetResponse(callback, null);
            if (asyncResult.CompletedSynchronously) return response;

            var hasSignal = waitHandle.WaitOne(timeout ?? Timeout.Infinite);
            if (!hasSignal)
            {
                try
                {
                    if (response != null)
                        return response;
                    if (request != null)
                        request.Abort();
                }
                catch
                {
                    throw new TimeoutException("No response received in time.");
                }

                throw new TimeoutException("No response received in time.");
            }

            //bubble exception that occurred on worker thread
            if (exception != null) throw exception;

            return response;
        }

    }  
}