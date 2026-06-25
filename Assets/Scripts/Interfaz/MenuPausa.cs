using UnityEngine;
using UnityEngine.SceneManagement;

// Gestiona el menú de pausa: congela y descongela el juego al pulsar Escape
// y ofrece las acciones de reanudar, volver al menú principal y salir.
public class MenuPausa : MonoBehaviour
{
    [Header("Interfaz de Pausa")]
    public GameObject panelPausa;
    public string nombreEscenaMenu = "Menu";

    [Header("Conexiones")]
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
            if (estaPausado) Reanudar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        estaPausado = true;
        if (panelPausa != null) panelPausa.SetActive(true);

        // Time.timeScale a 0 detiene todas las físicas y animaciones basadas en deltaTime
        Time.timeScale = 0f;

        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false;
        if (relojDelJuego != null) relojDelJuego.elTiempoPasa = false;
    }

    public void Reanudar()
    {
        estaPausado = false;
        if (panelPausa != null) panelPausa.SetActive(false);

        Time.timeScale = 1f;

        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = true;
        if (relojDelJuego != null) relojDelJuego.elTiempoPasa = true;
    }

    public void VolverAlMenu()
    {
        // Se restaura el timeScale antes de cambiar de escena para que la nueva no arranque congelada
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    public void SalirJuego()
    {
        Debug.Log($"[SISTEMA] Saliendo de la demo...");
        Application.Quit();
    }
}
