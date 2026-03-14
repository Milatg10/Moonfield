using UnityEngine;

public class CasaTransparente : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Obtener el SpriteRenderer del padre (la casa)
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            Color colorCasa = spriteRenderer.color;
            colorCasa.a = 0.5f; 
            spriteRenderer.color = colorCasa;
        }
    }

    void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            Color colorCasa = spriteRenderer.color;
            colorCasa.a = 1f; 
            spriteRenderer.color = colorCasa;
        }
    }
}