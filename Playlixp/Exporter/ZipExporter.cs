using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Playlixp.Entity;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using Playlixp.Entity.Playlist;
using System.ComponentModel;
using System.Collections;

namespace Playlixp.Exporter
{
    class ZipExporter : BaseExporter
    {
        private BackgroundWorker _backgroundWorker;
        
        private int _compressionLevel;
        /// <summary>
        /// Gets the compression level.
        /// </summary>
        public int CompressionLevel
        {
            get { return _compressionLevel; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipExporter"/> class with no compression.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        public ZipExporter(BasePlaylist playlist) : this (playlist, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipExporter"/> class.
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        /// <param name="compressionLevel">The compression level, 0 is min, 9 is max.</param>
        public ZipExporter(BasePlaylist playlist, int compressionLevel) : base(playlist)
        {
            _compressionLevel = compressionLevel < 0 ? 0 : compressionLevel > 9 ? 9 : compressionLevel;

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.DoWork += new DoWorkEventHandler(bw_DoWork);
        }

        /// <summary>
        /// Exports the playlist to the specified export path.
        /// </summary>
        /// <param name="exportPath">The export path.</param>
        public override void Export(string exportPath)
        {
            _backgroundWorker.RunWorkerAsync(exportPath);
        }

        /// <summary>
        /// Adds a ProgressChanged event handler to the worker.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        public void AddProgressChangedEventHandler(ProgressChangedEventHandler eventHandler)
        {
            if (_backgroundWorker != null)
                _backgroundWorker.ProgressChanged += eventHandler;
        }

        /// <summary>
        /// Adds a WorkerCompleted event handler to the worker.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        public void AddWorkerCompletedEventHandler(RunWorkerCompletedEventHandler eventHandler)
        {
            if (_backgroundWorker != null)
                _backgroundWorker.RunWorkerCompleted += eventHandler;
        }

        /// <summary>
        /// Handles the DoWork event of the backgroundWorker.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
                string zipOutputPath = e.Argument.ToString();

                FileStream fsOut = File.Create(zipOutputPath);
                ZipOutputStream zipStream;
                ZipEntry newEntry;
                byte[] buffer;

                zipStream = new ZipOutputStream(fsOut);
                zipStream.SetLevel(0); //0-9, 9 being the highest level of compression
                zipStream.UseZip64 = UseZip64.On;
            
                decimal counter = 0;
                decimal totalCount = _playlist.Songs.Count(s => s.Status == SongStatus.Enabled);

                foreach (Song song in _playlist.Songs)
                {
                    if (song.Status != SongStatus.Enabled)
                        continue;

                    //Report progress
                    decimal percentDone = (counter / totalCount)*100;
                    _backgroundWorker.ReportProgress((int)percentDone, song);

                    //Add file entry to zip
                    newEntry = new ZipEntry(ZipEntry.CleanName(song.FileName));
                    zipStream.PutNextEntry(newEntry);

                    // Zip the file in buffered chunks
                    buffer = new byte[4096];
                    using (FileStream streamReader = File.OpenRead(song.FilePath))
                    {
                        StreamUtils.Copy(streamReader, zipStream, buffer);
                    }
                    zipStream.CloseEntry();

                    counter++;
                }

                zipStream.IsStreamOwner = true;	// Makes the Close also Close the underlying stream
                zipStream.Close();
        }
    }
}
