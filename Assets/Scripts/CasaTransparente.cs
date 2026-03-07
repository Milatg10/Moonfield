using UnityEngine;

public class CasaTransparente : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Al empezar, guardamos el componente que dibuja la casa
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Cuando alguien entra en la zona invisible...
    void OnTriggerEnter2D(Collider2D otro)
    {
        // Comprobamos si el que ha entrado tiene la etiqueta "Player"
        if (otro.CompareTag("Player"))
        {
            // Cogemos el color actual de la casa
            Color colorCasa = spriteRenderer.color;
            // Le bajamos la opacidad a la mitad (0.5f)
            colorCasa.a = 0.5f; 
            spriteRenderer.color = colorCasa;
        }
    }

    // Cuando alguien sale de la zona invisible...
    void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            // Volvemos a poner la opacidad al máximo (1f = 100% sólido)
            Color colorCasa = spriteRenderer.color;
            colorCasa.a = 1f; 
            spriteRenderer.color = colorCasa;
        }
    }
}