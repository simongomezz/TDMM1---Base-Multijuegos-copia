using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Agregamos esto para manejar escenas (pasar de pantalla en pantalla => ganaste-perdiste)
using UnityEngine.UI; //Agregamos esto para manejar las propiedades UI (Canvas, Text, Image, etc).


public class Configuracion_General : MonoBehaviour
{
    [Header("Configuraci�n de tipo de juego")]
   
    static public bool runner3D = false;
    public bool _runner3D = false;


   // [Header("Configuraci�n de variables generales")]   
    
    public enum TiposDeJuego{
        Runner,
        Shooter,
        Sincro
    }
    public TiposDeJuego tipodejuego;

    public enum TiposDeDificultad{
        facil,
        intermedio,
        dificil
    }

    public float puntos = 0;
    public float tiempo;
    static public int cantPlayers = 1;
    static TiposDeDificultad dificultad; //La pueden utilizar como condición para variar la dificultad.
    public int vidas;
    public float velocidad;
    

    [Header("Configuracion de elementos UI")]
    [SerializeField] private Text scoreText; //Variable del texto que se visualizara en pantalla en el videojuego. La definimos como publica para poder arrastrar el objeto Text desde el Inspector
    [SerializeField] private Text lifeText;

    [Header("Configuracion de Escenas")]
    public int escenajuego;
    public int escenaperdiste;
    public int escenaganaste;
    public bool perdiste = false;
    public bool ganaste = false;

    void Awake() {
        runner3D = _runner3D;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntaje: " + ((float)puntos).ToString();
        }
        else
        {
            Debug.Log("No hay un text asignado para el puntaje");
        }
        if (lifeText != null)
        {
            lifeText.text = "Vidas: " + ((int)vidas).ToString();
        }
        else
        {
            Debug.Log("No hay un text asignado para la vida");
        }

        if (perdiste)
        {
            print("PERDISTE!");
            SceneManager.LoadScene(escenaperdiste);
        }
        else if (ganaste)
        {
            print("GANASTE!");
            SceneManager.LoadScene(escenaganaste);
        }


    }
}
