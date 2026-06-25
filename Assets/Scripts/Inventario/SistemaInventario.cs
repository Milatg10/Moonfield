using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Gestiona el inventario del jugador: almacena los objetos recogidos y
// sincroniza su representación visual con los slots de la barra de UI.
public class SistemaInventario : MonoBehaviour
{
    [Header("Objetos Guardados")]
    public List<ObjetoInventario> mochila = new List<ObjetoInventario>();

    [Header("Conexión con la UI")]
    public Image[] iconosSlotsUI;

    void Start()
    {
        ActualizarUI();
    }

    public void AñadirObjeto(ObjetoInventario nuevoObjeto)
    {
        mochila.Add(nuevoObjeto);
        Debug.Log($"[SISTEMA] Has conseguido: {nuevoObjeto.nombreObjeto}");
        ActualizarUI();
    }

    void ActualizarUI()
    {
        // Se resetean todos los slots a transparente para partir de un estado limpio
        for (int i = 0; i < iconosSlotsUI.Length; i++)
        {
            iconosSlotsUI[i].color = new Color(1, 1, 1, 0);
            iconosSlotsUI[i].sprite = null;
        }

        // Se pintan únicamente los slots que corresponden a objetos en la mochila;
        // el guard i < iconosSlotsUI.Length evita un desbordamiento si la mochila
        // tuviera más objetos que slots disponibles en la UI
        for (int i = 0; i < mochila.Count; i++)
        {
            if (i < iconosSlotsUI.Length)
            {
                iconosSlotsUI[i].sprite = mochila[i].icono;
                iconosSlotsUI[i].color = new Color(1, 1, 1, 1);
            }
        }
    }
}
