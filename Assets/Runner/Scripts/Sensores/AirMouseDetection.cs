using UnityEngine;

public class AirMouseDetection : MonoBehaviour
{
    private Vector3 previousMousePosition;
    public float significantMovementThreshold = 1f; // Distancia mínima para detectar un movimiento significativo

    private bool significantMovementDetected = false;
    private float cooldownTime = 1f; // Tiempo de enfriamiento en segundos
    private float cooldownTimer = 0f; // Temporizador interno

    void Start()
    {
        previousMousePosition = Input.mousePosition;
    }

    void Update()
    {
        // Si está en enfriamiento, incrementa el temporizador
        if (significantMovementDetected)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownTime)
            {
                // Reinicia el estado y permite detectar nuevamente
                significantMovementDetected = false;
                cooldownTimer = 0f;
            }
        }
        else
        {
            // Detecta movimiento significativo si no está en enfriamiento
            Vector3 currentMousePosition = Input.mousePosition;
            float distanceMoved = Vector3.Distance(previousMousePosition, currentMousePosition);

            if (distanceMoved > significantMovementThreshold)
            {
                significantMovementDetected = true;
                Debug.Log("Movimiento significativo detectado: " + distanceMoved);
            }

            // Actualiza la posición previa del mouse
            previousMousePosition = currentMousePosition;
        }
    }

    public bool IsSignificantMovement()
    {
        // Retorna verdadero solo si hay un movimiento significativo detectado y no está en enfriamiento
        Debug.Log("IsSignificantMovement llamado. Estado actual: " + significantMovementDetected);
        return significantMovementDetected;
    }
}
