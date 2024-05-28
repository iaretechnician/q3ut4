using System.Collections;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This represents a single marker
    /// </summary>
    public class Kit_MinimapMarker : MonoBehaviour
    {
        public enum MarkerMode { Friendly, Enemy }
        /// <summary>
        /// The Sprite Renderer of this marker
        /// </summary>
        public SpriteRenderer sr;
        /// <summary>
        /// If we shoot and are visible on enemy radar, how long?
        /// </summary>
        public float visibleLength = 2f;

        #region Runtime
        /// <summary>
        /// The player that belongs to this marker
        /// </summary>
        private Kit_PlayerBehaviour myPlayer;
        /// <summary>
        /// Current alpha
        /// </summary>
        private float alpha = 1f;
        /// <summary>
        /// Current color
        /// </summary>
        private Color col;
        /// <summary>
        /// Is the visible coroutine running
        /// </summary>
        private bool isRoutineRunning;
        /// <summary>
        /// The coroutine if it is running
        /// </summary>
        private Coroutine routine;
        /// <summary>
        /// Current world position
        /// </summary>
        private Vector3 worldPos;
        #endregion

        public void Setup(MarkerMode mode, Kit_PlayerBehaviour pb)
        {
            //Assign player
            myPlayer = pb;
            //Unparent if enemy mode
            if (mode == MarkerMode.Enemy)
            {
                transform.parent = null;
                //Set alpha to invisible too
                alpha = 0f;
                //Disable
                sr.enabled = false;
            }
            //Get color
            col = sr.color;
            //Apply alpha
            col.a = alpha;
            //Set color
            sr.color = col;
        }

        void Update()
        {
            //Get world pos
            worldPos = transform.position;
            //Set y
            worldPos.y = 35f;
            //Set back
            transform.position = worldPos;
        }

        /// <summary>
        /// Called when this represents an enemy player and he shot unsilenced
        /// </summary>
        public void EnemyShot()
        {
            if (isRoutineRunning)
            {
                StopCoroutine(routine);
                isRoutineRunning = false;
            }
            //Move
            transform.position = myPlayer.transform.position;
            //Start ShotRoutine
            routine = StartCoroutine(ShotRoutine());
        }

        IEnumerator ShotRoutine()
        {
            isRoutineRunning = true;
            //Enable
            sr.enabled = true;
            //Visible + fade
            alpha = visibleLength + 1f;
            //Fade
            while (alpha > 0f)
            {
                //Decrease
                alpha -= Time.deltaTime;
                //Set color
                col.a = alpha;
                sr.color = col;
                yield return null;
            }
            //Disable
            sr.enabled = false;
            isRoutineRunning = false;
        }
    }
}
