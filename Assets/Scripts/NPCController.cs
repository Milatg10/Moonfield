using UnityEngine;
using TMPro; 
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    [Header("Personalidad")]
    [TextArea(5, 10)] public string npcLore; // Aquí pegarás la historia de cada NPC
    
    [Header("Conexiones")]
    public LLMService cerebroIA;   // Arrastra aquí al GameManager
    public TextMeshProUGUI textoPantalla; // Arrastra aquí el texto del Canvas
    public TextMeshProUGUI textoNombre;    // Arrastra aquí el nombre del NPC
    public GameObject panelDialogo; // Arrastra el Panel entero para mostrarlo/ocultarlo

    private bool jugadorCerca = false;
    public string nombreNPC;

    void Start()
    {
        // Al empezar, ocultamos el panel
        if(panelDialogo != null) panelDialogo.SetActive(false);
    }

    void Update()
    {
        // Si el jugador está cerca y pulsa E
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            Hablar();
        }
    }

    void Hablar()
    {
        panelDialogo.SetActive(true);
        textoNombre.text = nombreNPC;
        textoPantalla.text = "Pensando...";

        // Llamamos a tu servicio (LLMService)
        // Le enviamos: Quién es (npcLore) y Qué dice el jugador (esto luego lo haremos dinámico)
        string mensajeJugador = "Hola, ¿qué ofreces?"; // Por ahora fijo
        
        StartCoroutine(cerebroIA.CallAI(npcLore, mensajeJugador, MostrarRespuesta));
    }

    // Esta función se ejecuta cuando la IA termina de pensar (Callback)
    void MostrarRespuesta(string respuesta)
    {
        textoPantalla.text = respuesta;
    }

    // Detectar si el jugador se acerca (Necesitas un Collider 2D en modo Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) jugadorCerca = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            panelDialogo.SetActive(false); // Cierra el chat al irse
        }
    }
}