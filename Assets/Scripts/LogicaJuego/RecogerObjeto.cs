using UnityEngine;

// Añade el objeto al inventario del jugador al entrar en contacto con él
// y desactiva el GameObject del mapa para simular que ha sido recogido.
public class RecogerObjeto : MonoBehaviour
{
    [Header("Qué objeto es")]
    [Tooltip("Arrastra aquí la cajita ScriptableObject de este objeto")]
    public ObjetoInventario datosDelObjeto;

    [Header("Conexión con el sistema")]
    public SistemaInventario inventario;

    private void OnTriggerEnter2D(Collider2D otroObjeto)
    {
        if (otroObjeto.CompareTag("Player"))
        {
            if (inventario != null && datosDelObjeto != null)
            {
                inventario.AñadirObjeto(datosDelObjeto);
                Debug.Log($"[SISTEMA] ¡Has pisado y recogido: {datosDelObjeto.nombreObjeto}!");
            }
            else
            {
                Debug.LogWarning("[SISTEMA] No has asignado el inventario o los datos del objeto en el inspector.");
            }

            // Se desactiva en lugar de destruir para preservar referencias del inspector en caliente
            gameObject.SetActive(false);
        }
    }
}
