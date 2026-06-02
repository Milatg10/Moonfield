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

            if (cerebroDePam != null)
            {
                // 1. OBLIGAMOS A PASAR POR EL CEREBRO
                // Esto hace que el NPCController meta las reglas del hacha en el buzón.
                cerebroDePam.Hablar(); 

                // 2. AÑADIMOS LA CINEMÁTICA AL FINAL DE LA COLA
                if (cerebroDePam.modoIAActivo)
                {
                    cerebroDePam.eventoAlCerrarIA += IniciarDespedida;
                }
                else
                {
                    // Con el "+=" sumamos el fundido a negro a las instrucciones del hacha,
                    // así hace las dos cosas sin pisarse.
                    controlador.eventoAlCerrar += IniciarDespedida;
                }
            }
        }
    }

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

        if (cuerpoDePam != null) cuerpoDePam.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        while (c.a > 0f)
        {
            c.a -= Time.deltaTime * velocidadFade;
            pantallaNegra.color = c;
            yield return null;
        }

        if (controlador.interfazInventario != null) controlador.interfazInventario.SetActive(true);
        if (controlador.scriptDelJugador != null) controlador.scriptDelJugador.puedeMoverse = true;
    }
}