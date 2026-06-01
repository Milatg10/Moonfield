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

        if (campoPassword.text.Trim().ToUpper() == passwordCorrecta.Trim().ToUpper())
        {
            CerrarPanelPassword(); 
            if (spriteRendererDelCofre != null && cofreAbierto != null) spriteRendererDelCofre.sprite = cofreAbierto;
            if (panelVictoria != null) panelVictoria.SetActive(true);
            if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false; 
        }
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

    // ¡AQUÍ ESTÁ EL CAMBIO! Salta al entrar en la zona
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