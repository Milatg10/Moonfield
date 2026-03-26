using UnityEngine;

public class TroncoInteractivo : MonoBehaviour
{
    [Header("Requisitos")]
    public ObjetoInventario hachaNecesaria; // Aquí arrastraremos la tarjeta del Hacha

    [Header("Diálogo de Fallo")]
    public NodoDialogo dialogoTroncoBloqueado; // La tarjeta que acabamos de crear

    // Cerebros automáticos
    private ControladorDialogo controladorDialogo;
    private SistemaInventario inventarioDelJugador;

    void Start()
    {
        // Buscamos los sistemas automáticamente para no tener que arrastrarlos a mano
        controladorDialogo = FindObjectOfType<ControladorDialogo>();
        inventarioDelJugador = FindObjectOfType<SistemaInventario>();
    }

    // Usamos OnCollisionEnter2D porque el tronco es una PARED SÓLIDA
    void OnCollisionEnter2D(Collision2D choche)
    {
        // Comprobamos si el que se ha chocado contra el tronco es el jugador
        if (choche.gameObject.CompareTag("Player"))
        {
            // 1. ¿Tenemos el hacha en la mochila?
            if (inventarioDelJugador.mochila.Contains(hachaNecesaria))
            {
                // ¡Tenemos el hacha! Rompemos el tronco
                Debug.Log("¡Has cortado el tronco!");
                
                // Hacemos que el tronco desaparezca
                gameObject.SetActive(false); 
                
                // (Opcional en el futuro: aquí podemos añadir un sonido de "¡Chop!" o estrellitas)
            }
            else
            {
                // No tenemos el hacha. Lanzamos el diálogo de error.
                // Comprobamos que la caja esté apagada para no abrir el diálogo 20 veces si te quedas pegado
                if (controladorDialogo.cajaDialogo.activeSelf == false)
                {
                    controladorDialogo.IniciarDialogo(dialogoTroncoBloqueado);
                }
            }
        }
    }
}