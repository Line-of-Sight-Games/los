using UnityEngine;

public static class TransformExtensions
{
    public static Transform FindRecursively(this Transform parent, string name)
    {
        if (parent.name == name)
            return parent;

        foreach (Transform child in parent)
        {
            Transform result = child.FindRecursively(name);
            if (result != null)
                return result;
        }

        return null;
    }
}
