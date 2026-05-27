using UnityEngine;
using TMPro; 
using UnityEngine.UI; 
using UnityEngine.EventSystems;
using System; 

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
    
    // ¡NUEVO! La bandera que le dirá al tronco que ya puede romperse
    public bool tieneHacha = false; 

    [Header("Nodos Finales (Evaluación)")]
    public NodoDialogo nodoFinalBueno; 
    public NodoDialogo nodoFinalMalo;  

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

    [Header("=== CASTIGO DEL HACHA ===")]
    public GameObject hachaFisicaEnMapa; 

     void Start()
    {
        cajaDialogo.SetActive(false); 
    }

    public void IniciarDialogo(NodoDialogo primerNodo)
    {
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false; 
        if (interfazInventario != null) interfazInventario.SetActive(false); 

        if (relojDelJuego != null)
        {
            relojDelJuego.elTiempoPasa = false;
        }

        amistadPam = 0; 
        cajaDialogo.SetActive(true); 
        MostrarNodo(primerNodo);
    }

    public void MostrarNodo(NodoDialogo nodo)
    {
        nodoActual = nodo;
        textoNombre.text = nodo.nombreNPC;
        textoNombre.color = nodo.colorNombre;
        textoMensaje.text = nodo.textoDelNPC;

        for (int i = 0; i < botones.Length; i++)
        {
            botones[i].SetActive(false);
        }

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
            
            int indice = i; 
            botones[i].GetComponent<Button>().onClick.RemoveAllListeners();
            botones[i].GetComponent<Button>().onClick.AddListener(() => BotonPulsado(indice));
        }
    }

    public void BotonPulsado(int indiceElegido)
    {
        EventSystem.current.SetSelectedGameObject(null); 
        Respuesta respuestaElegida = nodoActual.respuestas[indiceElegido];
        
        amistadPam += respuestaElegida.cambioAmistad;

        if (respuestaElegida.siguienteNodo != null) 
        {
            MostrarNodo(respuestaElegida.siguienteNodo);
        }
        else 
        {
            EvaluarFinal();
        }
    }

    public void EvaluarFinal()
    {
        if (amistadPam >= 1)
        {
            MostrarNodo(nodoFinalBueno); 
        }
        else
        {
            MostrarNodo(nodoFinalMalo); 
        }
    }

    public void ActivarHachaEnMapa()
    {
        if (hachaFisicaEnMapa != null)
        {
            hachaFisicaEnMapa.SetActive(true); 
            Debug.Log("Pam se ha enfadado. El hacha física ha aparecido en el mapa.");
        }
    }

    public void CerrarDialogo()
    {
        cajaDialogo.SetActive(false); 

        if (relojDelJuego != null)
        {
            relojDelJuego.elTiempoPasa = true;
        }
        
        // ¡NUEVO! Usamos la función para que quede súper limpio
        if (amistadPam >= 1) 
        {
            EntregarHachaAlJugador(); 
        }
        else 
        {
            ActivarHachaEnMapa();
        }

        if (eventoAlCerrar != null)
        {
            eventoAlCerrar.Invoke(); 
            eventoAlCerrar = null;   
        }
        else 
        {
            if (interfazInventario != null) interfazInventario.SetActive(true);
            if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = true;
        }
    }

    // ==========================================
    // LLAMARÁ LA IA (O EL MODO CLÁSICO)
    // ==========================================
    public void EntregarHachaAlJugador()
    {
        if (inventario != null && hachaParaEntregar != null) 
        {
            inventario.AñadirObjeto(hachaParaEntregar);
            Debug.Log("Hacha añadida al inventario.");
        }

        tieneHacha = true; // El tronco ya puede leer esto y dejarte pasar
    }
}