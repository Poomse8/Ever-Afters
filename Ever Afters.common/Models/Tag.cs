using Ever_Afters.common.DatabaseLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ever_Afters.common.Models
{
    public class Tag
    {
        //General Database Fields
        public int id { get; set; }
        public String name { get; set; }
        public int videoid { get; set; }

        /// <summary>
        /// This is an empty constructor meant for data binding at boot time.
        /// In code this constructor should be avoided.
        /// </summary>
        public Tag()
        {

        }

        /// <summary>
        /// This object represents the NFC tag in database.
        /// This takes only a name, ideal for the creation of a new tag.
        /// </summary>
        /// <param name="TagName">The TAG name written on the NFC label</param>
        public Tag(String TagName)
        {
            this.name = TagName;
        }

        /// <summary>
        /// This object represents the NFC tag in database
        /// </summary>
        /// <param name="id">The Database Identifier</param>
        /// <param name="TagName">The TAG name written on the NFC label</param>
        public Tag(int id, String TagName)
        {
            this.id = id;
            this.name = TagName;
        }
        
        //Static Functions
        public static bool tagExists(String TagName)
        {
            //Check if a tag exists with given name.
            return SQLiteService.CheckTagExist(TagName);
        }

        public static bool isBound(String TagName)
        {
            //Check if a tag with given name is bound to a video.
            SQLiteService sql = new SQLiteService();
            List<Tag> tags = sql.GetUnboundTags().ToList<Tag>();
            Tag tag = sql.LoadTagByName(TagName);

            if ((from t in tags where tag.id == t.id select t).Any())
                return false; //if the id is found in the unbound list -> tag isn't bound
            else
                return true;
        }

        //Overrides
        public override string ToString()
        {
            return this.name;
        }
    }
}
