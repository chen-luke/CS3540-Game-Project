using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public float explosionForce = 100;
    public float explosionRadius = 10;
    public float damangeRequired = 10;

    public GameObject objectPieces;

    void Start()
    {
        

    }


    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            Transform currentObj = gameObject.transform;
            GameObject pieces = Instantiate(objectPieces, currentObj.position, currentObj.rotation);
            pieces.transform.localScale = Vector3.one;

            Rigidbody[] rbPieces = pieces.GetComponentsInChildren<Rigidbody>();


            foreach (Rigidbody rb in rbPieces)
            {
                rb.AddExplosionForce(explosionForce, currentObj.position, explosionRadius);

            }
            Destroy(gameObject);

            Destroy(pieces, 2f);
        }

    }
}
