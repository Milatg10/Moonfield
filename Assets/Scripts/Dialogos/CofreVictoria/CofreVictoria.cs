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

    [Header("Interfaz Final")]
    public GameObject panelVictoria; 

    [Header("Interfaz de Contraseña")]
    public GameObject panelPassword; 
    public TMP_InputField campoPassword; 
    public string passwordCorrecta = "1412"; 
    
    [Header("Mensaje de Error")]
    public GameObject panelError; // ¡NUEVO! Ahora gestionamos el panel entero

    [Header("Diálogo de Pista (Acertijo)")]
    public NodoDialogo nodoPista; 

    private bool jugadorCerca = false;

    void Start()
    {
        if (panelVictoria != null) panelVictoria.SetActive(false);
        if (panelPassword != null) panelPassword.SetActive(false);
        if (panelError != null) panelError.SetActive(false); // Que el error empiece apagado

        if (spriteRendererDelCofre != null && cofreCerrado != null)
        {
            spriteRendererDelCofre.sprite = cofreCerrado;
        }
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E) && !panelVictoria.activeSelf && !panelPassword.activeSelf)
        {
            if (controlador != null && !controlador.cajaDialogo.activeSelf)
            {
                AbrirPanelPassword();
            }
        }
    }

    void AbrirPanelPassword()
    {
        if (panelPassword != null) 
        {
            panelPassword.SetActive(true);
            if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false;
            
            campoPassword.text = ""; 
            if (panelError != null) panelError.SetActive(false); // Lo apagamos al volver a abrir
            campoPassword.ActivateInputField(); 
        }
    }

    public void ComprobarPassword()
    {
        // Cancelamos cualquier apagado pendiente
        CancelInvoke("BorrarError"); 

        if (campoPassword.text.Trim().ToUpper() == passwordCorrecta.Trim().ToUpper())
        {
            // ¡ACERTÓ!
            CerrarPanelPassword(); 
            if (spriteRendererDelCofre != null && cofreAbierto != null) spriteRendererDelCofre.sprite = cofreAbierto;
            if (panelVictoria != null) panelVictoria.SetActive(true);
            if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false; 
        }
        else
        {
            // ¡FALLÓ!
            if (controlador != null && controlador.conoceSecreto == true)
            {
                // Ya habló con Michael -> Pista del acertijo
                CerrarPanelPassword();
                if (nodoPista != null) controlador.IniciarDialogo(nodoPista);
            }
            else
            {
                // No ha hablado con Michael -> Mostramos el panel de error 2 segundos
                if (panelError != null)
                {
                    panelError.SetActive(true); // Encendemos el panel negro con su texto
                    Invoke("BorrarError", 2f); 
                }
                campoPassword.text = ""; 
                campoPassword.ActivateInputField(); 
            }
        }
    }

    // Función que apaga el panel
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
        if (other.CompareTag("Player")) jugadorCerca = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            jugadorCerca = false;
            CerrarPanelPassword(); 
        }
    }
}