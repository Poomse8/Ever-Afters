using Ever_Afters.common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ever_Afters.common.Listeners
{
    public interface DataRequestHandler
    {
        //CREATE
        /// <summary>
        /// This method will save a new Tag to the database and will return an object with the saved ID.
        /// </summary>
        /// <param name="TagName">The name of the tag</param>
        /// <returns></returns>
        Tag SaveTag(String TagName);

        /// <summary>
        /// This method will save a new Video to the database and will return an object with the saved ID.
        /// </summary>
        /// <param name="BaseStartsOnScreen">Boolean value whether or not the base video starts with the figure on screen</param>
        /// <param name="BasePath">The path to the base of the movie. Always the first few seconds</param>
        /// <param name="OnScreenEndingPath">The path to the ending where the action ends in the middle of the screen</param>
        /// <param name="OffScreenEndingPath">The path to the ending where the action ends off the screen.</param>
        /// <returns></returns>
        Video SaveVideo(bool BaseStartsOnScreen, String BasePath, String OnScreenEndingPath, String OffScreenEndingPath);

        /// <summary>
        /// This function will bind a video to a tag. (A tag can only have one video bound to it.)
        /// </summary>
        /// <param name="video">The video object that will be bound.</param>
        /// <param name="tag">The tag that will be bound to.</param>
        /// <returns></returns>
        bool BindVideoToTag(Video video, Tag tag);

        //READ
        /// <summary>
        /// This function returns the tag based on its name
        /// </summary>
        /// <param name="TagIdentifier">The name read on the tag.</param>
        /// <returns></returns>
        Tag LoadTagByName(String TagIdentifier);
        /// <summary>
        /// This function returns the video that is bound to a given tag.
        /// </summary>
        /// <param name="tag">The tag for which the bound video has to be fetched.</param>
        /// <returns></returns>
        Video LoadVideoFromTag(Tag tag);
        /// <summary>
        /// This funcion gives all the tags that are currently unbound.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Tag> GetUnboundTags();

        //UPDATE - ID of the object given matches the ID of the object that has to be changed.
        Tag UpdateTag(Tag UpdatedTag);
        Video UpdateVideo(Video UpdatedVideo); //Only the BaseStartsOnScreen, BasePath, OnScreenEndingPath, OffScreenEndingPath

        //DELETE
        /// <summary>
        /// This method will delete a given tag and returns a success status
        /// </summary>
        /// <param name="TagId">The identifier of the tag that has to be deleted</param>
        /// <returns></returns>
        bool DeleteTag(int TagId);
        /// <summary>
        /// This method will delete a given video and returns a success status
        /// </summary>
        /// <param name="VideoId">The identifier of the tag that has to be deleted</param>
        /// <returns></returns>
        bool DeleteVideo(int VideoId);
        /// <summary>
        /// This method will unbound a video from a tag and returns a success status
        /// </summary>
        /// <param name="tag">The tag that needs to be unbound from its video</param>
        /// <returns></returns>
        bool DeleteBinding(Tag tag);
    }
}
