using UnityEngine;

public class RecogerObjeto : MonoBehaviour
{
    [Header("=== QUÉ OBJETO ES ===")]
    [Tooltip("Arrastra aquí la cajita ScriptableObject de este objeto (ej. el Hacha)")]
    public ObjetoInventario datosDelObjeto; 

    [Header("=== CONEXIÓN CON EL SISTEMA ===")]
    public SistemaInventario inventario; // El gestor de la mochila

    // Detectamos cuando alguien entra en la zona invisible (lo pisa)
    private void OnTriggerEnter2D(Collider2D otroObjeto)
    {
        // Comprobamos si el que lo ha pisado es el jugador
        if (otroObjeto.CompareTag("Player"))
        {
            // 1. Lo metemos en la mochila al instante
            if (inventario != null && datosDelObjeto != null)
            {
                inventario.AñadirObjeto(datosDelObjeto);
                Debug.Log("¡Has pisado y recogido: " + datosDelObjeto.nombreObjeto + "!");
            }
            else
            {
                Debug.LogWarning("⚠️ Falta asignar el Inventario o los Datos del Objeto en el Inspector.");
            }

            // 2. Lo hacemos desaparecer del mapa
            gameObject.SetActive(false); 
        }
    }
}