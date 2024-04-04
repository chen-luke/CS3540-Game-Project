using UnityEngine;

// behavior to destroy gameobject after given time
public class DestroyObject : MonoBehaviour
{
    public float destroyTime = 3f;
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
