using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    float axisH;
    float axisV;
    float cameraPitchAngle = 0f;

    [SerializeField]
    Transform camera;

    [SerializeField]
    float speed = 2f;

    [SerializeField]
    float sensibility = 10f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        axisH = Input.GetAxis("Horizontal");
        axisV = Input.GetAxis("Vertical");

        float mouseX = Input.GetAxis("Mouse X") * sensibility;
        float mouseY = Input.GetAxis("Mouse Y") * sensibility;

        // rotacion de la capsula en el eje Y (yaw)
        Quaternion newRot = transform.rotation * Quaternion.Euler(0f, mouseX, 0f);
        rb.MoveRotation(newRot);

        // rotacion de la camara en el eje X (pitch)
        cameraPitchAngle = Mathf.Clamp(cameraPitchAngle - mouseY, -75f, 75f);
        camera.localRotation = Quaternion.Euler(cameraPitchAngle, 0f, 0f);

    }

    void FixedUpdate()
    {
        Vector3 newPos = transform.position + ((transform.forward * axisV) + (transform.right * axisH)) * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }
}
