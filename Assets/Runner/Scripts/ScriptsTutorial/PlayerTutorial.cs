using UnityEngine;
using TMPro;  // Para usar TextMeshPro si necesitas mostrar mensajes en pantalla

public class PlayerTutorial : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    public float speed = 10f;                 // Velocidad de avance en el eje Z
    public float carrilOffset = 3.0f;         // Distancia entre los carriles
    private float carrilIzquierdo;
    private float carrilCentro;
    private float carrilDerecho;
    private int carrilActual = 1;             // 0 = Izquierdo, 1 = Centro, 2 = Derecho
    public bool primerCambioCarril = false;

    [Header("Estado de atrapamiento")]
    public bool isCaught = false;             // Indica si el jugador está atrapado
    private int keyPressCount = 0;
    private float releaseTimeLimit = 3.0f;    // Tiempo límite para liberarse
    private float releaseTimer;
    private int requiredKeyPresses = 3;       // Número de pulsaciones necesarias para liberarse
    public TextMeshProUGUI caughtText;

    private TutorialUIController tutorialUIController;
    private TutorialSpawnManager spawnManager;

    void Start()
    {
        carrilCentro = transform.position.x;
        carrilIzquierdo = carrilCentro - carrilOffset;
        carrilDerecho = carrilCentro + carrilOffset;

        tutorialUIController = Object.FindFirstObjectByType<TutorialUIController>();
        spawnManager = Object.FindFirstObjectByType<TutorialSpawnManager>(); // Referencia al controlador de spawneo del tutorial
        
        if (caughtText != null) caughtText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isCaught)
        {
            HandleCaughtState();  // Lógica de liberación
        }
        else
        {
            MoveForward();        // Movimiento normal
            HandleLaneChange();   // Cambio de carril
        }
    }

    private void MoveForward()
    {
        // Movimiento hacia adelante
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void HandleLaneChange()
    {
        // Cambia de carril usando las teclas A, S y D
        if (Input.GetKeyDown(KeyCode.A))
        {
            CambiarCarril(0); // Carril izquierdo
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            CambiarCarril(1); // Carril central
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            CambiarCarril(2); // Carril derecho
        }
    }

    private void CambiarCarril(int nuevoCarril)
    {
        // Cambia la posición solo si es a un carril diferente
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

        // Marca el primer cambio de carril y genera muros
        if (!primerCambioCarril)
        {
            primerCambioCarril = true;

            if (tutorialUIController != null)
            {
                tutorialUIController.OcultarInstrucciones();
            }

            // Llama al método para spawnear los muros
            if (spawnManager != null)
            {
                spawnManager.SpawnWalls();
            }
        }
    }

    public void StartCaughtState()
    {
        // Activa el estado de atrapamiento
        isCaught = true;
        keyPressCount = 0;
        releaseTimer = releaseTimeLimit;
        if (caughtText != null)
        {
            caughtText.gameObject.SetActive(true);
            caughtText.text = $"¡Estás atrapado! Presiona E {requiredKeyPresses - keyPressCount} veces para liberarte.";
        }
        Debug.Log("¡Estás atrapado! Presiona E para liberarte.");
    }

    private void HandleCaughtState()
    {
        releaseTimer -= Time.deltaTime;

        // Detecta la tecla E para liberarse
        if (Input.GetKeyDown(KeyCode.E))
        {
            keyPressCount++;
            Debug.Log($"Presionaste E, conteo: {keyPressCount}");

            // Actualiza el mensaje en pantalla si existe el texto
            if (caughtText != null)
            {
                caughtText.text = $"¡Estás atrapado! Presiona E {requiredKeyPresses - keyPressCount} veces más";
            }
        }

        // Liberación exitosa
        if (keyPressCount >= requiredKeyPresses)
        {
            ReleasePlayer();
        }
        // Si el tiempo se acaba, el jugador pierde
        else if (releaseTimer <= 0)
        {
            LoseGame();
        }
    }

    private void ReleasePlayer()
    {
        isCaught = false;
        keyPressCount = 0;
        if (caughtText != null)
        {
            caughtText.gameObject.SetActive(false);
        }
        Debug.Log("¡Te has liberado!");
    }

    private void LoseGame()
    {
        // Activa el estado de pérdida del juego en la configuración general
        Debug.Log("Perdiste el juego porque no te liberaste a tiempo.");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Activa el estado atrapado si colisiona con un muro
        if (other.CompareTag("Wall"))
        {
            StartCaughtState();
        }
    }
}