using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [Header("Configuración de Muro Rompible")]
    public float detectionDistance = 7.0f;   // Distancia para detectar si el jugador está cerca
    public int life = 1;                     // Vida del muro, por si quieres que tenga más de un golpe
    [SerializeField] private float speed = 3.0f; // Velocidad de movimiento en el eje Z
    [SerializeField] private Configuracion_General config;

    private void Start()
    {
        // Buscar el script de configuración general
        GameObject gm = GameObject.FindWithTag("GameController");
        if (gm != null)
        {
            config = gm.GetComponent<Configuracion_General>();
        }
    }

    private void Update()
    {
        Movement();

        // Opción para romper el muro con barra espaciadora
        if (Input.GetKeyDown(KeyCode.Space) && IsPlayerNearby())
        {
            BreakWall();
        }
    }

    private void Movement()
    {
        // Movimiento del muro en el eje Z
        if (Configuracion_General.runner3D == false)
        {
            if (transform.position.y >= -6.0f)
            {
                transform.Translate(Vector3.down * speed * Time.deltaTime);
            }
            else
            {
                DestroyWall();
            }
        }
        else
        {
            if (transform.position.z >= -6.0f)
            {
                transform.Translate(Vector3.back * speed * Time.deltaTime);
            }
            else
            {
                DestroyWall();
            }
        }
    }

    private bool IsPlayerNearby()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            return distance <= detectionDistance;
        }
        return false;
    }

    private void BreakWall()
    {
        Destroy(gameObject);
        Debug.Log("El jugador ha roto el muro manualmente.");
    }

    private void DestroyWall()
    {
        // Aquí destruyes el muro cuando se sale del área visible o cuando su vida es 0
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que ha colisionado es el jugador
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage(1); // Restar 1 vida al jugador
                Debug.Log("El jugador ha colisionado con el muro y ha perdido vida.");
            }

            // Destruir el muro después de colisionar
            Destroy(gameObject);
        }
    }
}
