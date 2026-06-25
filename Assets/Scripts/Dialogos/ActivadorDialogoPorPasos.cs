using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Activa la conversación con Pam cuando el jugador entra en la zona de trigger,
// encadenando el diálogo (o la respuesta de la IA) con una cinemática de fundido a negro
// que oculta al personaje y devuelve el control al jugador al terminar.
public class ActivadorDialogoPorPasos : MonoBehaviour
{
    [Header("Configuración del Diálogo")]
    public NodoDialogo dialogoDePam;
    public GameObject cuerpoDePam;

    [Header("CONEXIÓN CON LA IA")]
    public NPCController cerebroDePam;

    [Header("Efecto Cinemática")]
    public Image pantallaNegra;
    public float velocidadFade = 1.5f;

    private ControladorDialogo controlador;
    public bool yaHaHablado = false;

    void Start()
    {
        controlador = FindObjectOfType<ControladorDialogo>();

        // La pantalla negra comienza completamente transparente para no tapar la escena al iniciar
        if (pantallaNegra != null)
        {
            Color c = pantallaNegra.color;
            c.a = 0f;
            pantallaNegra.color = c;
        }
    }

    void OnTriggerEnter2D(Collider2D otroObjeto)
    {
        // La bandera yaHaHablado evita que la secuencia se dispare más de una vez
        if (otroObjeto.CompareTag("Player") && !yaHaHablado)
        {
            yaHaHablado = true;

            if (cerebroDePam != null)
            {
                // Se llama a Hablar() para que el NPCController procese las reglas narrativas
                // antes de mostrar el diálogo, garantizando que el contenido esté actualizado
                cerebroDePam.Hablar();

                // La cinemática de despedida se suscribe como callback para ejecutarse
                // justo cuando el diálogo termina, sin interrumpirlo
                if (cerebroDePam.modoIAActivo)
                {
                    cerebroDePam.eventoAlCerrarIA += IniciarDespedida;
                }
                else
                {
                    // En modo sin IA el evento de cierre pertenece al controlador de diálogo estándar
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

        // Primera fase: fundido de transparente a negro
        while (c.a < 1f)
        {
            c.a += Time.deltaTime * velocidadFade;
            pantallaNegra.color = c;
            yield return null;
        }

        // Con la pantalla negra cubriendo la escena se oculta el personaje de forma imperceptible
        if (cuerpoDePam != null) cuerpoDePam.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        // Segunda fase: fundido de negro a transparente para revelar la escena sin Pam
        while (c.a > 0f)
        {
            c.a -= Time.deltaTime * velocidadFade;
            pantallaNegra.color = c;
            yield return null;
        }

        // Al terminar la cinemática se reactiva el inventario y se devuelve el control al jugador
        if (controlador.interfazInventario != null) controlador.interfazInventario.SetActive(true);
        if (controlador.scriptDelJugador != null) controlador.scriptDelJugador.puedeMoverse = true;
    }
}
