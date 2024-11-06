using System.Collections;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    public bool carriles = false;
    public bool autoPilot = false;
    [HideInInspector] public float[] posCarriles;
    [SerializeField] private int cantCarriles = 3;
    [SerializeField] private float movementDistance = 6.0f;

    public float playerPosition;
    [SerializeField] private float limitY = -3.5f;
    [SerializeField] private float limitX = 8.10f;

    [HideInInspector] public float speed = 8;
    [SerializeField] private bool puedeVolar = false;

    [Header("Configuración de vida")]
    [HideInInspector] public int life = 1;
    [HideInInspector] public bool inmunity = false;

    [Header("Configuración generales")]
    [SerializeField] private Configuracion_General config;

    [Header("Configuración de Pared")]
    [SerializeField] private float stopPositionZ = 300f; // Posición en Z donde el jugador se detendrá
    private bool canMoveForward = true; // Controla si el jugador puede avanzar después de la pared
    public TextMeshProUGUI inmunityText; 

    private void Start()
    {
        life = config.vidas;
        speed = config.velocidad;

        if (carriles)
        {
            if (cantCarriles == 2)
            {
                posCarriles = new float[3] { -movementDistance, 0, movementDistance };
            }
            else if (cantCarriles == 3)
            {
                posCarriles = new float[2] { -movementDistance, movementDistance };
            }
            else
            {
                Debug.Log("Estás intentando usar " + cantCarriles + ". El permitido es tres o dos. Para otra configuración hay que programarlo.");
            }
        }

        // Asegúrate de que el texto de inmunidad esté inicialmente desactivado
        if (inmunityText != null)
        {
            inmunityText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (carriles)
        {
            float playerPosition = transform.position.x;

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (playerPosition < posCarriles[1])
                {
                    transform.Translate(movementDistance, 0, 0);
                }
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (playerPosition > posCarriles[0])
                {
                    transform.Translate(-movementDistance, 0, 0);
                }
            }
        }
        else
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            transform.Translate(Vector3.right * speed * horizontalInput * Time.deltaTime);

            if (transform.position.x > limitX)
            {
                transform.position = new Vector3(limitX, transform.position.y, transform.position.z);
            }
            else if (transform.position.x < -limitX)
            {
                transform.position = new Vector3(-limitX, transform.position.y, transform.position.z);
            }

            if (puedeVolar)
            {
                float verticalInput = Input.GetAxis("Vertical");
                transform.Translate(Vector2.up * speed * verticalInput * Time.deltaTime);

                if (transform.position.y > 0)
                {
                    transform.position = new Vector2(transform.position.x, 0);
                }
                else if (transform.position.y < limitY)
                {
                    transform.position = new Vector2(transform.position.x, limitY);
                }
            }
        }

        // Control de movimiento automático en el eje Z
        if (autoPilot && canMoveForward)
        {
            if (transform.position.z < stopPositionZ)
            {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else
            {
                canMoveForward = false;
                Debug.Log("El jugador ha alcanzado la pared y se detuvo.");
            }
        }
        else if (!canMoveForward && Input.GetKeyDown(KeyCode.Space))
        {
            // Si el jugador está detenido frente a la pared y presiona Espacio, intenta "romper" la pared
            BreakWall();
        }
    }

    private void BreakWall()
    {
        WallObstacle wall = FindFirstObjectByType<WallObstacle>();
        if (wall != null)
        {
            wall.BreakWall(); // Llama al método en el script de la pared para "romperla"
            canMoveForward = true; // Permite al jugador avanzar nuevamente después de romper la pared
            Debug.Log("Pared rota, el jugador puede avanzar.");
        }
        else
        {
            Debug.LogWarning("No se encontró la pared en la escena.");
        }
    }

    public void OnTriggerEnter(Collider obj)
    {
        Debug.Log("Choqué con algo y su tag es: " + obj.gameObject.tag);
    }

    public void Damage(int _dmg)
    {
        if (!inmunity)
        {
            if (life > 0)
            {
                life -= _dmg;
                if (life <= 0)
                {
                    config.perdiste = true;
                    Destroy(this.gameObject);
                }
            }
            config.vidas = life;
        }
        else
        {
            inmunity = false;
        }
    }

    public void moveOSC(float _x)
    {
        transform.Translate(Vector3.right * speed * _x * Time.deltaTime);
        if (transform.position.x > limitX)
        {
            transform.position = new Vector3(limitX, transform.position.y);
        }
        else if (transform.position.x < -limitX)
        {
            transform.position = new Vector3(-limitX, transform.position.y);
        }
    }

    // Nueva función para activar inmunidad e imprimir tiempo restante en el Canvas
    public IEnumerator ActivarInmunidad(float duracion)
    {
        inmunity = true; // Activar la inmunidad

        // Activa el texto de inmunidad en el Canvas
        if (inmunityText != null)
        {
            inmunityText.gameObject.SetActive(true);
        }

        float tiempoRestante = duracion;

        while (tiempoRestante > 0)
        {
            if (inmunityText != null)
            {
                // Actualiza el texto con el tiempo restante de inmunidad
                inmunityText.text = "Inmunidad activa: " + tiempoRestante.ToString("F1") + " segundos";
            }
            yield return new WaitForSeconds(0.1f);
            tiempoRestante -= 0.1f;
        }

        inmunity = false;

        // Desactiva el texto de inmunidad al finalizar
        if (inmunityText != null)
        {
            inmunityText.gameObject.SetActive(false);
        }

        Debug.Log("Inmunidad desactivada");
    }

    // Método que permite el avance del jugador después de romper la pared
    public void AllowForwardMovement()
    {
        canMoveForward = true; // Permite el avance nuevamente
        Debug.Log("Se ha desbloqueado el avance del jugador.");
    }
}
