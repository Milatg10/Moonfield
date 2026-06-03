using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class MenuPrincipal : MonoBehaviour
{
    // Asegúrate de poner aquí el nombre EXACTO de tu escena de juego
    public string nombreEscenaJuego = "SampleScene"; 

    public void JugarModoNormal()
    {
        // Guardamos un 0 en la memoria de Unity (0 = Clásico)
        PlayerPrefs.SetInt("ModoIA", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void JugarModoIA()
    {
        // Guardamos un 1 en la memoria de Unity (1 = IA)
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