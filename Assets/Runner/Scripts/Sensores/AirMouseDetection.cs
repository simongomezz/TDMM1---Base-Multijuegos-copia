using UnityEngine;

public class AirMouseDetection : MonoBehaviour
{
    // Esta variable almacenará la posición anterior del ratón
    private Vector3 previousMousePosition;

    void Start()
    {
        // Al iniciar, guardamos la posición inicial del ratón
        previousMousePosition = Input.mousePosition;
    }

    void Update()
    {
        // Obtenemos la posición actual del ratón
        Vector3 currentMousePosition = Input.mousePosition;

        // Imprimimos la posición del ratón en la consola
        Debug.Log("Posición del AirMouse: " + currentMousePosition);

        // Verificamos si el ratón se ha movido
        if (currentMousePosition != previousMousePosition)
        {
            // Si el ratón se ha movido, imprimimos la distancia recorrida
            float distanceMoved = Vector3.Distance(previousMousePosition, currentMousePosition);
            Debug.Log("El ratón se movió. Distancia: " + distanceMoved);

            // Actualizamos la posición previa
            previousMousePosition = currentMousePosition;
        }
    }
}