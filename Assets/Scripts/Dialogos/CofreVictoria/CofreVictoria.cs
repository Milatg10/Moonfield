using UnityEngine;
using TMPro; 

public class CofreVictoria : MonoBehaviour
{
    [Header("Referencias Principales")]
    public ControladorDialogo controlador; 
    public Movement scriptDelJugador; 

    [Header("Gráficos del Cofre")]
    public SpriteRenderer spriteRendererDelCofre; 
    public Sprite cofreCerrado;  
    public Sprite cofreAbierto;  

    [Header("Interfaz Final (Antigua)")]
    public GameObject panelVictoria; 

    [Header("=== NUEVO GESTOR FINAL ===")]
    public PantallaIntro gestorPantallas; // ¡AQUÍ ESTÁ LA CONEXIÓN AL MENÚ!

    [Header("Interfaz de Contraseña")]
    public GameObject panelPassword; 
    public TMP_InputField campoPassword; 
    public string passwordCorrecta = "1412"; 
    
    [Header("Mensaje de Error")]
    public GameObject panelError; 

    [Header("Diálogo de Pista (Acertijo)")]
    public NodoDialogo nodoPista; 

    void Start()
    {
        if (panelVictoria != null) panelVictoria.SetActive(false);
        if (panelPassword != null) panelPassword.SetActive(false);
        if (panelError != null) panelError.SetActive(false); 

        if (spriteRendererDelCofre != null && cofreCerrado != null)
        {
            spriteRendererDelCofre.sprite = cofreCerrado;
        }
    }

    void AbrirPanelPassword()
    {
        if (panelPassword != null) 
        {
            panelPassword.SetActive(true);
            if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false;
            
            campoPassword.text = ""; 
            if (panelError != null) panelError.SetActive(false); 
            campoPassword.ActivateInputField(); 
        }
    }

    public void ComprobarPassword()
    {
        CancelInvoke("BorrarError"); 

        // ¡EL MOMENTO DE LA VICTORIA!
        if (campoPassword.text.Trim().ToUpper() == passwordCorrecta.Trim().ToUpper())
        {
            CerrarPanelPassword(); 
            
            // 1. Cambiamos el dibujo al cofre abierto
            if (spriteRendererDelCofre != null && cofreAbierto != null) 
                spriteRendererDelCofre.sprite = cofreAbierto;
            
            // 2. ¡Lanzamos la pantalla negra que nos lleva al Menú!
            if (gestorPantallas != null)
            {
                string mensajeFinal = "¡Has abierto el cofre y finalizado el juego!\n\nGracias por jugar a la demo de Moonfield.";
                gestorPantallas.MostrarFinalJuego(mensajeFinal);
            }
            else
            {
                // Red de seguridad: si se te olvida arrastrar la pantalla en el Inspector, usa el panel viejo
                if (panelVictoria != null) panelVictoria.SetActive(true);
                if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false; 
            }
        }
        // SI LA CONTRASEÑA ES INCORRECTA:
        else
        {
            if (controlador != null && controlador.conoceSecreto == true)
            {
                CerrarPanelPassword();
                if (nodoPista != null) controlador.IniciarDialogo(nodoPista);
            }
            else
            {
                if (panelError != null)
                {
                    panelError.SetActive(true); 
                    Invoke("BorrarError", 2f); 
                }
                campoPassword.text = ""; 
                campoPassword.ActivateInputField(); 
            }
        }
    }

    void BorrarError()
    {
        if (panelError != null) panelError.SetActive(false);
    }

    public void CerrarPanelPassword()
    {
        if (panelPassword != null) panelPassword.SetActive(false);
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            if (!panelVictoria.activeSelf && !panelPassword.activeSelf)
            {
                if (controlador == null || !controlador.cajaDialogo.activeSelf)
                {
                    AbrirPanelPassword();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            CerrarPanelPassword(); 
        }
    }
}