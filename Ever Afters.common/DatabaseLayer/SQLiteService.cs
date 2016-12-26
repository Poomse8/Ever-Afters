using Ever_Afters.common.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ever_Afters.common.Models;
using System.IO;
using Windows.Storage;
using SQLite.Net;
using Ever_Afters.common.Core;

namespace Ever_Afters.common.DatabaseLayer
{
    public class SQLiteService : DataRequestHandler
    {
        private static string path = Path.Combine(ApplicationData.Current.GetPublisherCacheFolder("EverAfters").Path, "EverAfters.sqlite");
        private static string MovieTabelNaam = "movie";
        private static string TagTabelNaam = "tag";

        #region Singleton

        private static SQLiteService _sqLite;

        public static SQLiteService CurrentInstance => _sqLite ?? (_sqLite = new SQLiteService());

        #endregion

        #region create tables
        public static void InitSQLite()
        {
            using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                CreateMovieTB(conn);
                CreateTagTB(conn);
            }
        }
        private static void CreateMovieTB(SQLiteConnection conn)
        {
            string create = "create table if not exists "+MovieTabelNaam+" ("
                 + "id integer primary key"
                 + ", basestart integer not null"
                 + ", basepath text not null"
                 + ", onscreen_ending text not null"
                 + ", offscreen_ending text not null"
                 + ")";
            SQLiteCommand cc = conn.CreateCommand(create);
            cc.ExecuteNonQuery();
        }
        private static void CreateTagTB(SQLiteConnection conn)
        {
            string create = "create table if not exists "+TagTabelNaam+" ("
                 + "id integer primary key"
                 + ", name text not null"
                 + ", videoid integer"
                 + ")";
            SQLiteCommand cc = conn.CreateCommand(create);
            cc.ExecuteNonQuery();
        }
        #endregion

        /*
         * These are needed when you want to develop the mobile app
         * For now these are not necessary
        private static void CreateChildTB(SQLiteConnection conn)
        {
            string createtrk = "create table if not exists child ("
                 + "id integer primary key"
                 + ", firstname text not null"
                 + ", lastname text not null"
                 + ", startscore integer not null"
                 + ", currentscore integer not null"
                 +", parentid integer not null"
                 + ")";
            SQLiteCommand cc = conn.CreateCommand(createtrk);
            cc.ExecuteNonQuery();
        }
        private static void CreateParentTB(SQLiteConnection conn)
        {
            string createtrk = "create table if not exists parent ("
                 + "id integer primary key"
                 + ", firstname text not null"
                 + ", lastname text not null"
                 + ")";
            SQLiteCommand cc = conn.CreateCommand(createtrk);
            cc.ExecuteNonQuery();
        }*/

