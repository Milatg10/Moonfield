using UnityEngine;

// Reduce la opacidad del sprite del objeto padre cuando el jugador entra en su trigger,
// permitiendo ver al personaje cuando está detrás de él. Se busca el SpriteRenderer
// en el padre porque el trigger hijo actúa como zona de detección independiente.
public class CasaTransparente : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
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
