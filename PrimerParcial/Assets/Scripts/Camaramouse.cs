using UnityEngine;

public class CamaraMouse : MonoBehaviour
{
    [SerializeField] private Transform camara;       // la camara hija del jugador
    [SerializeField] private float sensibilidad = 2f;

    private float rotacionX = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidad;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidad;

        // Gira el cuerpo a los costados
        transform.Rotate(Vector3.up * mouseX);

        // Gira la camara arriba/abajo (limitado para no dar la vuelta)
        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);
        camara.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
    }
}