        #region Static functions
        private static int ExecuteInsert(string sql, params object[] args)
        {
            using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                conn.Execute(sql, args);
                int i = conn.ExecuteScalar<int>("select last_insert_rowid()");           
                return i;
            }
        }
        private static int ExecuteUpdate(string sql, params object[] args)
        {
            using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                int i = conn.Execute(sql,args);
                return i;
            }
        }
        private static int ExecuteDelete(string sql)
        {
            using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                SQLiteCommand cc = conn.CreateCommand(sql);
                 return cc.ExecuteNonQuery();
            }
        }
        private static List<Tag> SelectTags(string sql, params object[] args)
        {
            using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                var res = conn.Query<Tag>(sql, args);
                return res;
            }
        }
        public static bool CheckTagExist(string tagname)
        {
            List<Tag> t = new List<Tag>();
            using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                string command = "SELECT id, name, videoid FROM " + TagTabelNaam + " WHERE name=?";
                t = conn.Query<Tag>(command,tagname);

            }
            if (t.Count()!=0)
                return true;
            else
                return false;
        }
        public static bool CheckPathExist(string pathname)
        {
            List<Video> vid;
            using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                string sql = "SELECT id FROM " + MovieTabelNaam + " WHERE basepath=?" ;
               
                vid = conn.Query<Video>(sql,pathname);
            }
            if (vid.Count() !=0)
                return true;
            else
                return false;
        }
        public static bool IsBoundPathToTag(string pathname)
        {
            int? vid;
            using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                string sql = "SELECT m.id FROM " + MovieTabelNaam + " as m,"+TagTabelNaam+" as t  WHERE basepath=? AND t.videoid = m.id";
                SQLiteCommand comm = conn.CreateCommand(sql, pathname);
                vid = comm.ExecuteScalar<int>();
            }
            if (vid != null)
                return true;
            else
                return false;
        }
        #endregion

        #region interface implementations
        public bool BindVideoToTag(Video video, Tag tag)
        {
            string command = "UPDATE "+TagTabelNaam+" SET videoid = ? WHERE id="+tag.id;
            object[] args = new object[] { video.id};
            return ExecuteUpdate(command, args) > 0;
        }

        public bool DeleteBinding(Tag tag)
        {
            string command = "UPDATE " + TagTabelNaam + " SET videoid = ? WHERE id=" + tag.id;
            object[] args = new object[] { null };
            return ExecuteUpdate(command, args) > 0;
        }

        public bool DeleteTag(int TagId)
        {
            string command = "DELETE FROM "+TagTabelNaam+" WHERE id=" + TagId;
            return ExecuteUpdate(command) == 1;
        }

        public bool DeleteVideo(int VideoId)
        {
            string command = "DELETE FROM " + MovieTabelNaam + " WHERE id=" + VideoId;
            return ExecuteUpdate(command) == 1;
        }

        public IEnumerable<Tag> GetUnboundTags()
        {
            string command = "SELECT id, name, videoid FROM " + TagTabelNaam + " WHERE videoid IS NULL";
            object[] args = new object[] { null };
            IEnumerable<Tag> tags = SelectTags(command,args);
            return tags;
        }

        public IEnumerable<Tag> GetAllTags()
        {
            string command = "SELECT id, name, videoid FROM " + TagTabelNaam;
            object[] args = new object[] { null };
            IEnumerable<Tag> tags = SelectTags(command, args);
            return tags;
        }

        public IEnumerable<Video> GetAllVideos()
        {
            using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                string command = "SELECT id,basestart,basepath,onscreen_ending,offscreen_ending FROM " + MovieTabelNaam;
                object[] args = new object[] { null };
                IEnumerable<Video> videos = conn.Query<Video>(command, args);
                List<Tag> tags = GetAllTags().ToList<Tag>();
                foreach(Video v in videos)
                {
                    List<string> tag = new List<string>();
                    foreach (Tag t in tags)
                    {
                       
                        if (t.videoid == v.id)
                        {
                            tag.Add(t.name);
                        }
                    }
                    if (tag.Count != 0)
                    {
                        v.TAGS = tag;
                    }
                   
                }
            
                return videos;
            }
        }

        public Tag LoadTagByName(string TagIdentifier)
        {
            string command = "SELECT id, name, videoid FROM " + TagTabelNaam + " WHERE name=?";
            object[] args = new object[] { TagIdentifier };
            List<Tag> tags = SelectTags(command, args);
            return tags[0];
        }

        public Video LoadVideoFromTag(Tag tag)
        {
            Video result = new Video();           
           
            using (SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                string sqlgetvidID = "SELECT v.id FROM " + MovieTabelNaam + " as v, " + TagTabelNaam + " as t WHERE t.id = ? AND t.videoid=v.id";
                string command = "SELECT id, basepath,onscreen_ending,offscreen_ending FROM " + MovieTabelNaam+" WHERE id = ?";
                string getBasestart = "SELECT basestart FROM " + MovieTabelNaam + " WHERE id = ?";
                SQLiteCommand comm = conn.CreateCommand(sqlgetvidID,tag.id);
                int vidID = comm.ExecuteScalar<int>();
                
                result = conn.Query<Video>(command,vidID)[0];
                comm = conn.CreateCommand(getBasestart, vidID);
                int basestart = comm.ExecuteScalar<int>();
                if (basestart == 1)
                    result.BaseStartsOnScreen = true;
                else
                    result.BaseStartsOnScreen = false;
                
            }
            result.Request_TAG = tag.name;
            return result;
        }

        public Tag SaveTag(string TagName)
        {
            Tag t = new Tag();
            t.name = TagName;
            string ins = $"insert into " + TagTabelNaam + " values(?,?,?)";
            object[] args = new object[] { null, TagName, null };
            t.id = ExecuteInsert(ins, args);
            return t;
         
        }

        public Video SaveVideo(bool BaseStartsOnScreen, string BasePath, string OnScreenEndingPath, string OffScreenEndingPath)
        {
           
            Video vid = new Video();
            vid.OnScreenEndingPath = OnScreenEndingPath;
            vid.OffScreenEndingPath = OffScreenEndingPath;
            vid.BasePath = BasePath;
            vid.BaseStartsOnScreen = BaseStartsOnScreen;

            string ins = $"insert into "+MovieTabelNaam+" values(?,?,?,?,?)";
            int basestart =0;
            if (BaseStartsOnScreen)
                basestart = 1;
            object[] args = new object[] {null,basestart,BasePath,OnScreenEndingPath,OffScreenEndingPath };
            vid.id = ExecuteInsert(ins, args);
            return vid;
           
        }

        public Tag UpdateTag(Tag UpdatedTag)
        {
            string command = "UPDATE " + TagTabelNaam + " SET name = ? WHERE id=" + UpdatedTag.id;
            object[] args = new object[] { UpdatedTag.name };
            ExecuteUpdate(command, args);
            return UpdatedTag;
        }

        public Video UpdateVideo(Video UpdatedVideo)
        {
            string command = "UPDATE " + MovieTabelNaam + " SET basepath = ?, onscreen_ending=?, offscreen_ending=?, basestart =? WHERE id=" + UpdatedVideo.id;
            int basestart = 0;
            if (UpdatedVideo.BaseStartsOnScreen)
                basestart = 1;
            object[] args = new object[] {UpdatedVideo.BasePath, UpdatedVideo.OnScreenEndingPath,UpdatedVideo.OffScreenEndingPath,basestart};
            ExecuteUpdate(command, args);
            return UpdatedVideo;
        }
        #endregion


    }
}
