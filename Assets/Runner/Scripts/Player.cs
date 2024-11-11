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
    [SerializeField] private float stopPositionZ = 300f;
    private bool canMoveForward = true;
    public TextMeshProUGUI inmunityText;

    [Header("Liberación de Enemigo")]
    public bool isCaught = false;
    private int keyPressCount = 0;
    private float releaseTimeLimit = 3.0f;
    private float releaseTimer;
    private int requiredKeyPresses = 3;
    public TextMeshProUGUI caughtText;

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

        if (inmunityText != null) inmunityText.gameObject.SetActive(false);
        if (caughtText != null) caughtText.gameObject.SetActive(false);
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
        caughtText.gameObject.SetActive(false);
        Debug.Log("¡Te has liberado!");
    }

    private void LoseGame()
    {
        config.perdiste = true;
        Debug.Log("Perdiste el juego porque no te liberaste a tiempo.");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Solo activa el estado atrapado si colisiona con un muro
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
        Debug.Log("Inmunidad desactivada");
    }

    public void AllowForwardMovement()
    {
        canMoveForward = true;
        Debug.Log("Se ha desbloqueado el avance del jugador.");
    }

    // Método Damage para ser llamado externamente
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

    // Método para manejar el movimiento controlado por OSC
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
}