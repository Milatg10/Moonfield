using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

public class PantallaIntro : MonoBehaviour
{
    [Header("=== INTERFAZ ===")]
    public GameObject panelIntro; 
    public TextMeshProUGUI textoPantalla; 

    [Header("=== CONEXIONES ===")]
    public Movement scriptDelJugador; 
    public ControladorDialogo controladorClasico; 

    [Header("=== NAVEGACIÓN ===")]
    public string nombreEscenaMenu = "Menu"; // El nombre de la escena de tu menú
    private bool esFinalDelJuego = false; // Bandera para saber si toca salir

    void Start()
    {
        if (panelIntro != null) panelIntro.SetActive(true);
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false;
        
        if (controladorClasico != null && controladorClasico.relojDelJuego != null)
        {
            controladorClasico.relojDelJuego.elTiempoPasa = false;
        }
    }

    void Update()
    {
        if (panelIntro != null && panelIntro.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) 
            {
                CerrarPantalla();
            }
        }
    }

    // Mensaje normal en mitad de la partida (ej. al romper el tronco)
    public void MostrarNuevoMensaje(string nuevoTexto)
    {
        esFinalDelJuego = false; 
        PrepararPantalla(nuevoTexto);
    }

    // Mensaje de fin de demo (ej. al abrir el cofre)
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
            // Si era el mensaje final, destruimos el mapa y volvemos al menú
            SceneManager.LoadScene(nombreEscenaMenu);
        }
        else
        {
            // Si era un mensaje normal, simplemente quitamos la pausa
            if (panelIntro != null) panelIntro.SetActive(false);
            if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = true;

            if (controladorClasico != null && controladorClasico.relojDelJuego != null)
                controladorClasico.relojDelJuego.elTiempoPasa = true;
        }
    }
}