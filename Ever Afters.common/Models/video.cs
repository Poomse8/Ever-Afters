using Ever_Afters.common.DatabaseLayer;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ever_Afters.common.Models
{
    public class Video
    {
        //General Database Fields
        /// <summary>
        /// The database identifier
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// Boolean value whether or not the base video starts with the figure on screen
        /// </summary>
        [Column("basestart")]
        public bool BaseStartsOnScreen { get; set; }
        /// <summary>
        /// The path to the base of the movie. Always the first few seconds
        /// </summary>
        [Column("basepath")]
        public String BasePath { get; set; }
        /// <summary>
        /// The path to the ending where the action ends in the middle of the screen
        /// </summary>
        [Column("onscreen_ending")]
        public String OnScreenEndingPath { get; set; }
        /// <summary>
        /// The path to the ending where the action ends off the screen.
        /// </summary>
        [Column("offscreen_ending")]
        public String OffScreenEndingPath { get; set; }

        /// <summary>
        /// The TAG id which resolved this video
        /// </summary>
        public String Request_TAG { get; set; }

        /// <summary>
        /// ALL Tags that are a match for this video. (a video might be reused for multiple tags?)
        /// </summary>
        public IEnumerable<String> TAGS { get; set; }

        /// <summary>
        /// This resembles the VIDEO object used to map stories to tags.
        /// This is an empty video object. It is not adviced that this is used in code.
        /// The empty constructor is for data binding on boot time. Other constructors should be used in code.
        /// </summary>
        public Video()
        {

        }

        /// <summary>
        /// This resembles the VIDEO object used to map stories to tags
        /// </summary>
        /// <param name="id">The database identifier</param>
        /// <param name="startonscreen">Boolean value whether or not the base video starts with the figure on screen</param>
        /// <param name="basepath">The path to the base of the movie. Always the first few seconds</param>
        /// <param name="onscreenending">The path to the ending where the action ends in the middle of the screen</param>
        /// <param name="offscreenending">The path to the ending where the action ends off the screen.</param>
        public Video(int id, bool startonscreen, String basepath, String onscreenending, String offscreenending)
        {
            this.id = id;
            this.BaseStartsOnScreen = startonscreen;
            this.BasePath = basepath;
            this.OnScreenEndingPath = onscreenending;
            this.OffScreenEndingPath = offscreenending;
        }

        /// <summary>
        /// This resembles the VIDEO object used to map stories to tags - Extended
        /// </summary>
        /// <param name="id">The database identifier</param>
        /// <param name="startonscreen">Boolean value whether or not the base video starts with the figure on screen</param>
        /// <param name="basepath">The path to the base of the movie. Always the first few seconds</param>
        /// <param name="onscreenending">The path to the ending where the action ends in the middle of the screen</param>
        /// <param name="offscreenending">The path to the ending where the action ends off the screen.</param>
        /// <param name="requesttag">The TAG that has been used to look up this video in the database</param>
        public Video(int id, bool startonscreen, String basepath, String onscreenending, String offscreenending, String requesttag)
        {
            this.id = id;
            this.BaseStartsOnScreen = startonscreen;
            this.BasePath = basepath;
            this.OnScreenEndingPath = onscreenending;
            this.OffScreenEndingPath = offscreenending;
            this.Request_TAG = requesttag;
        }

        //General purpose methods
        public static bool pathExists(String path)
        {
            //Check if a given path resolves to a video.
           return SQLiteService.CheckPathExist(path);
           
        }

        public static bool isBound(String path)
        {
            //Check if a video with given path is bound to any tag.
        
            return SQLiteService.IsBoundPathToTag(path);
        }

        //Override
        public override string ToString()
        {
            //Give the ID and the basic video path
            return this.BasePath + "(" + this.id.ToString() + ")"; 
        }
    }
}
