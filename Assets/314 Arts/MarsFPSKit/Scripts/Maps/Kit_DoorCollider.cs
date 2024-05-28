using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This script checks if something is in the way of the door!
    /// </summary>
    public class Kit_DoorCollider : MonoBehaviour
    {
        public List<Collider> collidersInWay = new List<Collider>();

        public bool somethingInWay
        {
            get
            {
                return collidersInWay.Count > 0;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            //Clear
            collidersInWay = collidersInWay.Where(item => item != null).ToList();
            //Check if its in list
            if (!collidersInWay.Contains(other))
            {
                //Check if it has a rigidbody or is player
                if (other.transform.root.GetComponent<Kit_PlayerBehaviour>() || other.GetComponentInParent<Rigidbody>() && !other.GetComponentInParent<Rigidbody>().isKinematic || other.GetComponentInChildren<Rigidbody>() && !other.GetComponentInChildren<Rigidbody>().isKinematic)
                    //Add
                    collidersInWay.Add(other);
            }
        }

        void OnTriggerExit(Collider other)
        {
            //Clear
            collidersInWay = collidersInWay.Where(item => item != null).ToList();
            //Check if its in list
            if (collidersInWay.Contains(other))
            {
                collidersInWay.Remove(other);
            }
        }
    }
}