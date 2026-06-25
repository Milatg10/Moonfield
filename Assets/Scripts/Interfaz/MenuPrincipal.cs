using UnityEngine;
using UnityEngine.SceneManagement;

// Controla los botones del menú principal: guarda el modo de juego elegido
// en PlayerPrefs antes de cargar la escena, para que GestorModoJuego lo lea al arrancar.
public class MenuPrincipal : MonoBehaviour
{
    public string nombreEscenaJuego = "MainScene";

    public void JugarModoNormal()
    {
        PlayerPrefs.SetInt("ModoIA", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void JugarModoIA()
    {
        PlayerPrefs.SetInt("ModoIA", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void SalirJuego()
    {
        Debug.Log($"[SISTEMA] Saliendo del juego...");
        Application.Quit();
    }
}
