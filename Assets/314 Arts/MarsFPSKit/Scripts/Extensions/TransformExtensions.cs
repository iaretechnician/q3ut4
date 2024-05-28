using UnityEngine;

public static class TransformExtensions
{
    /// <summary>
    /// Sets the layer of the given transform recursively
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="layer"></param>
    public static void SetLayerRecursively(this Transform trans, int layer)
    {
        trans.gameObject.layer = layer;
        foreach (Transform child in trans)
            child.SetLayerRecursively(layer);
    }
}