using UnityEngine;
using UnityEngine.EventSystems;

// Sustituye el cursor del sistema por una imagen personalizada al pasar el ratón sobre un elemento de UI,
// y lo restaura al salir o al desactivarse el objeto.
public class CambiarCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración del Cursor")]
    public Texture2D cursorMano;

    // El hotspot define qué píxel de la textura actúa como punto de clic; (0,0) es la esquina superior izquierda
    public Vector2 puntoDeClic = Vector2.zero;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(cursorMano, puntoDeClic, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    // Si el objeto se desactiva mientras el cursor está encima, OnPointerExit no se llega a disparar,
    // por lo que es necesario restaurar el cursor aquí para evitar que quede atascado con la imagen de la mano
    void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
