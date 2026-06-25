using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

// Controla el flujo completo de los diálogos con opciones: muestra los nodos,
// procesa la elección del jugador, acumula la puntuación de amistad y
// determina el desenlace de la conversación al llegar al nodo final.
public class ControladorDialogo : MonoBehaviour
{
    [Header("Elementos Visuales (Arrastrar desde el Canvas)")]
    public GameObject cajaDialogo;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoMensaje;
    public GameObject[] botones;
    public TextMeshProUGUI[] textosBotones;

    [Header("Datos Internos")]
    public int amistadPam = 0;
    private NodoDialogo nodoActual;
    public bool tieneHacha = false;

    [Header("Nodos Finales (Evaluación)")]
    public NodoDialogo nodoFinalBueno;
    public NodoDialogo nodoFinalMalo;

    // Callback opcional que otros sistemas suscriben para ejecutar lógica al cerrar el diálogo
    [HideInInspector]
    public Action eventoAlCerrar;

    [Header("Referencias")]
    public Movement scriptDelJugador;
    public SistemaInventario inventario;
    public ObjetoInventario hachaParaEntregar;

    [Header("Referencias UI Extras")]
    public GameObject interfazInventario;

    [Header("Conexión con el Reloj")]
    public RelojJuego relojDelJuego;

    [Header("CASTIGO DEL HACHA")]
    public GameObject hachaFisicaEnMapa;
    public bool conoceSecreto = false;

    void Start()
    {
        cajaDialogo.SetActive(false);
    }

    public void IniciarDialogo(NodoDialogo primerNodo)
    {
        // Al iniciar un diálogo se congela el estado del juego: sin movimiento, sin inventario y sin reloj
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false;
        if (interfazInventario != null) interfazInventario.SetActive(false);
        if (relojDelJuego != null) relojDelJuego.elTiempoPasa = false;

        // La puntuación de amistad se reinicia en cada conversación para que cada partida sea independiente
        amistadPam = 0;
        cajaDialogo.SetActive(true);

        Debug.Log($"\n[DIÁLOGO] --- Iniciando conversación de opciones con {primerNodo.nombreNPC} ---");

        MostrarNodo(primerNodo);
    }

    public void MostrarNodo(NodoDialogo nodo)
    {
        nodoActual = nodo;
        textoNombre.text = nodo.nombreNPC;
        textoNombre.color = nodo.colorNombre;
        textoMensaje.text = nodo.textoDelNPC;

        // Se ocultan todos los botones antes de activar solo los que corresponden a este nodo
        for (int i = 0; i < botones.Length; i++) { botones[i].SetActive(false); }

        // Si el nodo no tiene respuestas es un nodo terminal: se muestra solo el botón de cerrar
        if (nodo.respuestas.Length == 0)
        {
            botones[0].SetActive(true);
            textosBotones[0].text = "Cerrar";

            botones[0].GetComponent<Button>().onClick.RemoveAllListeners();
            botones[0].GetComponent<Button>().onClick.AddListener(CerrarDialogo);
            return;
        }

        for (int i = 0; i < nodo.respuestas.Length; i++)
        {
            botones[i].SetActive(true);
            textosBotones[i].text = nodo.respuestas[i].textoBoton;

            // Se captura el índice en una variable local para que el closure lo capture correctamente;
            // sin esto, todos los botones llamarían siempre con el último valor de i
            int indice = i;
            botones[i].GetComponent<Button>().onClick.RemoveAllListeners();
            botones[i].GetComponent<Button>().onClick.AddListener(() => BotonPulsado(indice));
        }
    }

    public void BotonPulsado(int indiceElegido)
    {
        // Se deselecciona el botón del EventSystem para evitar que el teclado lo vuelva a pulsar
        EventSystem.current.SetSelectedGameObject(null);
        Respuesta respuestaElegida = nodoActual.respuestas[indiceElegido];

        amistadPam += respuestaElegida.cambioAmistad;

        GeneradorReportes.clicsClasicos++;
        Debug.Log($"[DIÁLOGO - {nodoActual.nombreNPC}] Jugador selecciona: \"{respuestaElegida.textoBoton}\". (Amistad actual: {amistadPam})");

        if (respuestaElegida.siguienteNodo != null)
        {
            MostrarNodo(respuestaElegida.siguienteNodo);
        }
        else
        {
            // Si la respuesta no apunta a ningún nodo siguiente, la conversación ha llegado a su fin
            EvaluarFinal();
        }
    }

    public void EvaluarFinal()
    {
        // El umbral de amistad positivo determina si Pam entrega el hacha o la deja clavada en el mapa
        if (amistadPam >= 1) { MostrarNodo(nodoFinalBueno); }
        else { MostrarNodo(nodoFinalMalo); }
    }

    public void CerrarDialogo()
    {
        cajaDialogo.SetActive(false);

        // Se restaura el estado del juego en el orden inverso al que se congeló
        if (relojDelJuego != null) relojDelJuego.elTiempoPasa = true;
        if (interfazInventario != null) interfazInventario.SetActive(true);
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = true;

        // Se invoca el callback y se limpia para que no persista entre conversaciones distintas
        if (eventoAlCerrar != null)
        {
            eventoAlCerrar.Invoke();
            eventoAlCerrar = null;
        }
    }

    public void ActivarHachaEnMapa()
    {
        GeneradorReportes.fracasosTotales++;
        Debug.Log($"[EVENTO] Resultado: FRACASO. El hacha se queda clavada en el mapa.");

        if (hachaFisicaEnMapa != null)
        {
            hachaFisicaEnMapa.SetActive(true);
        }
    }

    public void EntregarHachaAlJugador()
    {
        Debug.Log($"[EVENTO] Resultado: ÉXITO. Hacha entregada al jugador.");

        if (inventario != null && hachaParaEntregar != null)
        {
            inventario.AñadirObjeto(hachaParaEntregar);
        }
        tieneHacha = true;
    }

    public void AprenderSecreto()
    {
        conoceSecreto = true;
    }
}
