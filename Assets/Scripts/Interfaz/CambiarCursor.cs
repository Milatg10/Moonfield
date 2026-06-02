using UnityEngine;
using UnityEngine.EventSystems; // Necesario para detectar el ratón en la UI

// Le decimos a Unity que este script va a usar los eventos de "Entrar" y "Salir" del ratón
public class CambiarCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración del Cursor")]
    public Texture2D cursorMano; // Aquí pondremos la imagen de la mano
    
    // El "Hotspot" es el píxel exacto que hace clic (por defecto la esquina superior izquierda: 0,0)
    public Vector2 puntoDeClic = Vector2.zero; 

    // Cuando la flecha del ratón ENTRA en el botón...
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Cambiamos el cursor a la mano
        Cursor.SetCursor(cursorMano, puntoDeClic, CursorMode.Auto);
    }

    // Cuando la flecha del ratón SALE del botón...
    public void OnPointerExit(PointerEventData eventData)
    {
        // Ponemos "null" para que vuelva a la flecha normal del ordenador
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    // ¡SÚPER IMPORTANTE! Si el botón desaparece mientras el ratón está encima, 
    // nos aseguramos de devolver la flecha normal para que no se quede la mano atascada.
    void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}