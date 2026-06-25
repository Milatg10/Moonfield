using UnityEngine;

// Implementa el patrón Singleton para mantener un único AudioSource de música
// continua entre cambios de escena. Si al cargar una escena ya existe una instancia
// previa, la nueva se destruye para evitar que la música se superponga.
public class MusicaFondo : MonoBehaviour
{
    private static MusicaFondo instancia;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
