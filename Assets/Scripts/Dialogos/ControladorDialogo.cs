using UnityEngine;
using TMPro; // Para poder usar los textos modernos
using UnityEngine.UI; // Para poder usar botones
using UnityEngine.EventSystems;
using System; // Nos permite usar "Actions" (Recados)

public class ControladorDialogo : MonoBehaviour
{
    [Header("Elementos Visuales (Arrastrar desde el Canvas)")]
    public GameObject cajaDialogo; // El Panel oscuro
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoMensaje;
    public GameObject[] botones; // Arrastrar los 3 objetos Botón aquí
    public TextMeshProUGUI[] textosBotones; // Arrastrar los textos de dentro de los botones

    [Header("Datos Internos")]
    public int amistadPam = 0;
    private NodoDialogo nodoActual;

    [Header("Nodos Finales (Evaluación)")]
    public NodoDialogo nodoFinalBueno; // Aquí arrastraremos Pam_03A_Hacha
    public NodoDialogo nodoFinalMalo;  // Aquí arrastraremos Pam_03B_Nada

    [HideInInspector] // Lo ocultamos para que no ensucie Unity
    public Action eventoAlCerrar;

    [Header("Referencias")]
    // ¡OJO! Cambia "MovimientoJugador" por el nombre real de tu script de mover al personaje
    public Movement scriptDelJugador;

    public SistemaInventario inventario; // Inventario
    public ObjetoInventario hachaParaEntregar; // El objeto que le daremos al jugador si la amistad es buena

    [Header("Referencias UI Extras")]
    public GameObject interfazInventario; // Aquí arrastraremos la barra completa con su fondo

    [Header("Conexión con el Reloj")]
    public RelojJuego relojDelJuego; // Para poder pausarlo

    // Esta función la llamaremos cuando el jugador se acerque a Pam
    public void IniciarDialogo(NodoDialogo primerNodo)
    {
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false; // Bloqueamos el movimiento del jugador al iniciar el diálogo

        if (interfazInventario != null) interfazInventario.SetActive(false); // Esconde la barra de inventario durante el diálogo

        // Pausamos el tiempo mientras hablamos
        if (relojDelJuego != null)
        {
            relojDelJuego.elTiempoPasa = false;
        }

        amistadPam = 0; // Empezamos de cero
        cajaDialogo.SetActive(true); // Encendemos la UI
        MostrarNodo(primerNodo);
    }

    public void MostrarNodo(NodoDialogo nodo)
    {
        nodoActual = nodo;
        textoNombre.text = nodo.nombreNPC;
        textoNombre.color = nodo.colorNombre;
        textoMensaje.text = nodo.textoDelNPC;

        // Primero, apagamos todos los botones por si acaso
        for (int i = 0; i < botones.Length; i++)
        {
            botones[i].SetActive(false);
        }

        // Si es un NODO FINAL (como el 03A o 03B, que tienen 0 respuestas)
        if (nodo.respuestas.Length == 0)
        {
            botones[0].SetActive(true); // Encendemos solo el primer botón
            textosBotones[0].text = "Cerrar"; // Le cambiamos el texto
            
            // Le decimos al botón que al pulsarlo, cierre la caja
            botones[0].GetComponent<Button>().onClick.RemoveAllListeners();
            botones[0].GetComponent<Button>().onClick.AddListener(CerrarDialogo);
            return; // Terminamos aquí
        }

        // Si es un NODO NORMAL, encendemos los botones que hagan falta
        for (int i = 0; i < nodo.respuestas.Length; i++)
        {
            botones[i].SetActive(true);
            textosBotones[i].text = nodo.respuestas[i].textoBoton;
            
            // Este bloque de código le dice al botón qué hacer al ser pulsado
            int indice = i; 
            botones[i].GetComponent<Button>().onClick.RemoveAllListeners();
            botones[i].GetComponent<Button>().onClick.AddListener(() => BotonPulsado(indice));
        }
    }

    public void BotonPulsado(int indiceElegido)
    {
        EventSystem.current.SetSelectedGameObject(null); // Esto es para que el botón no se quede "seleccionado" después de pulsarlo
        Respuesta respuestaElegida = nodoActual.respuestas[indiceElegido];
        
        // 1. Sumamos (o restamos) la amistad en secreto
        amistadPam += respuestaElegida.cambioAmistad;

        // 2. ¿A dónde vamos ahora?
        if (respuestaElegida.siguienteNodo != null) // Si hay un nodo conectado...
        {
            MostrarNodo(respuestaElegida.siguienteNodo);
        }
        else // ¡SI ESTÁ EN NONE, ES HORA DE EVALUAR!
        {
            EvaluarFinal();
        }
    }

    public void EvaluarFinal()
    {
        if (amistadPam >= 1)
        {
            MostrarNodo(nodoFinalBueno); // ¡Enseña la tarjeta de darle el hacha!
        }
        else
        {
            MostrarNodo(nodoFinalMalo); // ¡Enseña la tarjeta de mandarle a pastar!
        }
    }

    public void CerrarDialogo()
    {
        cajaDialogo.SetActive(false); // Apaga la UI

        // Reanudamos el tiempo SIEMPRE que se cierre la caja, sin importar lo que pase después
        if (relojDelJuego != null)
        {
            relojDelJuego.elTiempoPasa = true;
        }
        
        // Damos el hacha en secreto 
        if (amistadPam >= 1) {
            if (inventario != null && hachaParaEntregar != null) {
                inventario.AñadirObjeto(hachaParaEntregar);
            }
        }

        // ¿Alguien nos dejó un recado para hacer una película (como la puerta)?
        if (eventoAlCerrar != null)
        {
            eventoAlCerrar.Invoke(); // Ejecutamos la película 
            eventoAlCerrar = null;   // Borramos el recado
        }
        else 
        {
            // ¡NO hay película! Devolvemos el control inmediatamente al jugador
            if (interfazInventario != null) interfazInventario.SetActive(true);
            if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = true;
        }
    }

}