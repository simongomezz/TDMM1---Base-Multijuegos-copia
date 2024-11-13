using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObstacle : MonoBehaviour
{
    private Configuracion_General config;
    private int requiredHits = 3;  // Cantidad inicial de golpes necesarios para romper la pared
    private int currentHits = 0;   // Contador de golpes actuales
    private bool requiredHitsSet = false; // Verifica si los golpes necesarios ya han sido establecidos

    private float timeSinceLastHit = 0f;  // Tiempo acumulado desde el último golpe
    private float maxTimeWithoutBreaking = 10f;  // Tiempo límite de 10 segundos sin romper la pared

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
        Player player = Object.FindFirstObjectByType<Player>();

        if (player != null)
        {
            float distanceToPlayer = transform.position.z - player.transform.position.z;

            if (distanceToPlayer <= 15.0f && !requiredHitsSet)
            {
                player.autoPilot = false;
                UpdateRequiredHits();
                requiredHitsSet = true;
            }

            // Detecta si se ha presionado la barra espaciadora para golpear el muro
            if (Input.GetKeyDown(KeyCode.Space) && requiredHitsSet)
            {
                currentHits++;
                timeSinceLastHit = 0f;  // Reinicia el temporizador al recibir un golpe
                Debug.Log("Golpe recibido. Total de golpes realizados: " + currentHits);

                if (currentHits >= requiredHits)
                {
                    BreakWall();
                }
                else
                {
                    Debug.Log("Golpes restantes para romper la pared: " + (requiredHits - currentHits));
                }
            }
            else if (requiredHitsSet)
            {
                // Incrementa el temporizador si no se está golpeando la pared
                timeSinceLastHit += Time.deltaTime;
                
                // Si el jugador excede el tiempo sin romper la pared, lo atrapa
                if (timeSinceLastHit >= maxTimeWithoutBreaking)
                {
                    player.StartCaughtState();  // Activa el estado atrapado en el jugador
                    Debug.Log("El jugador ha sido atrapado por permanecer demasiado tiempo frente a la pared.");
                    timeSinceLastHit = 0f;  // Reinicia el temporizador para evitar múltiples capturas
                }
            }
        }
    }

    private void UpdateRequiredHits()
    {
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

        Debug.Log("Golpes necesarios para romper la pared: " + requiredHits);
    }

    public void BreakWall()
    {
        Player player = Object.FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.autoPilot = true;
            config.ganaste = true;
            Debug.Log("¡Pared rota y has ganado!");
        }

        Destroy(gameObject);
    }
}
