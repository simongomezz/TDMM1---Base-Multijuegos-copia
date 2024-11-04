using System.Collections;
using UnityEngine;

public class BoostMovement : MonoBehaviour
{
    private float speed; // Velocidad que se asignará desde el BoostSpawnManager

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void Update()
    {
        // Mover el boost hacia el jugador en el eje Z
        if (transform.position.z >= -6.0f)
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject); // Destruir el boost si sale del rango visual
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegurarse de que el jugador tenga la etiqueta "Player"
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.StartCoroutine(player.ActivarInmunidad(3.0f)); // Otorgar inmunidad por 3 segundos
                Debug.Log("Jugador ha recogido un boost: inmunidad activada");
            }
            
            // Destruir el boost una vez recogido
            Destroy(gameObject);
        }
    }
}