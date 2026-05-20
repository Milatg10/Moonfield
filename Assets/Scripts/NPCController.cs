using UnityEngine;
using TMPro; 
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    [Header("=== EL INTERRUPTOR DEL TFG ===")]
    public bool modoIAActivo = false; 

    [Header("--- MODO CLÁSICO (Árboles) ---")]
    public ControladorDialogo controladorClasico; 
    public NodoDialogo primerNodo; 

    [Header("--- MODO IA (LLM) ---")]
    [TextArea(5, 10)] public string npcLore; 
    public LLMService cerebroIA;   
    public TextMeshProUGUI textoPantalla; 
    public TextMeshProUGUI textoNombre;    
    public GameObject panelDialogo; 
    
    [Space(10)]
    public GameObject contenedorInterfazIA; 
    public TMP_InputField cajetinEntrada;    
    public Button botonEnviar;               

    [Header("--- INVENTARIO Y JUEGO ---")]
    public ObjetoInventario hachaParaEntregar; 

    private int intentosMaximos = 3;
    private int intentosActuales;
    private bool jugadorCerca = false;
    public string nombreNPC = "Pam";

    // Variables de control para el flujo de lectura e Intro
    private bool esperandoEnter = false;
    private bool faseMensajeMapa = false;

    void Start()
    {
        if(panelDialogo != null) panelDialogo.SetActive(false);
        if(contenedorInterfazIA != null) contenedorInterfazIA.SetActive(false);
    }

    void Update()
    {
        // 1. Abrir diálogo normal
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E) && !panelDialogo.activeSelf)
        {
            Hablar();
        }

        // 2. DETECTOR MAESTRO DEL ENTER PARA PASAR TEXTOS
        if (esperandoEnter && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            ManejarPulsacionEnter();
        }
    }

    public void Hablar()
    {
        panelDialogo.SetActive(true);
        textoNombre.text = nombreNPC;
        intentosActuales = intentosMaximos; 
        esperandoEnter = false;
        faseMensajeMapa = false;

        // Congelamos al jugador y ocultamos inventario
        if (controladorClasico != null)
        {
            if (controladorClasico.scriptDelJugador != null)
                controladorClasico.scriptDelJugador.puedeMoverse = false;
            if (controladorClasico.interfazInventario != null)
                controladorClasico.interfazInventario.SetActive(false); 
        }

        if (modoIAActivo)
        {
            if(contenedorInterfazIA != null) contenedorInterfazIA.SetActive(true);
            
            // Bloqueamos el cajetín temporalmente mientras la IA genera el saludo
            cajetinEntrada.interactable = false; 
            textoPantalla.text = "Pam está mirándote...";

            // LLAMADA 1: El propio LLM genera el saludo inicial acorde a su Lore
            string peticionSaludo = "Saluda al jugador por primera vez de forma natural según tu personalidad y contexto.";
            StartCoroutine(cerebroIA.CallAI(npcLore, peticionSaludo, MostrarSaludoInicial));
        }
        else
        {
            if(contenedorInterfazIA != null) contenedorInterfazIA.SetActive(false);
            if (controladorClasico != null && primerNodo != null)
                controladorClasico.IniciarDialogo(primerNodo);
        }
    }

    void MostrarSaludoInicial(string saludo)
    {
        textoPantalla.text = saludo + "\n<size=75%><color=yellow>(Tienes " + intentosActuales + " intentos)</color></size>";
        
        // Desbloqueamos el cajetín para que el jugador pueda escribir ya
        cajetinEntrada.interactable = true;
        cajetinEntrada.text = ""; 
        cajetinEntrada.Select(); 
        cajetinEntrada.ActivateInputField();

        // Configuramos botones y Enter de escritura
        if (botonEnviar != null)
        {
            botonEnviar.onClick.RemoveAllListeners(); 
            botonEnviar.onClick.AddListener(EnviarMensaje);
        }

        cajetinEntrada.onSubmit.RemoveAllListeners();
        cajetinEntrada.onSubmit.AddListener(delegate { EnviarMensaje(); });
    }

    void EnviarMensaje()
    {
        // Si el cajetín está vacío o estamos esperando que pase un texto, no hacemos nada
        if (string.IsNullOrEmpty(cajetinEntrada.text) || esperandoEnter) return;

        intentosActuales--; 

        textoPantalla.text = "Pam está pensando...";
        string mensajeJugador = cajetinEntrada.text; 
        cajetinEntrada.text = ""; 
        cajetinEntrada.interactable = false; // Bloqueamos entrada mientras procesa

        StartCoroutine(cerebroIA.CallAI(npcLore, mensajeJugador, MostrarRespuesta));
    }

    void MostrarRespuesta(string respuesta)
    {
        // CASO 1: ¡ÉXITO! Nos da el hacha
        if (respuesta.Contains("[DAR_HACHA]"))
        {
            respuesta = respuesta.Replace("[DAR_HACHA]", "").Trim();
            
            if (controladorClasico != null && controladorClasico.inventario != null && hachaParaEntregar != null)
            {
                controladorClasico.inventario.AñadirObjeto(hachaParaEntregar);
            }
            
            textoPantalla.text = respuesta + "\n\n<size=75%><color=green>[Pulsa Enter para terminar]</color></size>";
            cajetinEntrada.interactable = false;
            esperandoEnter = true; // Esperamos el Enter de lectura
            return;
        }

        // CASO 2: FRACASO (Te quedas sin intentos. El LLM ya ha generado su respuesta de despedida enfadada)
        if (intentosActuales <= 0)
        {
            textoPantalla.text = respuesta + "\n\n<size=75%><color=orange>[Pulsa Enter para continuar]</color></size>";
            cajetinEntrada.interactable = false;
            esperandoEnter = true;
            faseMensajeMapa = true; // Activamos la fase del recado del mapa
        }
        // CASO 3: RESPUESTA NORMAL (Continúa la charla)
        else
        {
            cajetinEntrada.interactable = true;
            textoPantalla.text = respuesta + "\n<size=75%><color=yellow>(Te quedan " + intentosActuales + " intentos)</color></size>";
            cajetinEntrada.Select();
            cajetinEntrada.ActivateInputField();
        }
    }

    // GESTOR DE ESCENAS DE TEXTO AL PULSAR ENTER
    void ManejarPulsacionEnter()
    {
        if (faseMensajeMapa)
        {
            // Ocultamos el cajetín de la IA para que parezca un mensaje del sistema/narrador
            if (contenedorInterfazIA != null) contenedorInterfazIA.SetActive(false);
            textoNombre.text = "Narrador";
            
            textoPantalla.text = "<color=red>Pam se ha marchado furiosa y se niega a ayudarte. El hacha debe de haber quedado olvidada en algún rincón del bosque... Tendrás que explorar el mapa para encontrarla por tu cuenta.</color>\n\n<size=75%>[Pulsa Enter para salir]</size>";
            
            faseMensajeMapa = false; // El siguiente Enter ya cerrará del todo
        }
        else
        {
            // Fin de la conversación, limpiamos todo y descongelamos
            CerrarDialogoIA();
        }
    }

    public void CerrarDialogoIA()
    {
        esperandoEnter = false;
        faseMensajeMapa = false;

        if (panelDialogo != null) panelDialogo.SetActive(false); 
        if (contenedorInterfazIA != null) contenedorInterfazIA.SetActive(false);

        if (controladorClasico != null)
        {
            if (controladorClasico.scriptDelJugador != null)
                controladorClasico.scriptDelJugador.puedeMoverse = true;
            if (controladorClasico.interfazInventario != null)
                controladorClasico.interfazInventario.SetActive(true); 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) jugadorCerca = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) jugadorCerca = false;
    }
}