using UnityEngine;

// behavior for an enemy health bar
public class EnemyHealthBarBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject mainCamera;
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(mainCamera.transform); // make the health bar face the player
    }
}
