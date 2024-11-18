using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Necesario para trabajar con UI
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    public bool carriles = false;
    public bool autoPilot = false;
    [HideInInspector] public float[] posCarriles;
    [SerializeField] private float movementDistance = 6.0f;

    public float playerPosition;
    [SerializeField] private float limitX = 8.10f;

    [HideInInspector] public float speed = 8;

    [Header("Configuración de vida")]
    [HideInInspector] public int life = 1;
    [HideInInspector] public bool inmunity = false;

    [Header("Configuración generales")]
    [SerializeField] private Configuracion_General config;

    [Header("Configuración de Pared")]
    [SerializeField] private float stopPositionZ = 300f;
    private bool canMoveForward = true;
    public TextMeshProUGUI inmunityText;

    [Header("Configuración de inmunidad")]
    public Image inmunityImage; // Nueva referencia a la imagen de inmunidad

    [Header("Liberación de Enemigo")]
    public bool isCaught = false;
    private int keyPressCount = 0;
    private float releaseTimeLimit = 3.0f;
    private float releaseTimer;
    private int requiredKeyPresses = 3;
    public TextMeshProUGUI caughtText;

    // movimiento entre carriles
    public float carrilIzquierdo;
    public float carrilCentro;
    public float carrilDerecho;
    private int carrilActual = 1;
    public bool primerCambioCarril = false;

    // Variables para el AirMouse
    private Vector3 previousMousePosition;
    public float liberationThreshold = 40.0f; // Umbral de distancia de movimiento para liberar al jugador

    private void Start()
    {
        life = config.vidas;
        speed = config.velocidad;

        // Definir posiciones de los carriles
        carrilCentro = transform.position.x;
        carrilIzquierdo = carrilCentro - movementDistance;
        carrilDerecho = carrilCentro + movementDistance;

        if (inmunityText != null) inmunityText.gameObject.SetActive(false);
        if (caughtText != null) caughtText.gameObject.SetActive(false);
        if (inmunityImage != null) inmunityImage.gameObject.SetActive(false); // Asegurarse de que la imagen está desactivada al inicio

        // Inicializar la posición del AirMouse
        previousMousePosition = Input.mousePosition;
    }

    private void Update()
    {
        if (isCaught)
        {
            HandleCaughtState();
        }
        else
        {
            Movement();
        }

        // Comprobar si el AirMouse se ha movido significativamente
        if (DetectMouseMovement())
        {
            ReleasePlayer(); // Llamamos correctamente a ReleasePlayer() si la distancia del mouse es suficiente
        }
    }

    private void Movement()
    {
        if (carriles)
        {
            HandleLaneChange();
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
        }

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
            BreakWall();
        }
    }

    private void HandleLaneChange()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CambiarCarril(0);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            CambiarCarril(1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            CambiarCarril(2);
        }
    }

    private void CambiarCarril(int nuevoCarril)
    {
        if (nuevoCarril == carrilActual) return;

        carrilActual = nuevoCarril;
        Vector3 nuevaPosicion = transform.position;

        if (carrilActual == 0)
            nuevaPosicion.x = carrilIzquierdo;
        else if (carrilActual == 1)
            nuevaPosicion.x = carrilCentro;
        else if (carrilActual == 2)
            nuevaPosicion.x = carrilDerecho;

        transform.position = nuevaPosicion;

        if (!primerCambioCarril)
        {
            primerCambioCarril = true;
        }
    }

    private void BreakWall()
    {
        WallObstacle wall = FindFirstObjectByType<WallObstacle>();
        if (wall != null)
        {
            wall.BreakWall();
            canMoveForward = true;
            Debug.Log("Pared rota, el jugador puede avanzar.");
        }
        else
        {
            Debug.LogWarning("No se encontró la pared en la escena.");
        }
    }

    private void HandleCaughtState()
    {
        releaseTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.E))
        {
            keyPressCount++;
            Debug.Log("Presionaste E, conteo: " + keyPressCount);
        }

        caughtText.text = $"¡Estás atrapado! Presiona E {requiredKeyPresses - keyPressCount} veces más";
        
        if (keyPressCount >= requiredKeyPresses)
        {
            ReleasePlayer(); // Esta es la misma función que se llama cuando se presiona E
        }
        else if (releaseTimer <= 0)
        {
            LoseGame();
        }
    }

    void ReleasePlayer()
    {
        isCaught = false;
        keyPressCount = 0;

        if (caughtText != null)
        {
            caughtText.gameObject.SetActive(false);
        }

        Debug.Log("¡Te has liberado con el AirMouse!");
    }

    private void LoseGame()
    {
        config.perdiste = true;
        Debug.Log("Perdiste el juego porque no te liberaste a tiempo.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            StartCaughtState();
            Debug.Log("Colisión con muro, inmunidad ignorada.");
        }
        else if (other.CompareTag("Enemy") && !inmunity)
        {
            StartCaughtState();
            Debug.Log("Colisión con enemigo sin inmunidad.");
        }
    }

    public void StartCaughtState()
    {
        isCaught = true;
        keyPressCount = 0;
        releaseTimer = releaseTimeLimit;
        caughtText.gameObject.SetActive(true);
        Debug.Log("¡Estás atrapado! Presiona E para liberarte.");
    }

    public IEnumerator ActivarInmunidad(float duracion)
    {
        inmunity = true;

        if (inmunityText != null) inmunityText.gameObject.SetActive(true);
        if (inmunityImage != null) inmunityImage.gameObject.SetActive(true);

        float tiempoRestante = duracion;

        while (tiempoRestante > 0)
        {
            if (inmunityText != null)
            {
                inmunityText.text = "Inmunidad activa: " + tiempoRestante.ToString("F1") + " segundos";
            }
            yield return new WaitForSeconds(0.1f);
            tiempoRestante -= 0.1f;
        }

        inmunity = false;

        if (inmunityText != null) inmunityText.gameObject.SetActive(false);
        if (inmunityImage != null) inmunityImage.gameObject.SetActive(false);

        Debug.Log("Inmunidad desactivada");
    }

    public void AllowForwardMovement()
    {
        canMoveForward = true;
        Debug.Log("Se ha desbloqueado el avance del jugador.");
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

    bool DetectMouseMovement()
    {
        Vector3 currentMousePosition = Input.mousePosition;
        float distanceMoved = Vector3.Distance(previousMousePosition, currentMousePosition);

        Debug.Log("El ratón se movió. Distancia: " + distanceMoved);

        if (distanceMoved > liberationThreshold)
        {
            previousMousePosition = currentMousePosition;
            return true;
        }
        return false;
    }
}