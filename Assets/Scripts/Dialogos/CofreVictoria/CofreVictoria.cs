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
    public PantallaIntro gestorPantallas; 

    [Header("Interfaz de Contraseña")]
    public GameObject panelPassword; 
    public TMP_InputField campoPassword; 
    public string passwordCorrecta = "1412"; 
    
    [Header("Mensaje de Error")]
    public GameObject panelError; 
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

        string intento = campoPassword.text.Trim();
        Debug.Log($"[COFRE] El jugador ha introducido el código: \"{intento}\"");

        if (intento.ToUpper() == passwordCorrecta.Trim().ToUpper())
        {

            GeneradorReportes.LanzarMetricasAlLog();

            Debug.Log("[COFRE] ¡CÓDIGO CORRECTO! El cofre se ha abierto con éxito. Fin de la demo.");

            CerrarPanelPassword(); 
            if (spriteRendererDelCofre != null && cofreAbierto != null) spriteRendererDelCofre.sprite = cofreAbierto;
            
            if (gestorPantallas != null)
            {
                string mensajeFinal = "¡Has abierto el cofre!\n\nGracias por jugar a la demo de Moonfield.";
                gestorPantallas.MostrarFinalJuego(mensajeFinal);
            }
            else
            {
                if (panelVictoria != null) panelVictoria.SetActive(true);
                if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false; 
            }
        }
        else
        {
            GeneradorReportes.fracasosTotales++;
            Debug.Log($"[SISTEMA] Contraseña incorrecta: '{campoPassword.text}'.");
            
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