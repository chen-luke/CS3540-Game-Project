
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    // <<<<LEGACY>>>>
    Transform playerBody;
    public float pitchDeg = 30f;
    public float camDistance = 10f;
    public float mouseSensitivity = 300f;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = transform.parent.transform;
        PositionCameraAtStart();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(!PauseMenuBehavior.isGamePaused) {
            float moveX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        // float moveY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Camera is always positioned directly behind the player, but this means
        // there is no freeform rotation of the camera, even when the player is not moving
        playerBody.Rotate(Vector3.up * moveX);

        // Will add vertical rotation around the playerBody.position in the future
        }
        
    }

    private void PositionCameraAtStart()
    {
        float pitchRad = pitchDeg * Mathf.Deg2Rad;
        transform.localRotation = Quaternion.Euler(pitchDeg, 0, 0);
        transform.localPosition = new Vector3(0, Mathf.Sin(pitchRad) * camDistance, -1 * Mathf.Cos(pitchRad) * camDistance);
    }
}
