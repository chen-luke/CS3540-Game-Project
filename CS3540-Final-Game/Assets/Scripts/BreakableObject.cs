using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public float explosionForce = 100;
    public float explosionRadius = 10;

    // minimum amount of damage required in one hit to destroy object
    public float damageRequired = 10;
    public GameObject objectPieces;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            if (0 >= damageRequired)
            {
                // will eventually check to see if enough damage was done
                Transform currentObj = gameObject.transform;

                // create pieces and send them exploding
                GameObject pieces = Instantiate(objectPieces, currentObj.position, currentObj.rotation);
                Rigidbody[] rbPieces = pieces.GetComponentsInChildren<Rigidbody>();

                foreach (Rigidbody rb in rbPieces)
                {
                    rb.AddExplosionForce(explosionForce, currentObj.position, explosionRadius);
                }

                // get rid of original object and then pieces after a delay
                Destroy(gameObject);
                Destroy(pieces, 2f);
            }

        }

    }
}
