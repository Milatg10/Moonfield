using UnityEngine;
using UnityEngine.UI; 
using System.Collections; 

public class TroncoInteractivo : MonoBehaviour
{
    [Header("Requisitos")]
    public ObjetoInventario hachaNecesaria; 

    [Header("Diálogo de Fallo")]
    public NodoDialogo dialogoTroncoBloqueado; 

    [Header("Cinemática de Romper (Fading)")]
    public Image pantallaNegra; 
    public float velocidadFade = 1.5f;

    [Header("=== GESTOR DE MENSAJES ===")]
    public PantallaIntro gestorPantallas; 

    // Cerebros automáticos
    private ControladorDialogo controladorDialogo;
    private SistemaInventario inventarioDelJugador;
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
                Debug.Log("¡Arrancando cinemática de cortar tronco!");
                estaRompiendose = true; 
                StartCoroutine(RutinaRomperTroncoWithFade()); 
            }
            else
            {
                if (controladorDialogo.cajaDialogo.activeSelf == false)
                {
                    controladorDialogo.IniciarDialogo(dialogoTroncoBloqueado);
                }
            }
        }
    }

    IEnumerator RutinaRomperTroncoWithFade()
    {
        // 1. Congelamos al jugador e interfaz INMEDIATAMENTE
        if (controladorDialogo.scriptDelJugador != null)
        {
            controladorDialogo.scriptDelJugador.puedeMoverse = false;
        }
        
        // 2. Oscurecer la pantalla poco a poco hasta que esté negra
        Color c = pantallaNegra.color;
        while (c.a < 1f)
        {
            c.a += Time.deltaTime * velocidadFade;
            pantallaNegra.color = c;
            yield return null; 
        }

        // 3. Hacemos el cambiazo
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        
        yield return new WaitForSeconds(0.5f);

        // 4. Volver a aclarar la pantalla poco a poco
        while (c.a > 0f)
        {
            c.a -= Time.deltaTime * velocidadFade;
            pantallaNegra.color = c;
            yield return null;
        }

        // En lugar de devolver el movimiento, lanzamos el mensaje en pantalla.
        // El propio script PantallaIntro se encargará de devolverle el movimiento al jugador al pulsar Enter.
        if (gestorPantallas != null)
        {
            string mensaje = "¡El camino está despejado!\n\nDebería buscar al viejo Michael en la plaza del pueblo para averiguar la contraseña del cofre.";
            gestorPantallas.MostrarNuevoMensaje(mensaje);
        }
        else 
        {
            // Red de seguridad: si se te olvida arrastrar la PantallaIntro al Inspector, devolvemos el control para que el juego no se rompa
            if (controladorDialogo.scriptDelJugador != null)
                controladorDialogo.scriptDelJugador.puedeMoverse = true;
        }

        // 6. Apagamos y destruimos el tronco
        gameObject.SetActive(false);
    }
}