using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Gestiona la interacción con el tronco que bloquea el camino:
// si el jugador lleva el hacha lo rompe mediante una cinemática de fundido a negro,
// y si no lo tiene inicia un diálogo que le indica qué necesita.
public class TroncoInteractivo : MonoBehaviour
{
    [Header("Requisitos")]
    public ObjetoInventario hachaNecesaria;

    [Header("Diálogo de fallo")]
    public NodoDialogo dialogoTroncoBloqueado;

    [Header("Cinemática de romper")]
    public Image pantallaNegra;
    public float velocidadFade = 1.5f;

    [Header("Gestor de mensajes")]
    public PantallaIntro gestorPantallas;

    private ControladorDialogo controladorDialogo;
    private SistemaInventario inventarioDelJugador;
    // Evita que la corrutina se lance varias veces si el jugador sigue en contacto con el tronco
    private bool estaRompiendose = false;

    void Start()
    {
        controladorDialogo = FindObjectOfType<ControladorDialogo>();
        inventarioDelJugador = FindObjectOfType<SistemaInventario>();

        if (pantallaNegra != null)
        {
            Color c = pantallaNegra.color;
            c.a = 0f;
            pantallaNegra.color = c;
        }
    }

    void OnCollisionEnter2D(Collision2D choque)
    {
        if (choque.gameObject.CompareTag("Player") && !estaRompiendose)
        {
            if (inventarioDelJugador.mochila.Contains(hachaNecesaria))
            {
                Debug.Log($"[SISTEMA] Rompiendo el tronco con el hacha...");
                estaRompiendose = true;
                StartCoroutine(RutinaRomperTroncoWithFade());
            }
            else
            {
                // Se comprueba que no haya ya un diálogo abierto para no interrumpirlo
                if (controladorDialogo.cajaDialogo.activeSelf == false)
                    controladorDialogo.IniciarDialogo(dialogoTroncoBloqueado);
            }
        }
    }

    IEnumerator RutinaRomperTroncoWithFade()
    {
        if (controladorDialogo.scriptDelJugador != null)
            controladorDialogo.scriptDelJugador.puedeMoverse = false;

        // Primera fase: fundido a negro
        Color c = pantallaNegra.color;
        while (c.a < 1f)
        {
            c.a += Time.deltaTime * velocidadFade;
            pantallaNegra.color = c;
            yield return null;
        }

        // Con la pantalla negra cubriendo la escena se oculta el tronco de forma imperceptible
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(0.5f);

        // Segunda fase: fundido a transparente para revelar la escena sin el tronco
        while (c.a > 0f)
        {
            c.a -= Time.deltaTime * velocidadFade;
            pantallaNegra.color = c;
            yield return null;
        }

        // El movimiento se devuelve desde PantallaIntro al pulsar Enter, no aquí,
        // para que el jugador lea el mensaje narrativo antes de recuperar el control
        if (gestorPantallas != null)
        {
            string mensaje = "¡El camino está despejado!\n\nDebería buscar al viejo Michael en la plaza del pueblo para conseguir la contraseña del cofre de la playa (Quizás si le cuento algún chisme jugoso...)";
            gestorPantallas.MostrarNuevoMensaje(mensaje);
        }
        else
        {
            // Si gestorPantallas no está asignado en el inspector se devuelve el control
            // directamente para que el juego no quede bloqueado
            if (controladorDialogo.scriptDelJugador != null)
                controladorDialogo.scriptDelJugador.puedeMoverse = true;
        }

        gameObject.SetActive(false);
    }
}
