using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public float explosionForce = 100;
    public float explosionRadius = 10;
    public GameObject objectPieces;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SwordSlashProjectile"))
        {
            Transform currentObj = gameObject.transform;

            // create pieces and send them exploding
            GameObject pieces = Instantiate(objectPieces, currentObj.position, currentObj.rotation);
            pieces.transform.localScale = new Vector3(pieces.transform.localScale.x * currentObj.localScale.x, pieces.transform.localScale.y * currentObj.localScale.y, pieces.transform.localScale.z * currentObj.localScale.z);
            Rigidbody[] rbPieces = pieces.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rb in rbPieces)
            {
                rb.AddExplosionForce(explosionForce, currentObj.position, explosionRadius);
            }

            // fade out pieces so they don't just suddenly disappear
            FadeOutChildren[] fades = pieces.GetComponentsInChildren<FadeOutChildren>();
            foreach (FadeOutChildren fade in fades)
            {
                fade.Invoke("StartFade", .5f);
            }

            // get rid of original object and then pieces after a delay
            Destroy(gameObject);
            Destroy(pieces, 1.5f);
        }

    }
}
