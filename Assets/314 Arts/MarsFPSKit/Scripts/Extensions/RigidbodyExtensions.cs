using UnityEngine;

public static class RigidbodyExtensions
{
    public static void AddForceNextFrame(this Rigidbody body, Vector3 force)
    {
        body.gameObject.AddComponent<RigidbodyCoroutineHelper>().StartCoroutine("AddForceNextFrame", force);
    }
}