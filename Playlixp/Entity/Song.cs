using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Playlixp.Entity
{
    
    public enum SongStatus
    {
        Enabled, Disabled, InvalidPath, Added
    }

    class Song
    {
        private string _songPath;
        private string _songName;
        private SongStatus _status;

        public string FilePath
        {
            get { return _songPath; }
            set { _songPath = value; }
        }

        public string FileName
        {
            get { return _songName; }
            set { _songName = value; }
        }

        public SongStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public Song(String filePath)
        {
            _songName = Path.GetFileName(filePath);
            _songPath = filePath;
            _status = IsAccessible() ? SongStatus.Enabled : SongStatus.InvalidPath;
        }

        public Song(String folderPath, String fileName) : this(Path.Combine(folderPath, fileName))
        {
        }

        public bool IsAccessible()
        {
            return File.Exists(this.FilePath);
        }

        public override string ToString()
        {
            return _songName;
        }
    }
}
