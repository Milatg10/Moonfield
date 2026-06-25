using UnityEngine;

// Define los datos de un objeto del inventario como asset reutilizable.
// Cada instancia representa un tipo de objeto distinto (nombre e icono)
// que el SistemaInventario puede añadir al jugador en tiempo de ejecución.
[CreateAssetMenu(fileName = "NuevoObjeto", menuName = "Inventario/Objeto")]
public class ObjetoInventario : ScriptableObject
{
    public string nombreObjeto;
    public Sprite icono;
}
