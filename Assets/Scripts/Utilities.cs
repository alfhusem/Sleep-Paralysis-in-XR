using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    private static Dictionary<GameObject, Bounds?> prefabBoundsCache = new();

    public static readonly float Sqrt2 = Mathf.Sqrt(2f);
    public static readonly float InvSqrt2 = 1f / Mathf.Sqrt(2f);

    public static Bounds? GetPrefabBounds(GameObject prefab)
    {
        if (prefabBoundsCache.TryGetValue(prefab, out Bounds? cachedBounds))
        {
            return cachedBounds;
        }

        Bounds? bounds = CalculateBoundsRecursively(prefab.transform);
        prefabBoundsCache.Add(prefab, bounds);
        return bounds;
    }

    private static Bounds? CalculateBoundsRecursively(Transform transform)
    {
        Bounds? bounds = null;
        Renderer renderer = transform.GetComponent<Renderer>();

        if (renderer != null && renderer.bounds.size != Vector3.zero)
        {
            bounds = renderer.bounds;
        }

        foreach (Transform child in transform)
        {
            Bounds? childBounds = CalculateBoundsRecursively(child);
            if (childBounds != null)
            {
                if (bounds != null)
                {
                    var boundsValue = bounds.Value;
                    boundsValue.Encapsulate(childBounds.Value);
                    bounds = boundsValue;
                }
                else
                {
                    bounds = childBounds;
                }
            }
        }

        return bounds;
    }
}

