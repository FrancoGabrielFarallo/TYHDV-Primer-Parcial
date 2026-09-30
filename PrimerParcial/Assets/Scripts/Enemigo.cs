using UnityEngine;

public class Enemigo : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float distanciaPersecucion = 5f;  // a que distancia empieza a perseguir
    [SerializeField] private float distanciaMinima = 1.5f;     // se frena cerca del jugador para no empujarlo

    [Header("Vida")]
    [SerializeField] private float vida = 100f;

    [Header("Ataque (pistola)")]
    [SerializeField] private float rango = 5f;
    [SerializeField] private float danio = 20f;
    [SerializeField] private float cadencia = 2f; // segundos entre disparo y disparo (balas ilimitadas)

    [Header("Objetivo")]
    [SerializeField] private Transform jugador;

    private float tiempoProximoDisparo = 0f;

    void Start()
    {
        // Si no arrastraste al jugador en el editor, lo busca solo
        if (jugador == null)
        {
            Player scriptJugador = FindFirstObjectByType<Player>();

            if (scriptJugador != null)
            {
                jugador = scriptJugador.transform;
            }
        }
    }

    void Update()
    {
        // Si esta muerto no se mueve ni ataca (pero sigue en la escena)
        if (vida <= 0 || jugador == null)
        {
            return;
        }

        float distancia = Vector3.Distance(transform.position, jugador.position);

        // Persigue solo si el jugador esta dentro de la distancia
        if (distancia <= distanciaPersecucion)
        {
            Perseguir(distancia);
        }

        Atacar(distancia);
    }

    void Perseguir(float distancia)
    {
        // Mira al jugador (sin inclinarse hacia arriba o abajo)
        Vector3 puntoMirar = jugador.position;
        puntoMirar.y = transform.position.y;
        transform.LookAt(puntoMirar);

        if (distancia > distanciaMinima)
        {
            Vector3 direccion = jugador.position - transform.position;
            direccion.y = 0;

            transform.position += direccion.normalized * speed * Time.deltaTime;
        }
    }

    void Atacar(float distancia)
    {
        if (distancia <= rango && Time.time >= tiempoProximoDisparo)
        {
            tiempoProximoDisparo = Time.time + cadencia;

            Vector3 direccion = (jugador.position - transform.position).normalized;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, direccion, out hit, rango))
            {
                Player scriptJugador = hit.collider.GetComponent<Player>();

                if (scriptJugador != null)
                {
                    scriptJugador.RecibirDanio(danio);
                }
            }
        }
    }

    public void RecibirDanio(float cantidad)
    {
        vida -= cantidad;
        Debug.Log(gameObject.name + " vida: " + vida);

        if (vida <= 0)
        {
            vida = 0;
            Debug.Log(gameObject.name + " murio");
        }
    }
}