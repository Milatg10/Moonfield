using UnityEngine;
using TMPro;

// Gestiona el cofre final del juego: muestra un panel de contraseña al interactuar,
// valida el código introducido y desencadena el final si es correcto.
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
        // Todos los paneles comienzan ocultos para no interferir con el inicio de la escena
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
            // Se bloquea el movimiento del jugador mientras el panel está abierto
            if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false;

            // Se limpia el campo y se oculta cualquier error previo antes de mostrar el panel
            campoPassword.text = "";
            if (panelError != null) panelError.SetActive(false);
            campoPassword.ActivateInputField();
        }
    }

    public void ComprobarPassword()
    {
        // Si hay un temporizador de borrado de error pendiente, se cancela para reiniciar la cuenta
        CancelInvoke("BorrarError");

        string intento = campoPassword.text.Trim();
        Debug.Log($"[COFRE] El jugador ha introducido el código: \"{intento}\"");

        // La comparación ignora mayúsculas/minúsculas para no penalizar al jugador por el formato
        if (intento.ToUpper() == passwordCorrecta.Trim().ToUpper())
        {
            Debug.Log("[COFRE] ¡CÓDIGO CORRECTO! El cofre se ha abierto con éxito. Fin de la demo.");

            CerrarPanelPassword();
            if (spriteRendererDelCofre != null && cofreAbierto != null) spriteRendererDelCofre.sprite = cofreAbierto;

            // Si existe el gestor de pantallas se delega en él la presentación del final del juego
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
            // Se registra el fallo en el sistema de métricas para el análisis de la experiencia de juego
            GeneradorReportes.fracasosTotales++;
            Debug.Log($"[SISTEMA] Contraseña incorrecta: '{campoPassword.text}'.");

            // Si el jugador ya conoce el secreto del árbol, se le ofrece la pista mediante diálogo
            // en lugar de mostrar el mensaje de error genérico
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
                    // El mensaje de error se oculta automáticamente tras 2 segundos
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
            // Solo se abre el panel si no hay ya una pantalla activa ni un diálogo en curso
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
        // Al alejarse del cofre se cierra el panel y se devuelve el control al jugador
        if (other.CompareTag("Player"))
        {
            CerrarPanelPassword();
        }
    }
}
