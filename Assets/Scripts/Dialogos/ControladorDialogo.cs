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
    public bool conoceSecreto = false; 

     void Start()
    {
        cajaDialogo.SetActive(false); 
    }

    public void IniciarDialogo(NodoDialogo primerNodo)
    {
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false; 
        if (interfazInventario != null) interfazInventario.SetActive(false); 
        if (relojDelJuego != null) relojDelJuego.elTiempoPasa = false;

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

        for (int i = 0; i < botones.Length; i++) { botones[i].SetActive(false); }

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
        if (amistadPam >= 1) { MostrarNodo(nodoFinalBueno); }
        else { MostrarNodo(nodoFinalMalo); }
    }

    public void CerrarDialogo()
    {
        // ¡AQUÍ COMPROBAMOS QUIÉN ESTÁ INTENTANDO CERRAR!
        Debug.Log($"🔴 1. [CONTROLADOR] Soy el Controlador llamado '{gameObject.name}' con el DNI: {gameObject.GetInstanceID()}. Cerrando diálogo...");
        Debug.Log($"🔴 1B. [CONTROLADOR] ¿Hay evento esperando en MI buzón? " + (eventoAlCerrar != null));
        
        cajaDialogo.SetActive(false); 

        if (relojDelJuego != null) relojDelJuego.elTiempoPasa = true;
        if (interfazInventario != null) interfazInventario.SetActive(true);
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = true;
        
        if (eventoAlCerrar != null)
        {
            Debug.Log("🔴 2. [CONTROLADOR] Ejecutando el evento guardado...");
            eventoAlCerrar.Invoke(); 
            eventoAlCerrar = null;   
        }
        else
        {
            Debug.LogWarning("🔴 [ATENCIÓN] El eventoAlCerrar estaba vacío. El NPC no mandó instrucciones.");
        }
    }

    public void ActivarHachaEnMapa()
    {
        Debug.Log("🪓 3. [HACHA] ¡Se acaba de llamar a ActivarHachaEnMapa!");
        
        if (hachaFisicaEnMapa != null)
        {
            hachaFisicaEnMapa.SetActive(true); 
            Debug.Log("🪓 4. [HACHA] ¡Hacha física encendida con éxito!");
        }
        else
        {
            Debug.LogError("🪓 [ERROR DE HACHA] La función se ejecutó, pero el hueco 'Hacha Fisica En Mapa' del Inspector está VACÍO.");
        }
    }

    public void EntregarHachaAlJugador()
    {
        if (inventario != null && hachaParaEntregar != null) 
        {
            inventario.AñadirObjeto(hachaParaEntregar);
            Debug.Log("Hacha añadida al inventario.");
        }
        tieneHacha = true; 
    }
    
    public void AprenderSecreto()
    {
        conoceSecreto = true;
        Debug.Log("¡El jugador ha aprendido la contraseña de Michael!");
    }
}