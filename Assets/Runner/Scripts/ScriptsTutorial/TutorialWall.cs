using UnityEngine;

public class TutorialWall : MonoBehaviour
{
    [Header("Configuración de Muro Rompible")]
    public float speed = 3.0f;  // Velocidad de movimiento en el eje Z

    private void Update()
    {
        Movimiento();
    }

    private void Movimiento()
    {
        if (transform.position.z >= -6.0f)
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
        }
        else
        {
            DestruirMuro();
        }
    }

    private void DestruirMuro()
    {
        // Destruye el muro cuando sale del área visible
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si el jugador colisiona con el muro, activar el estado "atrapado"
        if (other.CompareTag("Player"))
        {
            PlayerTutorial player = other.GetComponent<PlayerTutorial>();
            if (player != null)
            {
                player.StartCaughtState();  // Activar el estado atrapado (asumiendo que el script PlayerTutorial tenga este método)
                Debug.Log("El jugador ha colisionado con el muro.");
            }

            // Destruye el muro después de colisionar
            Destroy(gameObject);
        }
    }
}