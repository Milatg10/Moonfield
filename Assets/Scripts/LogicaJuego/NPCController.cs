using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

// Gestiona la conversación con un NPC en dos modos intercambiables:
// modo árbol clásico (diálogo por nodos predefinidos) y modo IA (texto libre procesado por un LLM).
// El modo activo se establece desde GestorModoJuego al cargar la escena.
public class NPCController : MonoBehaviour
{
    [Header("Modo de juego")]
    public bool modoIAActivo = false;

    [Header("Modo clásico (árboles de diálogo)")]
    public ControladorDialogo controladorClasico;
    public NodoDialogo primerNodo;

    [Header("Modo IA (LLM)")]
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

    [Header("Configuración universal del NPC")]
    public string nombreNPC = "Pam";
    // Etiquetas que el LLM incluye en su respuesta para señalar el desenlace de la conversación
    public string etiquetaExito;
    public string etiquetaFracaso;
    [TextArea(2, 4)] public string textoNarradorFracaso = "Pam se ha marchado furiosa y se niega a ayudarte...";

    [Header("Eventos de consecuencias")]
    public UnityEvent eventoAlTenerExito;
    public UnityEvent eventoAlFracasar;

    // Callback que otros sistemas suscriben para ejecutar lógica al cerrar el diálogo de IA
    [HideInInspector]
    public Action eventoAlCerrarIA;

    private int intentosMaximos = 3;
    private int intentosActuales;
    private bool esperandoEnter = false;
    // Indica que el jugador ha fallado y aún falta mostrar el mensaje del narrador antes de cerrar
    private bool faseMensajeMapa = false;

    void Start()
    {
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (contenedorInterfazIA != null) contenedorInterfazIA.SetActive(false);
    }

    void Update()
    {
        if (esperandoEnter && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            ManejarPulsacionEnter();
    }

    public void Hablar()
    {
        panelDialogo.SetActive(true);
        textoNombre.text = nombreNPC;
        intentosActuales = intentosMaximos;
        esperandoEnter = false;
        faseMensajeMapa = false;

        // Se congela el estado del juego independientemente del modo activo
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
            if (contenedorInterfazIA != null) contenedorInterfazIA.SetActive(true);
            if (zonaBotones != null) zonaBotones.SetActive(false);

            // El campo se bloquea hasta recibir la respuesta del LLM para evitar envíos prematuros
            cajetinEntrada.interactable = false;
            textoPantalla.text = nombreNPC + " está mirándote...";

            string peticionSaludo = "Saluda al jugador de forma natural según tu personalidad. No le des pistas de buenas a primeras.";
            StartCoroutine(cerebroIA.CallAI(npcLore, peticionSaludo, MostrarSaludoInicial));
        }
        else
        {
            if (contenedorInterfazIA != null) contenedorInterfazIA.SetActive(false);
            if (zonaBotones != null) zonaBotones.SetActive(true);

            if (controladorClasico != null && primerNodo != null)
            {
                // Se suscribe el resultado al evento de cierre para que el desenlace
                // se ejecute justo cuando el jugador termina el diálogo, no antes
                controladorClasico.eventoAlCerrar = () =>
                {
                    if (controladorClasico.amistadPam >= 1)
                    {
                        if (eventoAlTenerExito != null) eventoAlTenerExito.Invoke();
                    }
                    else
                    {
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
            // Se limpian los listeners antes de añadir el nuevo para evitar suscripciones duplicadas
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

        // Se registran métricas del mensaje antes de enviarlo al LLM
        GeneradorReportes.mensajesIA++;
        string[] palabras = mensajeJugador.Split(new char[] { ' ', '.', '?', ',', '!' }, StringSplitOptions.RemoveEmptyEntries);
        GeneradorReportes.palabrasTotalesIA += palabras.Length;
        Debug.Log($"[DIÁLOGO IA - {nombreNPC}] Jugador escribe: \"{mensajeJugador}\"");

        cajetinEntrada.text = "";
        cajetinEntrada.interactable = false;

        StartCoroutine(cerebroIA.CallAI(npcLore, mensajeJugador, MostrarRespuesta));
    }

    void MostrarRespuesta(string respuesta)
    {
        Debug.Log($"[DIÁLOGO IA - {nombreNPC}] IA responde: \"{respuesta}\"");

        // El LLM señala el éxito incluyendo la etiqueta acordada en su respuesta;
        // se elimina la etiqueta del texto antes de mostrarlo al jugador
        if (!string.IsNullOrEmpty(etiquetaExito) && respuesta.Contains(etiquetaExito))
        {
            Debug.Log($"[SISTEMA IA] Resultado con {nombreNPC}: ÉXITO. Entregando recompensa.");

            respuesta = respuesta.Replace(etiquetaExito, "").Trim();
            if (eventoAlTenerExito != null) eventoAlTenerExito.Invoke();

            textoPantalla.text = respuesta + "\n\n<size=75%><color=green>[Pulsa Enter para terminar]</color></size>";
            cajetinEntrada.interactable = false;
            esperandoEnter = true;
            return;
        }

        // El LLM señala el fracaso por ofensa con su propia etiqueta, distinta al agotamiento de intentos
        if (!string.IsNullOrEmpty(etiquetaFracaso) && respuesta.Contains(etiquetaFracaso))
        {
            Debug.Log($"[SISTEMA IA] Resultado con {nombreNPC}: FRACASO. El NPC se ha ofendido.");

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
            Debug.Log($"[SISTEMA IA] Resultado con {nombreNPC}: FRACASO. El jugador se ha quedado sin intentos.");

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
            // Antes de cerrar se muestra el mensaje del narrador que contextualiza el fracaso narrativamente
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
        if (zonaBotones != null) zonaBotones.SetActive(true);

        if (controladorClasico != null)
        {
            if (controladorClasico.scriptDelJugador != null)
                controladorClasico.scriptDelJugador.puedeMoverse = true;
            if (controladorClasico.interfazInventario != null)
                controladorClasico.interfazInventario.SetActive(true);
            if (controladorClasico.relojDelJuego != null)
                controladorClasico.relojDelJuego.elTiempoPasa = true;
        }

        // Se invoca el callback y se limpia para que no persista entre conversaciones distintas
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
            // No se inicia una nueva conversación si ya hay un diálogo clásico abierto
            bool dialogoClasicoAbierto = controladorClasico != null && controladorClasico.cajaDialogo.activeSelf;

            if (!panelDialogo.activeSelf && !dialogoClasicoAbierto)
                Hablar();
        }
    }
}
