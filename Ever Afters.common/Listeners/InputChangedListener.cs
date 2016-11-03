using System;
using Ever_Afters.common.Enums;

namespace Ever_Afters.common.Listeners
{
    public interface InputChangedListener
    {
        /// <summary>
        /// This function is called when a Tag appears on a sensor.
        /// </summary>
        /// <param name="sensor">The sensor detecting the new tag.</param>
        /// <param name="TagIdentifier">The Identifier of the new tag. (Eg. BASIC-01, MATH-03)</param>
        void OnTagAdded(Sensors sensor, String TagIdentifier);

        /// <summary>
        /// This function is called when a Tag disappears from a sensor.
        /// </summary>
        /// <param name="sensor">The sensor from which a tag has disappeared.</param>
        void OnTagRemoved(Sensors sensor);

        /// <summary>
        /// This function is called when the display video queue needs to be cleared.
        /// This returns a boolean true when the queue has been successfully cleared.
        /// This returns a boolean false when the queue couldn't be cleared. The most probable cause is because the queue was already empty.
        /// </summary>
        /// <param name="force">Setting this parameter on true will immediately stop any output to the screen. Setting this parameter to false will clear the queue but finish the current playing video.</param>
        /// <returns></returns>
        bool OnQueueClearRequest(bool force);
    }
}
