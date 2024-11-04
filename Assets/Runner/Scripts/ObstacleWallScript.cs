using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObstacle : MonoBehaviour
{
    private Configuracion_General config;
    private int requiredHits = 3;  // Cantidad inicial de golpes necesarios para romper la pared
    private int currentHits = 0;   // Contador de golpes actuales
    private bool requiredHitsSet = false; // Verifica si los golpes necesarios ya han sido establecidos

    private void Start()
    {
        // Encuentra el script de configuración general al iniciar
        config = Object.FindFirstObjectByType<Configuracion_General>();

        if (config == null)
        {
            Debug.LogError("ConfiguracionGeneral no encontrado.");
        }
    }

    private void Update()
    {
        // Verifica si el jugador ha alcanzado la distancia objetivo de la pared
        Player player = Object.FindFirstObjectByType<Player>();

        if (player != null)
        {
            float distanceToPlayer = transform.position.z - player.transform.position.z;

            // Establece el valor de requiredHits cuando el jugador esté cerca (a 15 unidades o menos)
            if (distanceToPlayer <= 15.0f && !requiredHitsSet)
            {
                player.autoPilot = false;  // Detiene el movimiento automático del jugador
                UpdateRequiredHits();
                requiredHitsSet = true; // Asegura que los golpes necesarios se establezcan solo una vez
            }

            // Detecta si se ha presionado la barra espaciadora para golpear el muro
            if (Input.GetKeyDown(KeyCode.Space) && requiredHitsSet)
            {
                currentHits++;
                Debug.Log("Golpe recibido. Total de golpes realizados: " + currentHits);

                if (currentHits >= requiredHits)
                {
                    BreakWall();  // Llama al método para romper la pared
                }
                else
                {
                    Debug.Log("Golpes restantes para romper la pared: " + (requiredHits - currentHits));
                }
            }
        }
    }

    private void UpdateRequiredHits()
    {
        // Ajusta la cantidad de golpes necesarios según el número de brazaletes recogidos
        if (Bracelet.braceletsCollected >= 9)
        {
            requiredHits = 1;
        }
        else if (Bracelet.braceletsCollected >= 5)
        {
            requiredHits = 2;
        }
        else
        {
            requiredHits = 3;
        }

        Debug.Log("Golpes necesarios para romper la pared: " + requiredHits); // Confirma los golpes necesarios en consola
    }

    public void BreakWall()
    {
        // Restaura el movimiento automático del jugador y destruye la pared
        Player player = Object.FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.autoPilot = true;
            config.ganaste = true;  // Marca que el jugador ha ganado
            Debug.Log("¡Pared rota y has ganado!");
        }

        Destroy(gameObject);  // Destruye el objeto de la pared
    }
}
