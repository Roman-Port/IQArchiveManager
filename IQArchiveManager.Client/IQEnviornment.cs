using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client
{
    public class IQEnviornment
    {
        public IQEnviornment(string iqaDir, string dbPath, string editDir, string moveDir)
        {
            IqaDir = iqaDir;
            DbPath = dbPath;
            EditDir = editDir;
            MoveDir = moveDir;
            db = new ClipDatabase(DbPath);
        }

        public string IqaDir { get; set; }
        public string DbPath { get; set; }
        public string EditDir { get; set; }
        public string MoveDir { get; set; }
        public ClipDatabase Db { get => db; }

        private ClipDatabase db;

        public bool GetIqFileById(string id, out string filename)
        {
            filename = IqaDir + id + ".iqa";
            return File.Exists(filename);
        }

        public void FindIqaFiles(List<string> files)
        {
            string[] names = Directory.GetFiles(IqaDir);
            foreach(var n in names)
            {
                if (n.EndsWith(".iqa"))
                    files.Add(n);
            }
        }
    }
}
