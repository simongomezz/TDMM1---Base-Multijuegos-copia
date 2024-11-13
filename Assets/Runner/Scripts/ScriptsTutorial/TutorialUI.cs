using UnityEngine;
using UnityEngine.UI;

public class TutorialUIController : MonoBehaviour
{
    public GameObject instruccionesImagen;  // La imagen en el Canvas
    private bool carrilCambiado = false;    // Verifica si el jugador ha cambiado de carril

    void Start()
    {
        // Asegura que la imagen esté visible al inicio
        if (instruccionesImagen != null)
        {
            instruccionesImagen.SetActive(true);
        }
    }

    // Este método lo llamará el script del jugador cuando cambie de carril por primera vez
    public void OcultarInstrucciones()
    {
        if (!carrilCambiado)
        {
            carrilCambiado = true;
            if (instruccionesImagen != null)
            {
                instruccionesImagen.SetActive(false); // Oculta la imagen
            }
        }
    }
}