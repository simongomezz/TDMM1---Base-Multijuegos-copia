using UnityEngine;
using UnityEngine.UI;

public class TutorialUIController : MonoBehaviour
{
    public GameObject instruccionesImagen;       // La imagen de instrucciones inicial
    public GameObject segundaImagen;             // La nueva imagen que aparece después del primer cambio de carril
    public GameObject atrapadoImagen;            // La imagen que aparece al ser atrapado

    private bool carrilCambiado = false;         // Verifica si el jugador ha cambiado de carril

    void Start()
    {
        // Asegura que la primera imagen esté visible y las demás estén ocultas al inicio
        if (instruccionesImagen != null)
        {
            instruccionesImagen.SetActive(true);
        }

        if (segundaImagen != null)
        {
            segundaImagen.SetActive(false);
        }

        if (atrapadoImagen != null)
        {
            atrapadoImagen.SetActive(false);
        }
    }

    // Este método lo llamará el script del jugador cuando cambie de carril por primera vez
    public void OcultarInstrucciones()
    {
        if (!carrilCambiado)
        {
            carrilCambiado = true;

            // Oculta la primera imagen y muestra la segunda
            if (instruccionesImagen != null)
            {
                instruccionesImagen.SetActive(false);
            }

            if (segundaImagen != null)
            {
                segundaImagen.SetActive(true);
            }
        }
    }

    // Método para ocultar la segunda imagen definitivamente
    public void OcultarSegundaImagen()
    {
        if (segundaImagen != null)
        {
            segundaImagen.SetActive(false);
        }
    }

    // Método para mostrar la imagen de atrapado
    public void MostrarAtrapadoImagen()
    {
        if (atrapadoImagen != null)
        {
            atrapadoImagen.SetActive(true);
        }
    }

    // Método para ocultar la imagen de atrapado
    public void OcultarAtrapadoImagen()
    {
        if (atrapadoImagen != null)
        {
            atrapadoImagen.SetActive(false);
        }
    }
}