using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    // Start is called before the first frame update
public static Transform FindChildTransformWithTag(GameObject parent, string tag)
{
    Queue<Transform> queue = new Queue<Transform>();
    queue.Enqueue(parent.transform);

    while (queue.Count > 0)
    {
        Transform current = queue.Dequeue();

        if (current.tag == tag)
        {
            return current;
        }

        foreach (Transform child in current)
        {
            queue.Enqueue(child);
        }
    }

    return null;
}
}
