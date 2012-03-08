using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Playlixp.Entity.Playlist
{
	abstract class BasePlaylist
	{
        private string _playlistName;
        private string _playlistPath;
        private List<Song> _songs;

        public string PlaylistPath
        {
            get { return _playlistPath; }
            set { _playlistPath = value; }
        }

        public string PlaylistName
        {
            get { return _playlistName; }
            set { _playlistName = value; }
        }

        public List<Song> Songs
        {
            get { return _songs; }
            set { _songs = value; }
        }

        public BasePlaylist(string filePath)
            : this(filePath, Path.GetFileName(filePath))
        {

        }

        public BasePlaylist(string filePath, string displayName)
        {
            _songs = new List<Song>();
            _playlistPath = filePath;
            _playlistName = displayName;

            LoadSong();
        }

        public IEnumerable<SongStatus> ExportToZip(String zipOutputPath, int compressionLevel)
        {
            FileStream fsOut = File.Create(zipOutputPath);
            ZipOutputStream zipStream;
            ZipEntry newEntry;
            byte[] buffer;

            zipStream = new ZipOutputStream(fsOut);
            zipStream.SetLevel(0); //0-9, 9 being the highest level of compression
            zipStream.UseZip64 = UseZip64.On;

            foreach (Song song in _songs)
            {
                //Skip song if file cannot be found
                if (song.Status != SongStatus.Enabled)
                {
                    yield return song.Status;
                    continue;
                }

                newEntry = new ZipEntry(ZipEntry.CleanName(song.FileName));

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(song.FilePath))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();

                song.Status = SongStatus.Added;
                yield return SongStatus.Added;
            }

            zipStream.IsStreamOwner = true;	// Makes the Close also Close the underlying stream
            zipStream.Close();

        }

        /// <summary>
        /// To be implemented by the playlist specific type
        /// </summary>
        protected abstract void LoadSong();

        public override string ToString()
        {
            return _playlistName;
        }
	}
}
