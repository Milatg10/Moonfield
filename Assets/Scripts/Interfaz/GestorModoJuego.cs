using UnityEngine;

public class GestorModoJuego : MonoBehaviour
{
    void Start()
    {
        // Leemos la memoria. Si no hay nada, por defecto será 0 (Clásico)
        int modoGuardado = PlayerPrefs.GetInt("ModoIA", 0);
        bool activarIA = (modoGuardado == 1);

        // Buscamos a TODOS los NPCs del mapa automáticamente
        NPCController[] todosLosNPCs = FindObjectsOfType<NPCController>();

        foreach (NPCController npc in todosLosNPCs)
        {
            npc.modoIAActivo = activarIA;
        }

        Debug.Log("🎮 El juego ha arrancado. ¿Modo IA activado?: " + activarIA);
    }
}