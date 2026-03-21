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

    // Esta función la llamaremos cuando el jugador se acerque a Pam
    public void IniciarDialogo(NodoDialogo primerNodo)
    {
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = false; // Bloqueamos el movimiento del jugador al iniciar el diálogo
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
        if (scriptDelJugador != null) scriptDelJugador.puedeMoverse = true; // Desbloqueamos el movimiento del jugador al cerrar el diálogo
        cajaDialogo.SetActive(false); // Apaga la UI
        
        // ¡Aquí en el futuro pondremos el código para que el hacha aparezca en tu inventario!
        if (amistadPam >= 1) {
            Debug.Log("¡Pam te ha dado el Hacha!"); 
        } else {
            Debug.Log("Pam se ha ido sin darte nada.");
        }

        // Si alguien nos dejó un recado, lo ejecutamos ahora
        if (eventoAlCerrar != null)
        {
            eventoAlCerrar.Invoke(); // ¡Ejecuta el recado!
            eventoAlCerrar = null; // Borramos el recado para el siguiente diálogo
        }
    }

    [Header("Prueba Rapida")]
    public NodoDialogo nodoDeArranque;

    void Update()
    {
        // Si pulsas la barra espaciadora y la caja de diálogo está apagada... ¡Arranca!
        if (Input.GetKeyDown(KeyCode.Space) && cajaDialogo.activeSelf == false)
        {
            IniciarDialogo(nodoDeArranque);
        }
    }
}