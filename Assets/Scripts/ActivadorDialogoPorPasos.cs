using UnityEngine;
using UnityEngine.UI; // ¡NUEVO! Necesario para manejar la imagen negra
using System.Collections; // ¡NUEVO! Necesario para las Corrutinas (el tiempo)

public class ActivadorDialogoPorPasos : MonoBehaviour
{
    [Header("Configuración del Diálogo")]
    public NodoDialogo dialogoDePam; 
    public GameObject cuerpoDePam; 
    
    [Header("Efecto Cinemática")]
    public Image pantallaNegra; // Aquí arrastraremos nuestro panel negro
    public float velocidadFade = 1.5f; // Lo rápido que se oscurece

    private ControladorDialogo controlador;
    public bool yaHaHablado = false; 

    void Start()
    {
        controlador = FindObjectOfType<ControladorDialogo>();
        
        // Nos aseguramos de que la pantalla empiece transparente por si acaso
        if (pantallaNegra != null)
        {
            Color c = pantallaNegra.color;
            c.a = 0f;
            pantallaNegra.color = c;
        }
    }

    void OnTriggerEnter2D(Collider2D otroObjeto)
    {
        if (otroObjeto.CompareTag("Player") && !yaHaHablado)
        {
            // Le dejamos el recado al cerebro de que llame a IniciarDespedida
            controlador.eventoAlCerrar += IniciarDespedida;
            controlador.IniciarDialogo(dialogoDePam);
            yaHaHablado = true;
        }
    }

    // El cerebro llama a esta función al cerrar el diálogo
    void IniciarDespedida()
    {
        // StartCoroutine arranca una función que puede durar varios segundos
        StartCoroutine(RutinaFundidoANegro()); 
    }

    // Esta es la "película" que se reproduce paso a paso
    IEnumerator RutinaFundidoANegro()
    {
        Color c = pantallaNegra.color;

        // 1. Oscurecer la pantalla poco a poco
        while (c.a < 1f)
        {
            c.a += Time.deltaTime * velocidadFade;
            pantallaNegra.color = c;
            yield return null; 
        }

        // 2. Cambiazo en la oscuridad
        cuerpoDePam.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        // 3. Volver a aclarar la pantalla
        while (c.a > 0f)
        {
            c.a -= Time.deltaTime * velocidadFade;
            pantallaNegra.color = c;
            yield return null;
        }

        // 4. ¡Fin de la película! Ahora SÍ devolvemos el inventario y el movimiento
        if (controlador.interfazInventario != null)
        {
            controlador.interfazInventario.SetActive(true);
        }
        
        if (controlador.scriptDelJugador != null)
        {
            controlador.scriptDelJugador.puedeMoverse = true;
        }
    }
}