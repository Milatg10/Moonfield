using UnityEngine;

public class MusicaFondo : MonoBehaviour
{
    private static MusicaFondo instancia;

    void Awake()
    {
        // Si no hay música sonando, yo soy la música
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject); // ¡El escudo de la inmortalidad!
        }
        else
        {
            // Si ya hay una música sonando y vuelves al menú, destruye esta copia nueva
            Destroy(gameObject);
        }
    }
}