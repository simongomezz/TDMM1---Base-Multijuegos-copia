using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerTutorial : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    public float speed = 10f;
    public float carrilOffset = 3.0f;
    private float carrilIzquierdo;
    private float carrilCentro;
    private float carrilDerecho;
    private int carrilActual = 1;
    public bool primerCambioCarril = false;

    [Header("Estado de atrapamiento")]
    public bool isCaught = false;
    private int keyPressCount = 0;
    private float releaseTimeLimit = 3.0f;
    private float releaseTimer;
    private int requiredKeyPresses = 3;
    public TextMeshProUGUI caughtText;

    [Header("Control de UI")]
    private TutorialUIController tutorialUIController;

    private TutorialSpawnManager spawnManager;

    void Start()
    {
        carrilCentro = transform.position.x;
        carrilIzquierdo = carrilCentro - carrilOffset;
        carrilDerecho = carrilCentro + carrilOffset;

        tutorialUIController = Object.FindFirstObjectByType<TutorialUIController>();
        spawnManager = Object.FindFirstObjectByType<TutorialSpawnManager>();

        if (caughtText != null) caughtText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isCaught)
        {
            HandleCaughtState();
        }
        else
        {
            MoveForward();
            HandleLaneChange();
        }
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
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

            if (tutorialUIController != null)
            {
                tutorialUIController.OcultarInstrucciones();
            }

            if (spawnManager != null)
            {
                spawnManager.SpawnWalls();
            }
        }
    }

public void StartCaughtState()
{
    isCaught = true;
    keyPressCount = 0;
    releaseTimer = releaseTimeLimit;

    if (caughtText != null)
    {
        caughtText.gameObject.SetActive(true);
        caughtText.text = $"¡Estás atrapado! Presiona E {requiredKeyPresses - keyPressCount} veces para liberarte.";
    }

    if (tutorialUIController != null)
    {
        // Mostrar la imagen de atrapado en el tutorial
        tutorialUIController.MostrarAtrapadoImagen();

        // Ocultar la segunda imagen si aún está activa
        tutorialUIController.OcultarSegundaImagen();
    }

    Debug.Log("¡Estás atrapado! Presiona E para liberarte.");
}


    private void HandleCaughtState()
    {
        releaseTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.E))
        {
            keyPressCount++;
            Debug.Log($"Presionaste E, conteo: {keyPressCount}");

            if (caughtText != null)
            {
                caughtText.text = $"¡Estás atrapado! Presiona E {requiredKeyPresses - keyPressCount} veces más";
            }
        }

        if (keyPressCount >= requiredKeyPresses)
        {
            ReleasePlayer();
        }
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

        if (tutorialUIController != null)
        {
            // Ocultar la imagen de atrapado cuando se libera
            tutorialUIController.OcultarAtrapadoImagen();
        }
        Debug.Log("¡Te has liberado!");
    }

    private void LoseGame()
    {
        Debug.Log("Perdiste el juego porque no te liberaste a tiempo.");
        SceneManager.LoadScene("Perdiste");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            StartCaughtState();
        }
    }
}