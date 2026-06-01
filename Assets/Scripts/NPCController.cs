using UnityEngine;
using TMPro; 
using UnityEngine.UI;
using System; 
using UnityEngine.Events; 

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
    public string etiquetaExito; 
    public string etiquetaFracaso; 
    [TextArea(2, 4)] public string textoNarradorFracaso = "Pam se ha marchado furiosa y se niega a ayudarte..."; 

    [Header("--- EVENTOS DE CONSECUENCIAS ---")]
    public UnityEvent eventoAlTenerExito; 
    public UnityEvent eventoAlFracasar;   

    [HideInInspector] 
    public Action eventoAlCerrarIA; 

    private int intentosMaximos = 3;
    private int intentosActuales;
    private bool esperandoEnter = false;
    private bool faseMensajeMapa = false;

    void Start()
    {
        if(panelDialogo != null) panelDialogo.SetActive(false);
        if(contenedorInterfazIA != null) contenedorInterfazIA.SetActive(false); 
    }

    void Update()
    {
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

            string peticionSaludo = "Saluda al jugador de forma natural según tu personalidad. No le des pistas de buenas a primeras.";
            StartCoroutine(cerebroIA.CallAI(npcLore, peticionSaludo, MostrarSaludoInicial));
        }
        else 
        {
            if(contenedorInterfazIA != null) contenedorInterfazIA.SetActive(false);
            if(zonaBotones != null) zonaBotones.SetActive(true);
            
            if (controladorClasico != null && primerNodo != null)
            {
                // ¡AQUÍ ESTÁ EL ESCÁNER DE DNI!
                Debug.Log($"🟢 1. [NPC] Soy {gameObject.name}. Le estoy metiendo las reglas de éxito/fracaso al Controlador llamado '{controladorClasico.gameObject.name}' que tiene el DNI: {controladorClasico.gameObject.GetInstanceID()}");

                controladorClasico.eventoAlCerrar = () => 
                {
                    Debug.Log("🟢 2. [NPC LAMBDA] ¡El Controlador me ha llamado de vuelta! Evaluando amistad: " + controladorClasico.amistadPam);
                    if (controladorClasico.amistadPam >= 1)
                    {
                        Debug.Log("🟢 3A. [NPC] ¡Éxito! Disparando evento de éxito...");
                        if (eventoAlTenerExito != null) eventoAlTenerExito.Invoke();
                    }
                    else
                    {
                        Debug.Log("🟢 3B. [NPC] Fracaso. Disparando evento de fracaso...");
                        if (eventoAlFracasar != null) eventoAlFracasar.Invoke();
                    }
                };

                controladorClasico.IniciarDialogo(primerNodo);
            }
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

        StartCoroutine(cerebroIA.CallAI(npcLore, mensajeJugador, MostrarRespuesta));
    }

    void MostrarRespuesta(string respuesta)
    {
        if (!string.IsNullOrEmpty(etiquetaExito) && respuesta.Contains(etiquetaExito))
        {
            respuesta = respuesta.Replace(etiquetaExito, "").Trim();
            if (eventoAlTenerExito != null) eventoAlTenerExito.Invoke();
            
            textoPantalla.text = respuesta + "\n\n<size=75%><color=green>[Pulsa Enter para terminar]</color></size>";
            cajetinEntrada.interactable = false;
            esperandoEnter = true; 
            return;
        }

        if (!string.IsNullOrEmpty(etiquetaFracaso) && respuesta.Contains(etiquetaFracaso))
        {
            respuesta = respuesta.Replace(etiquetaFracaso, "").Trim();
            intentosActuales = 0; 
            
            textoPantalla.text = respuesta + "\n\n<size=75%><color=red>[" + nombreNPC + " se ha ofendido. Pulsa Enter para continuar]</color></size>";
            cajetinEntrada.interactable = false;
            esperandoEnter = true;
            faseMensajeMapa = true; 
            return;
        }

        if (intentosActuales <= 0)
        {
            textoPantalla.text = respuesta + "\n\n<size=75%><color=orange>[Pulsa Enter para continuar]</color></size>";
            cajetinEntrada.interactable = false;
            esperandoEnter = true;
            faseMensajeMapa = true; 
        }
        else 
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
            textoPantalla.text = "<color=red>" + textoNarradorFracaso + "</color>\n\n<size=75%>[Pulsa Enter para salir]</size>";
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

    private void OnTriggerEnter2D(Collider2D other) 
    { 
        if (other.CompareTag("Player")) 
        {
            bool dialogoClasicoAbierto = (controladorClasico != null && controladorClasico.cajaDialogo.activeSelf);
            
            if (!panelDialogo.activeSelf && !dialogoClasicoAbierto)
            {
                Hablar();
            }
        } 
    }
}