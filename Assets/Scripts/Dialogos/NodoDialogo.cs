using UnityEngine;

[CreateAssetMenu(fileName = "NuevoNodo", menuName = "Dialogos/Nodo de Dialogo")]
public class NodoDialogo : ScriptableObject
{
    [Header("Datos del NPC")]
    public string nombreNPC = "Pam"; // El nombre que saldrá
    public Color colorNombre = Color.white; // ¡Una paleta de color en Unity!

    [Header("Mensaje")]
    [TextArea(3, 10)]
    public string textoDelNPC;
    
    public Respuesta[] respuestas;
}

// Esta clase pequeñita guarda qué dice el botón y a qué nodo te lleva
[System.Serializable]
public class Respuesta
{
    public string textoBoton;
    public NodoDialogo siguienteNodo; // Si está vacío, se acaba la charla
    public int cambioAmistad; // +1, -1 o 0
}