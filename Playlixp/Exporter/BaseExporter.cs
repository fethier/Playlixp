using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Playlixp.Entity.Playlist;
using System.Collections;

namespace Playlixp.Exporter
{
    abstract class BaseExporter
    {
        protected BasePlaylist _playlist;

        private BaseExporter() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseExporter"/> class.
        /// </summary>
        /// <param name="playlist">The playlist to export.</param>
        public BaseExporter(BasePlaylist playlist)
        {
            _playlist = playlist;
        }

        /// <summary>
        /// Exports the playlist to the specified export path.
        /// </summary>
        /// <param name="exportPath">The export path.</param>
        /// <returns></returns>
        public abstract void Export(string exportPath);
    }
}
