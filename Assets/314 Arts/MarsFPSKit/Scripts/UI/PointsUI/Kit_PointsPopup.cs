using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    public class Kit_PointsPopup : Kit_PointsUIBase
    {
        /// <summary>
        /// How does the <see cref="fontAnimation"/> size change?
        /// </summary>
        public AnimationCurve fontSizeCurve;
        /// <summary>
        /// How long is the curve?
        /// </summary>
        public float fontSizeCurveLength;
        /// <summary>
        /// Our current state regarding the size
        /// </summary>
        public float fontSizeDelta = 1f;
        /// <summary>
        /// The text which to animate
        /// </summary>
        public TextMeshProUGUI fontAnimation;
        /// <summary>
        /// How much time do we have to stack points?
        /// </summary>
        public float timeToStackPoints = 2f;
        /// <summary>
        /// When were points added for the last time?
        /// </summary>
        private float lastPointAdd;

        public int currentPoints;

        public override void DisplayPoints(int points, PointType type)
        {
            //Set size
            fontSizeDelta = 0f;

            //Check if we can still stack ppoints
            if (lastPointAdd + timeToStackPoints > Time.time)
            {
                //Add
                lastPointAdd = Time.time;
                currentPoints += points;
            }
            else
            {
                //We can't, set points
                lastPointAdd = Time.time;
                currentPoints = points;
            }

            //Set text
            fontAnimation.text = "+" + currentPoints.ToString();
        }

        void Update()
        {
            fontAnimation.fontSize = Mathf.RoundToInt(fontSizeCurve.Evaluate(fontSizeDelta));

            if (fontSizeDelta < 1f)
            {
                fontSizeDelta += Time.deltaTime / fontSizeCurveLength;
                fontSizeDelta = Mathf.Clamp(fontSizeDelta, 0, 1f);
            }

            if (lastPointAdd + timeToStackPoints > Time.time)
            {
                fontAnimation.enabled = true;
            }
            else
            {
                fontAnimation.enabled = false;
            }
        }
    }
}
