using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Interfaz de Pausa")]
    public GameObject panelPausa; 
    public string nombreEscenaMenu = "Menu"; 
    
    [Header("=== CONEXIONES ===")]
    public Movement scriptDelJugador; 
    public RelojJuego relojDelJuego; 

    private bool estaPausado = false;

    void Start()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estaPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        estaPausado = true;
        if (panelPausa != null) panelPausa.SetActive(true);
        
        Time.timeScale = 0f; 
        
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false;
        
        // ¡Congelamos el reloj!
        if (relojDelJuego != null) relojDelJuego.elTiempoPasa = false; 
    }

    public void Reanudar()
    {
        estaPausado = false;
        if (panelPausa != null) panelPausa.SetActive(false);
        
        Time.timeScale = 1f; 
        
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = true;
        
        // ¡Devolvemos el reloj a la normalidad!
        if (relojDelJuego != null) relojDelJuego.elTiempoPasa = true; 
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    public void SalirJuego()
    {
        Debug.Log("Saliendo de la demo...");
        Application.Quit();
    }
}