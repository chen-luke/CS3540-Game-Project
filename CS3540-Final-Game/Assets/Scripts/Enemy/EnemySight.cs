using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform enemyEyes;
    public float lookDistance = 9f;

    public float fieldOfView = 45f;
    void Start()
    {
        
    }

    public bool SeePlayer() {
        RaycastHit hit;
        Vector3 directionToPlayer = gameObject.transform.position - enemyEyes.position;
        if (Vector3.Angle(directionToPlayer, enemyEyes.forward) <= fieldOfView) {
            if(Physics.Raycast(enemyEyes.position, directionToPlayer, out hit, lookDistance)) {
                if (hit.collider.CompareTag("Player")) {
                    return true;
                }
                return false;
            }
            return false;
        }
        return false;
    }
    private void OnDrawGizmos() {

        Vector3 frontRayPoint = enemyEyes.position + (enemyEyes.forward * lookDistance);
        //Vector3 leftRayPoint = Quaternion.Euler(0, fieldOfView * 0.5f, 0) * frontRayPoint;

        //Vector3 rightRayPoint = Quaternion.Euler(0, -fieldOfView * 0.5f, 0) * frontRayPoint;

        Debug.DrawLine(enemyEyes.position, frontRayPoint, Color.red);
        //Debug.DrawLine(enemyEyes.position, leftRayPoint, Color.yellow);
        //Debug.DrawLine(enemyEyes.position, rightRayPoint, Color.yellow);

    }
}
