using IQArchiveManager.Common;
using IQArchiveManager.Common.IO.Editor.Post;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.Util
{
    /// <summary>
    /// Class that handles writing the IQ post files as they're being edited.
    /// </summary>
    class PostFileWriter
    {
        public PostFileWriter(string filename, string finalFilename)
        {
            //Set filename
            this.filename = filename;
            this.finalFilename = finalFilename;

            //Initialize data - Either by loading or creating
            if (File.Exists(filename))
            {
                //Load data file if it exists
                data = JsonConvert.DeserializeObject<PostEditFile>(File.ReadAllText(filename));
            } else
            {
                //Initialize new
                data = new PostEditFile();
                data.EditorVersion = Constants.CURRENT_EDITOR_VERSION;
                data.LastEdited = DateTime.UtcNow;
                data.Delete = false;
                data.Edits = new List<TrackEditInfo>();
            }
        }

        private readonly string filename;
        private readonly string finalFilename;
        private readonly PostEditFile data;
        private bool finalized; // Set when the file is moved to the finalized location

        /// <summary>
        /// Gets all current edits in the file.
        /// </summary>
        public IEnumerable<TrackEditInfo> Edits => data.Edits;

        /// <summary>
        /// Adds a new edit to the file and saves it.
        /// </summary>
        /// <param name="info"></param>
        public void AddEdit(TrackEditInfo info)
        {
            data.Edits.Add(info);
            data.EditorVersion = Constants.CURRENT_EDITOR_VERSION;
            data.LastEdited = DateTime.UtcNow;
            Save();
        }

        /// <summary>
        /// Saves the file to disk.
        /// </summary>
        public void Save()
        {
            //Make sure it's not finalized
            EnsureNotFinalized();

            //Create filename of backup
            string backup = filename + ".bak";

            //Delete backup if it exists
            if (File.Exists(backup))
                File.Delete(backup);

            //If file exists, move to backup
            if (File.Exists(filename))
                File.Move(filename, backup);

            //Write to new file
            File.WriteAllText(filename, JsonConvert.SerializeObject(data));

            //Delete backup
            File.Delete(backup);
        }

        public void Finalize(bool delete)
        {
            //Set flag in file
            data.Delete = delete;

            //Save
            Save();

            //Set finalized flag
            finalized = true;

            //Move file
            File.Move(filename, finalFilename);
        }

        private void EnsureNotFinalized()
        {
            if (finalized)
                throw new Exception("Edit is finalized.");
        }
    }
}
