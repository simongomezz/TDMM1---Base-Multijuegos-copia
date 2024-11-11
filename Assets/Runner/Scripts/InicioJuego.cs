using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InicioJuego : MonoBehaviour
{
    [Header("Configuración UI")]
    [SerializeField] private TMP_Text startText;

    void Start()
    {
        if (startText != null) startText.text = "Presiona Espacio para comenzar";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CargarEscenaDeJuego();
        }
    }

    private void CargarEscenaDeJuego()
    {
        SceneManager.LoadScene("Runner3D");
    }
}
