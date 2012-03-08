using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Playlixp.Entity.Playlist
{
    class M3u : BasePlaylist
    {
        //Get the songs under this xml node
        public const string _parentTagName = "media";

        public M3u(string filePath): base(filePath)
        {
        }

        public M3u(string filePath, string displayName): base(filePath, displayName)
        {
        }

        protected override void LoadSong()
        {
            foreach (string line in ReadLines(this.PlaylistPath))
            {
                //Skip comment line
                if (line[0].Equals('#')) continue;

                if (Path.IsPathRooted(line))
                    this.Songs.Add(new Song(line));
                else
                    this.Songs.Add(new Song(Path.Combine(Path.GetDirectoryName(this.PlaylistPath), line)));
            }
        }

        private IEnumerable<String> ReadLines(string filePath) {
            string line;
            using(var reader = File.OpenText(filePath)) {
                while((line = reader.ReadLine()) != null) {
                    yield return line;
                }
            }
        }

    }
}
