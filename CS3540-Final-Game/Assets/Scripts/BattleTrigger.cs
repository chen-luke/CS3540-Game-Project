using UnityEngine;

// The behavior for the collider that triggers the waking of the boss
public class BattleTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            LevelManager.isBossAwake = true;
        }
    }
}
