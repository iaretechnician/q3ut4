using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// How did we gain these points?
    /// </summary>
    public enum PointType { Kill, Assist }

    /// <summary>
    /// Use this to implement your own PointsUI. It displays points gained from Kills, Captures, Plants etc
    /// </summary>
    public abstract class Kit_PointsUIBase : MonoBehaviour
    {
        /// <summary>
        /// Called when points should be displayed
        /// </summary>
        /// <param name="points"></param>
        /// <param name="type"></param>
        public abstract void DisplayPoints(int points, PointType type);
    }
}
