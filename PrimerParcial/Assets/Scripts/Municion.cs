using UnityEngine;

public class Municion : MonoBehaviour
{
    [SerializeField] private int cantidadBalas = 5;

    // Se ejecuta cuando algo entra al Collider (que tiene que estar en Is Trigger)
    void OnTriggerEnter(Collider other)
    {
        Player jugador = other.GetComponent<Player>();

        // Si lo que entro no es el jugador, no hace nada
        if (jugador != null)
        {
            jugador.AgregarBalas(cantidadBalas);
            Destroy(gameObject); // desaparece el cubo de la escena
        }
    }
}