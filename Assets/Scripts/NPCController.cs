using UnityEngine;
using TMPro; 
using UnityEngine.UI;
using System; 
using UnityEngine.Events; // ¡NUEVO! Necesario para eventos universales en el Inspector

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
    public GameObject zonaBotones;         

    [Header("--- CONFIGURACIÓN UNIVERSAL DEL NPC ---")]
    public string nombreNPC = "Pam";
    public string etiquetaExito ; // Lo que escribe la IA si ganas
    public string etiquetaFracaso; // Lo que escribe la IA si pierdes
    [TextArea(2, 4)] public string textoNarradorFracaso = "Pam se ha marchado furiosa y se niega a ayudarte..."; 

    [Header("--- EVENTOS DE CONSECUENCIAS ---")]
    // Aquí conectaremos en Unity qué pasa si ganas (ej. dar hacha) o pierdes (ej. tirar hacha al mapa)
    public UnityEvent eventoAlTenerExito; 
    public UnityEvent eventoAlFracasar;   

    [HideInInspector] 
    public Action eventoAlCerrarIA; 

    private int intentosMaximos = 3;
    private int intentosActuales;
    private bool jugadorCerca = false;

    private bool esperandoEnter = false;
    private bool faseMensajeMapa = false;

    void Start()
    {
        if(panelDialogo != null) panelDialogo.SetActive(false);
        if(contenedorInterfazIA != null) contenedorInterfazIA.SetActive(false); 
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E) && !panelDialogo.activeSelf)
        {
            Hablar();
        }

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

        if (controladorClasico != null)
        {
            if (controladorClasico.scriptDelJugador != null)
                controladorClasico.scriptDelJugador.puedeMoverse = false;
            if (controladorClasico.interfazInventario != null)
                controladorClasico.interfazInventario.SetActive(false); 
            if (controladorClasico.relojDelJuego != null)
                controladorClasico.relojDelJuego.elTiempoPasa = false; 
        }

        if (modoIAActivo)
        {
            if(contenedorInterfazIA != null) contenedorInterfazIA.SetActive(true);
            if(zonaBotones != null) zonaBotones.SetActive(false);
            
            cajetinEntrada.interactable = false; 
            textoPantalla.text = nombreNPC + " está mirándote...";

            // =========================================================
            // 🛑 ZONA MOCK - SALUDO INICIAL
            // =========================================================
            /* --- VERSIÓN FINAL PARA EL TFG (Descomentar esto) ---
            string peticionSaludo = "Saluda al jugador por primera vez de forma natural según tu personalidad y contexto.";
            StartCoroutine(cerebroIA.CallAI(npcLore, peticionSaludo, MostrarSaludoInicial));
            */

            // --- VERSIÓN DE PRUEBA (Borrar esto en el futuro) ---
            MostrarSaludoInicial("Hola. ¿Tienes algo interesante que contarme o solo vienes a molestar?");
            // =========================================================
        }
        else
        {
            if(contenedorInterfazIA != null) contenedorInterfazIA.SetActive(false);
            if(zonaBotones != null) zonaBotones.SetActive(true);
            
            if (controladorClasico != null && primerNodo != null)
                controladorClasico.IniciarDialogo(primerNodo);
        }
    }

    void MostrarSaludoInicial(string saludo)
    {
        textoPantalla.text = saludo + "\n<size=75%><color=yellow>(Tienes " + intentosActuales + " intentos)</color></size>";
        cajetinEntrada.interactable = true;
        cajetinEntrada.text = ""; 
        cajetinEntrada.Select(); 
        cajetinEntrada.ActivateInputField();

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
        if (string.IsNullOrEmpty(cajetinEntrada.text) || esperandoEnter) return;

        intentosActuales--; 
        textoPantalla.text = nombreNPC + " está pensando...";
        string mensajeJugador = cajetinEntrada.text; 
        cajetinEntrada.text = ""; 
        cajetinEntrada.interactable = false; 

        // =========================================================
        // 🛑 ZONA MOCK - RESPUESTA AL JUGADOR
        // =========================================================
        /* --- VERSIÓN FINAL PARA EL TFG (Descomentar esto) ---
        StartCoroutine(cerebroIA.CallAI(npcLore, mensajeJugador, MostrarRespuesta));
        */

        // --- VERSIÓN DE PRUEBA (Borrar esto en el futuro) ---
        string textoPrueba = mensajeJugador.ToLower();
        string respuestaFalsa = "";

        if (textoPrueba.Contains("hacha") || textoPrueba.Contains("alcalde") || textoPrueba.Contains("secreto")) 
        {
            respuestaFalsa = "Lo has conseguido, aquí tienes. " + etiquetaExito;
        } 
        else if (textoPrueba.Contains("tonto") || textoPrueba.Contains("feo")) 
        {
            respuestaFalsa = "¡Qué falta de respeto! Lárgate. " + etiquetaFracaso;
        } 
        else 
        {
            respuestaFalsa = "Eso es muy aburrido. Cuéntame algo mejor.";
        }
        
        MostrarRespuesta(respuestaFalsa);
        // =========================================================
    }

    void MostrarRespuesta(string respuesta)
    {
        // CASO ÉXITO (Dinámico)
        if (respuesta.Contains(etiquetaExito))
        {
            respuesta = respuesta.Replace(etiquetaExito, "").Trim();
            
            // Disparamos el evento genérico de éxito
            if (eventoAlTenerExito != null) eventoAlTenerExito.Invoke();
            
            textoPantalla.text = respuesta + "\n\n<size=75%><color=green>[Pulsa Enter para terminar]</color></size>";
            cajetinEntrada.interactable = false;
            esperandoEnter = true; 
            return;
        }

        // CASO FRACASO POR ETIQUETA (Dinámico)
        if (respuesta.Contains(etiquetaFracaso))
        {
            respuesta = respuesta.Replace(etiquetaFracaso, "").Trim();
            intentosActuales = 0; 
            
            textoPantalla.text = respuesta + "\n\n<size=75%><color=red>[" + nombreNPC + " se ha ofendido. Pulsa Enter para continuar]</color></size>";
            cajetinEntrada.interactable = false;
            esperandoEnter = true;
            faseMensajeMapa = true; 
            return;
        }

        // CASO FRACASO POR INTENTOS
        if (intentosActuales <= 0)
        {
            textoPantalla.text = respuesta + "\n\n<size=75%><color=orange>[Pulsa Enter para continuar]</color></size>";
            cajetinEntrada.interactable = false;
            esperandoEnter = true;
            faseMensajeMapa = true; 
        }
        else // SIGUE LA CHARLA
        {
            cajetinEntrada.interactable = true;
            textoPantalla.text = respuesta + "\n<size=75%><color=yellow>(Te quedan " + intentosActuales + " intentos)</color></size>";
            cajetinEntrada.Select();
            cajetinEntrada.ActivateInputField();
        }
    }

    void ManejarPulsacionEnter()
    {
        if (faseMensajeMapa)
        {
            if (contenedorInterfazIA != null) contenedorInterfazIA.SetActive(false);
            textoNombre.text = "Narrador";
            
            // Usamos el texto de narrador que hayamos configurado en el Inspector
            textoPantalla.text = "<color=red>" + textoNarradorFracaso + "</color>\n\n<size=75%>[Pulsa Enter para salir]</size>";
            
            // Disparamos el evento genérico de fracaso
            if (eventoAlFracasar != null) eventoAlFracasar.Invoke();
            
            faseMensajeMapa = false; 
        }
        else
        {
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
            if (controladorClasico.relojDelJuego != null)
                controladorClasico.relojDelJuego.elTiempoPasa = true; 
        }

        if (eventoAlCerrarIA != null)
        {
            eventoAlCerrarIA.Invoke();
            eventoAlCerrarIA = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) jugadorCerca = true; }
    private void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Player")) jugadorCerca = false; }
}