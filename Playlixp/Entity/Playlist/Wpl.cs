using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Playlixp.Entity.Playlist
{
    class Wpl : BasePlaylist
    {
        //Get the songs under this xml node
        public const string _parentTagName = "media";

        public Wpl(string filePath): base(filePath)
        {
        }

        public Wpl(string filePath, string displayName):base(filePath, displayName)
        {
        }

        protected override void LoadSong()
        {
            using (XmlTextReader reader = new XmlTextReader(this.PlaylistPath))
            {
                while (reader.Read())
                {
                    if (reader.Name != _parentTagName)
                        continue;
                    reader.MoveToAttribute("src");

                    if (Path.IsPathRooted(reader.Value))
                        this.Songs.Add(new Song(reader.Value));
                    else
                        this.Songs.Add(new Song(Path.Combine(Path.GetDirectoryName(this.PlaylistPath), reader.Value)));
                }
            }
        }
    }
}
