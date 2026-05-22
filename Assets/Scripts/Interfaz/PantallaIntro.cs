using UnityEngine;

public class PantallaIntro : MonoBehaviour
{
    [Header("=== INTERFAZ ===")]
    public GameObject panelIntro; // Arrastra aquí el Panel_Intro del Canvas

    [Header("=== CONEXIONES ===")]
    public Movement scriptDelJugador; // Arrastra a tu jugador aquí para congelarlo
    // (Nota: Si tu script de moverse se llama distinto, cambia la palabra 'Movement')

    void Start()
    {
        // Al darle al Play, nos aseguramos de que el panel se vea y congelamos al jugador
        if (panelIntro != null) 
        {
            panelIntro.SetActive(true);
        }
        
        if (scriptDelJugador != null) 
        {
            scriptDelJugador.puedeMoverse = false;
        }
    }

    void Update()
    {
        // Si el panel está encendido, estamos en la Intro y esperamos que el jugador pulse algo
        if (panelIntro != null && panelIntro.activeSelf)
        {
            // Detectamos el Enter para pasar la intro
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ComenzarAventura();
            }
        }
    }

    void ComenzarAventura()
    {
        // ¡Se acabó la intro! Apagamos la pantalla
        panelIntro.SetActive(false);
        
        // Descongelamos al jugador para que empiece a caminar hacia el trigger de Pam
        if (scriptDelJugador != null)
        {
            scriptDelJugador.puedeMoverse = true;
        }
    }
}