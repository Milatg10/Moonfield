using UnityEngine;

// ScriptableObject que representa un nodo del árbol de diálogo.
// Cada nodo contiene el texto que muestra el NPC y las respuestas disponibles para el jugador.
// Se crea desde el menú de Unity como un asset reutilizable y editable sin tocar código.
[CreateAssetMenu(fileName = "NuevoNodo", menuName = "Dialogos/Nodo de Dialogo")]
public class NodoDialogo : ScriptableObject
{
    [Header("Datos del NPC")]
    public string nombreNPC = "Pam";
    public Color colorNombre = Color.white;

    [Header("Mensaje")]
    [TextArea(3, 10)]
    public string textoDelNPC;

    // Si el array está vacío, el ControladorDialogo interpreta este nodo como terminal
    public Respuesta[] respuestas;
}

// Cada respuesta conecta un botón de diálogo con el siguiente nodo y con un cambio en la puntuación de amistad
[System.Serializable]
public class Respuesta
{
    public string textoBoton;
    // Si siguienteNodo es null, al elegir esta respuesta se evalúa el desenlace de la conversación
    public NodoDialogo siguienteNodo;
    public int cambioAmistad;
}
