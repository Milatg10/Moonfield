using UnityEngine;

// Esto nos permite crear "tarjetas" de objetos haciendo clic derecho en Unity
[CreateAssetMenu(fileName = "NuevoObjeto", menuName = "Inventario/Objeto")]
public class ObjetoInventario : ScriptableObject
{
    public string nombreObjeto; // Ej: "Hacha de Pam"
    public Sprite icono;        // El dibujo que saldrá en la barra
}