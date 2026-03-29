using UnityEngine;
using UnityEngine.UI; // ¡NUEVO! Para manejar la imagen negra
using System.Collections; // ¡NUEVO! Para las corrutinas

public class TroncoInteractivo : MonoBehaviour
{
    [Header("Requisitos")]
    public ObjetoInventario hachaNecesaria; // Aquí arrastraremos la tarjeta del Hacha

    [Header("Diálogo de Fallo")]
    public NodoDialogo dialogoTroncoBloqueado; // La tarjeta que ya creamos

    [Header("Cinemática de Romper (Fading)")]
    // ¡NUEVO! Arrastra aquí tu Imagen Negra del Canvas
    public Image pantallaNegra; 
    public float velocidadFade = 1.5f;

    // Cerebros automáticos
    private ControladorDialogo controladorDialogo;
    private SistemaInventario inventarioDelJugador;
    private bool estaRompiendose = false; // Bandera para que no empiece la corrutina 20 veces

    void Start()
    {
        // Buscamos los sistemas automáticamente
        controladorDialogo = FindObjectOfType<ControladorDialogo>();
        inventarioDelJugador = FindObjectOfType<SistemaInventario>();
        
        // Aseguramos que la pantalla empiece transparente
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
            // ¿Tenemos el hacha en la mochila?
            if (inventarioDelJugador.mochila.Contains(hachaNecesaria))
            {
                // ¡Tenemos el hacha! Arrancamos la película de romper
                Debug.Log("¡Arrancando cinemática de cortar tronco!");
                estaRompiendose = true; // Marcamos que ya está en proceso
                StartCoroutine(RutinaRomperTroncoWithFade()); 
            }
            else
            {
                // No tenemos el hacha. Lanzamos el diálogo de error.
                if (controladorDialogo.cajaDialogo.activeSelf == false)
                {
                    controladorDialogo.IniciarDialogo(dialogoTroncoBloqueado);
                }
            }
        }
    }

    // Esta es la corrutina que maneja el tiempo y la pantalla negra
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

        // 3. Hacemos el cambiazo: Apagamos el DIBUJO y la FÍSICA, pero NO el objeto entero
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        // ------------------------------
        
        // Esperamos medio segundo en negro
        yield return new WaitForSeconds(0.5f);

        // 4. Volver a aclarar la pantalla poco a poco
        while (c.a > 0f)
        {
            c.a -= Time.deltaTime * velocidadFade;
            pantallaNegra.color = c;
            yield return null;
        }

        // 5. ¡Fin de la película! Devolvemos el control
        if (controladorDialogo.scriptDelJugador != null)
        {
            controladorDialogo.scriptDelJugador.puedeMoverse = true;
        }

        // 6. Ahora que la película ha terminado, SÍ podemos apagar y destruir el objeto entero
        gameObject.SetActive(false);
    }
}