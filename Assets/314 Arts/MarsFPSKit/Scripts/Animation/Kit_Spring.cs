using System.Collections.Generic;
using UnityEngine;


public class Kit_Spring
{
    [System.Serializable]
    public class SpringForce
    {
        /// <summary>
        /// Force to apply to the spring
        /// </summary>
        public Vector3 forceToApply;

        [Range(0, 50)]
        /// <summary>
        /// Spread the force across frames?
        /// </summary>
        public int spreadAcrossPhysicFrames;

        /// <summary>
        /// Divides the force if it is meant to be spread across multiple frames
        /// </summary>
        public bool equalizeAcrossPhysicFrames = true;
    }

    [System.Serializable]
    public class SpringConfig
    {
        /// <summary>
        /// Stiffness
        /// </summary>
        public Vector3 stiffness = new Vector3(0.1f, 0.1f, 0.1f);
        /// <summary>
        /// Position damping
        /// </summary>
        public Vector3 damping = new Vector3(0.3f, 0.3f, 0.3f);
        /// <summary>
        /// Speed at which we lerp
        /// </summary>
        public float lerpSpeed;
    }

    /// <summary>
    /// How stiff our spring is
    /// </summary>
    private Vector3 stiffness = new Vector3(0.1f, 0.1f, 0.1f);
    /// <summary>
    /// Damping per update
    /// </summary>
    private Vector3 damping = new Vector3(0.3f, 0.3f, 0.3f);

    /// <summary>
    /// Current velocity.
    /// </summary>
    private Vector3 velocity;
    /// <summary>
    /// Current position
    /// </summary>
    private Vector3 position;
    /// <summary>
    /// Speed at which we lerp
    /// </summary>
    private float lerpSpeed;
    /// <summary>
    /// If lerp speed is > 0 then we return this instead of unlerped <see cref="position"/>
    /// </summary>
    private Vector3 lerpedPosition;

    /// <summary>
    /// Forces that are spread across several frames
    /// </summary>
    private Queue<Vector3> spreadForces = new Queue<Vector3>();

    public Kit_Spring(SpringConfig config)
    {
        //Initialize our queue
        spreadForces = new Queue<Vector3>(100);

        stiffness = config.stiffness;
        damping = config.damping;
        lerpSpeed = config.lerpSpeed;
    }

    public Kit_Spring(Vector3 stiffness, Vector3 damping)
    {
        //Initialize our queue
        spreadForces = new Queue<Vector3>(100);

        this.stiffness = stiffness;
        this.damping = damping;
    }

    public Kit_Spring(Vector3 stiffness, Vector3 damping, float lerpSpeed)
    {
        //Initialize our queue
        spreadForces = new Queue<Vector3>(100);

        this.stiffness = stiffness;
        this.damping = damping;
        this.lerpSpeed = lerpSpeed;
    }

    /// <summary>
    /// Resets our spring
    /// </summary>
    public void Reset()
    {
        velocity = Vector3.zero;
    }

    public void AddImmediateForce(Vector3 force)
    {
        //Add force to velocity
        velocity += force;
    }

    public void AddImmediateForce(SpringForce force)
    {
        if (force.spreadAcrossPhysicFrames <= 0)
        {
            //Add force to velocity
            velocity += force.forceToApply;
        }
        else
        {
            Vector3 forceToAdd = force.forceToApply;

            if (force.equalizeAcrossPhysicFrames)
            {
                forceToAdd /= force.spreadAcrossPhysicFrames;
            }

            for (int i = 0; i < force.spreadAcrossPhysicFrames; i++)
            {
                //Enqueue
                spreadForces.Enqueue(forceToAdd);
            }
        }
    }

    /// <summary>
    /// Callback for Update (every frame)
    /// </summary>
    public void Update()
    {
        if (lerpSpeed > 0)
        {
            lerpedPosition = Vector3.Lerp(lerpedPosition, position, Time.deltaTime * lerpSpeed);
        }
    }

    /// <summary>
    /// Callback for fixed update.
    /// </summary>
    public void FixedUpdate()
    {
        //Apply forces that are spread over multiple frames
        if (spreadForces.Count > 0)
        {
            //Apply it and remove from queue
            AddImmediateForce(spreadForces.Dequeue());
        }

        //Apply stiffness
        velocity += Vector3.Scale(-position, stiffness);
        //Apply damping
        velocity = Vector3.Scale(velocity, Vector3.one - damping);

        //Apply velocity
        position += velocity;
    }

    /// <summary>
    /// Returns the current position of the spring
    /// </summary>
    /// <returns></returns>
    public Vector3 GetSpringPosition()
    {
        if (lerpSpeed > 0)
        {
            return lerpedPosition;
        }
        else
        {
            return position;
        }
    }

    /// <summary>
    /// Returns the current position of the spring
    /// </summary>
    /// <returns></returns>
    public Vector3 GetSpringPositionClamped(Vector3 min, Vector3 max)
    {
        Vector3 returnPos = position;

        if (lerpSpeed > 0)
        {
            returnPos = lerpedPosition;
        }

        returnPos.x = Mathf.Clamp(returnPos.x, min.x, max.x);
        returnPos.y = Mathf.Clamp(returnPos.y, min.y, max.y);
        returnPos.z = Mathf.Clamp(returnPos.z, min.z, max.z);

        return returnPos;
    }
}
