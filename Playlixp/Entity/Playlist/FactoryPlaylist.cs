using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Playlixp.Entity.Playlist
{
    class FactoryPlaylist
    {
        public static BasePlaylist createPlaylist(string filePath)
        {
            BasePlaylist playlist;
            string fileExtension = Path.GetExtension(filePath);

            switch (fileExtension)
            {
                case ".wpl":
                    playlist = new Wpl(filePath);
                    break;
                case ".m3u":
                    playlist = new M3u(filePath);
                    break;
                case ".txt":
                    playlist = new M3u(filePath);
                    break;
                default:
                    playlist = null;
                    break;
            }

            return playlist;
        }
    }
}
