using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;

    [Header("Salto")]
    [SerializeField] private float fuerzaSalto = 6f;
    [SerializeField] private float costoSaltoStamina = 5f;
    [SerializeField] private float distanciaSuelo = 1.1f; // mitad de la altura del capsule + un poquito

    [Header("Vida")]
    [SerializeField] private float vida = 100f;

    [Header("Stamina")]
    [SerializeField] private float stamina = 10f;
    [SerializeField] private float staminaMaxima = 10f;
    [SerializeField] private float staminaPorSegundo = 2f;

    [Header("Camara / FOV")]
    [SerializeField] private Camera camara;
    [SerializeField] private float velocidadFOV = 30f;
    [SerializeField] private float fovMinimo = 50f;
    [SerializeField] private float fovMaximo = 120f;

    [Header("Pistola")]
    [SerializeField] private float rango = 20f;
    [SerializeField] private float danio = 25f;
    [SerializeField] private float cadencia = 1.5f; // segundos entre disparo y disparo
    [SerializeField] private int balas = 10;

    private Rigidbody rb;
    private float tiempoProximoDisparo = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Si esta muerto no hace nada (no se mueve ni ataca)
        if (vida <= 0)
        {
            return;
        }

        Mover();
        Saltar();
        RegenerarStamina();
        CambiarFOV();
        Disparar();
    }

    void Mover()
    {
        Vector3 direccion = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direccion += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direccion -= transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direccion -= transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direccion += transform.right;
        }

        transform.position += direccion.normalized * speed * Time.deltaTime;
    }

    bool EstaEnElSuelo()
    {
        return Physics.Raycast(transform.position, Vector3.down, distanciaSuelo);
    }

    void Saltar()
    {
        if (Input.GetKeyDown(KeyCode.Space) && EstaEnElSuelo() && stamina >= costoSaltoStamina)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            stamina -= costoSaltoStamina;
            stamina = Mathf.Clamp(stamina, 0f, staminaMaxima);
        }
    }

    void RegenerarStamina()
    {
        stamina += staminaPorSegundo * Time.deltaTime;
        stamina = Mathf.Clamp(stamina, 0f, staminaMaxima); // nunca menor a 0 ni mayor a 10
    }

    void CambiarFOV()
    {
        if (Input.GetKey(KeyCode.T))
        {
            camara.fieldOfView -= velocidadFOV * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Y))
        {
            camara.fieldOfView += velocidadFOV * Time.deltaTime;
        }

        camara.fieldOfView = Mathf.Clamp(camara.fieldOfView, fovMinimo, fovMaximo);
    }

    void Disparar()
    {
        if (Input.GetMouseButtonDown(0) && balas > 0 && Time.time >= tiempoProximoDisparo)
        {
            balas--;
            tiempoProximoDisparo = Time.time + cadencia;

            // Rayo que sale del centro de la pantalla
            Ray rayo = camara.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;

            if (Physics.Raycast(rayo, out hit, rango))
            {
                Enemigo enemigo = hit.collider.GetComponent<Enemigo>();

                if (enemigo != null)
                {
                    enemigo.RecibirDanio(danio);
                }
            }

            Debug.Log("Balas restantes: " + balas);
        }
    }

    // Para que los enemigos o trampas puedan lastimar al jugador
    public void RecibirDanio(float cantidad)
    {
        vida -= cantidad;

        if (vida <= 0)
        {
            vida = 0;
            Debug.Log("El jugador murio");
        }
    }

    // Lo usa el item Municion para sumar balas
    public void AgregarBalas(int cantidad)
    {
        balas += cantidad;
        Debug.Log("Balas: " + balas);
    }
}