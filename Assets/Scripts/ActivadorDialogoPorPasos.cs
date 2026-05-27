using UnityEngine;
using UnityEngine.UI; 
using System.Collections; 

public class ActivadorDialogoPorPasos : MonoBehaviour
{
    [Header("Configuración del Diálogo")]
    public NodoDialogo dialogoDePam; 
    public GameObject cuerpoDePam; 
    
    [Header("=== CONEXIÓN CON LA IA ===")]
    public NPCController cerebroDePam; 

    [Header("Efecto Cinemática")]
    public Image pantallaNegra; 
    public float velocidadFade = 1.5f; 

    private ControladorDialogo controlador;
    public bool yaHaHablado = false; 

    void Start()
    {
        controlador = FindObjectOfType<ControladorDialogo>();
        
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
            yaHaHablado = true;

            if (cerebroDePam != null && cerebroDePam.modoIAActivo)
            {
                // ---- MODO IA ----
                // Le dejamos el recado al nuevo Cerebro IA
                cerebroDePam.eventoAlCerrarIA += IniciarDespedida;
                cerebroDePam.Hablar(); 
            }
            else
            {
                // ---- MODO CLÁSICO ----
                // Le dejamos el recado al Controlador Clásico antiguo
                controlador.eventoAlCerrar += IniciarDespedida;
                controlador.IniciarDialogo(dialogoDePam);
            }
        }
    }

    // Esta función la haremos pública más adelante para que la IA también pueda lanzar la película
    public void IniciarDespedida()
    {
        StartCoroutine(RutinaFundidoANegro()); 
    }

    IEnumerator RutinaFundidoANegro()
    {
        Color c = pantallaNegra.color;

        while (c.a < 1f)
        {
            c.a += Time.deltaTime * velocidadFade;
            pantallaNegra.color = c;
            yield return null; 
        }

        cuerpoDePam.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        while (c.a > 0f)
        {
            c.a -= Time.deltaTime * velocidadFade;
            pantallaNegra.color = c;
            yield return null;
        }

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