using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

// Muestra una pantalla de texto que bloquea el juego hasta que el jugador pulsa Enter.
// Se usa tanto para mensajes narrativos en mitad de la partida como para la pantalla
// de fin de demo; en este último caso, al cerrarla se vuelve al menú principal.
public class PantallaIntro : MonoBehaviour
{
    [Header("Interfaz")]
    public GameObject panelIntro;
    public TextMeshProUGUI textoPantalla;

    [Header("Conexiones")]
    public Movement scriptDelJugador;
    public ControladorDialogo controladorClasico;

    [Header("Navegación")]
    public string nombreEscenaMenu = "Menu";
    // Distingue si el cierre de la pantalla debe regresar al menú o simplemente reanudar la partida
    private bool esFinalDelJuego = false;

    void Start()
    {
        // La pantalla de intro se muestra desde el primer frame y congela el juego hasta que el jugador la cierre
        if (panelIntro != null) panelIntro.SetActive(true);
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false;

        if (controladorClasico != null && controladorClasico.relojDelJuego != null)
            controladorClasico.relojDelJuego.elTiempoPasa = false;
    }

    void Update()
    {
        if (panelIntro != null && panelIntro.activeSelf)
        {
            // Se acepta tanto el Enter del teclado principal como el del teclado numérico
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                CerrarPantalla();
        }
    }

    public void MostrarNuevoMensaje(string nuevoTexto)
    {
        esFinalDelJuego = false;
        PrepararPantalla(nuevoTexto);
    }

    public void MostrarFinalJuego(string textoFinal)
    {
        esFinalDelJuego = true;
        PrepararPantalla(textoFinal);
    }

    private void PrepararPantalla(string texto)
    {
        if (textoPantalla != null) textoPantalla.text = texto;
        if (panelIntro != null) panelIntro.SetActive(true);
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false;

        if (controladorClasico != null && controladorClasico.relojDelJuego != null)
            controladorClasico.relojDelJuego.elTiempoPasa = false;
    }

    void CerrarPantalla()
    {
        if (esFinalDelJuego)
        {
            SceneManager.LoadScene(nombreEscenaMenu);
        }
        else
        {
            if (panelIntro != null) panelIntro.SetActive(false);
            if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = true;

            if (controladorClasico != null && controladorClasico.relojDelJuego != null)
                controladorClasico.relojDelJuego.elTiempoPasa = true;
        }
    }
}
