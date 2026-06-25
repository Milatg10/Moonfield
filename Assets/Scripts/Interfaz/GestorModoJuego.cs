using UnityEngine;

// Lee la preferencia de modo de juego guardada en la escena de menú y la propaga
// a todos los NPCController presentes en la escena al iniciar la partida.
public class GestorModoJuego : MonoBehaviour
{
    void Start()
    {
        // El valor por defecto 0 corresponde al modo árbol clásico, que se usa si el jugador
        // nunca ha guardado una preferencia (primera ejecución o PlayerPrefs borradas)
        int modoGuardado = PlayerPrefs.GetInt("ModoIA", 0);
        bool activarIA = (modoGuardado == 1);

        NPCController[] todosLosNPCs = FindObjectsOfType<NPCController>();

        foreach (NPCController npc in todosLosNPCs)
        {
            npc.modoIAActivo = activarIA;
        }

        string textoModo = activarIA ? "MODO IA" : "MODO ÁRBOL CLÁSICO";
        Debug.Log($"[SISTEMA] Partida cargada con éxito. Configuración actual: {textoModo}");
    }
}
