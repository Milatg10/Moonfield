using UnityEngine;
using UnityEngine.UI; // Para controlar las imágenes de la UI
using System.Collections.Generic; // Para usar Listas

public class SistemaInventario : MonoBehaviour
{
    [Header("Mis Objetos Guardados")]
    // Aquí el juego recordará qué objetos tienes
    public List<ObjetoInventario> mochila = new List<ObjetoInventario>();

    [Header("Conexión con la UI")]
    // Aquí arrastraremos las 10 imágenes (solo los iconos) de tus slots
    public Image[] iconosSlotsUI; 

    void Start()
    {
        // Al empezar el juego, nos aseguramos de que todos los iconos estén invisibles
        ActualizarUI();
    }

    // Esta es la función mágica que llamaremos cuando Pam nos dé el hacha
    public void AñadirObjeto(ObjetoInventario nuevoObjeto)
    {
        mochila.Add(nuevoObjeto);
        Debug.Log("Has conseguido: " + nuevoObjeto.nombreObjeto);
        ActualizarUI(); // Refrescamos la barra visual
    }

    // Esta función dibuja los objetos en los cuadraditos correspondientes
    void ActualizarUI()
    {
        // 1. Primero, hacemos invisibles todos los iconos de los 10 slots
        for (int i = 0; i < iconosSlotsUI.Length; i++)
        {
            iconosSlotsUI[i].color = new Color(1, 1, 1, 0); // Transparente (Alpha a 0)
            iconosSlotsUI[i].sprite = null;
        }

        // 2. Luego, encendemos solo los iconos de los objetos que tenemos en la mochila
        for (int i = 0; i < mochila.Count; i++)
        {
            // Solo si no nos hemos pasado del límite de 10 slots
            if (i < iconosSlotsUI.Length) 
            {
                iconosSlotsUI[i].sprite = mochila[i].icono;
                iconosSlotsUI[i].color = new Color(1, 1, 1, 1); // Visible (Alpha a 1)
            }
        }
    }
}