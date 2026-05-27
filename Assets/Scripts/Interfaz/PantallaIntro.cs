using UnityEngine;

public class PantallaIntro : MonoBehaviour
{
    [Header("=== INTERFAZ ===")]
    public GameObject panelIntro; 

    [Header("=== CONEXIONES ===")]
    public Movement scriptDelJugador; 
    public ControladorDialogo controladorClasico; // ¡NUEVO! Para acceder al reloj

    void Start()
    {
        if (panelIntro != null) panelIntro.SetActive(true);
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false;
        
        // ¡PARAMOS EL RELOJ EN LA INTRO!
        if (controladorClasico != null && controladorClasico.relojDelJuego != null)
        {
            controladorClasico.relojDelJuego.elTiempoPasa = false;
        }
    }

    void Update()
    {
        if (panelIntro != null && panelIntro.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0))
            {
                ComenzarAventura();
            }
        }
    }

    void ComenzarAventura()
    {
        panelIntro.SetActive(false);
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = true;

        // ¡REANUDAMOS EL RELOJ AL EMPEZAR A JUGAR!
        if (controladorClasico != null && controladorClasico.relojDelJuego != null)
        {
            controladorClasico.relojDelJuego.elTiempoPasa = true;
        }
    }
}