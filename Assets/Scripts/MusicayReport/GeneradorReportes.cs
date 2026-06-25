using UnityEngine;
using System.IO;
using System;

// Recoge métricas de la sesión de juego y las vuelca en un archivo de texto en disco.
// Los contadores son estáticos para que cualquier script los incremente sin necesidad de referencia directa.
// El reporte se construye capturando todos los Debug.Log de Unity mediante el evento logMessageReceived.
public class GeneradorReportes : MonoBehaviour
{
    private string rutaArchivo;
    public static float tiempoInicioPartida;

    public static int clicsClasicos = 0;
    public static int mensajesIA = 0;
    public static int palabrasTotalesIA = 0;
    public static int fracasosTotales = 0;

    void Start()
    {
        // Los contadores son estáticos y persisten entre escenas, por lo que se reinician
        // explícitamente al comenzar la partida para que cada sesión parta de cero
        clicsClasicos = 0; mensajesIA = 0; palabrasTotalesIA = 0; fracasosTotales = 0;

        int modoGuardado = PlayerPrefs.GetInt("ModoIA", 0);
        string nombreModo = (modoGuardado == 1) ? "Modo_IA" : "Modo_Clasico";
        string fechaExacta = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        rutaArchivo = Application.persistentDataPath + $"/Reporte_{nombreModo}_{fechaExacta}.txt";

        File.WriteAllText(rutaArchivo, "========================================\n");
        File.AppendAllText(rutaArchivo, $"   REPORTE DE PARTIDA - MOONFIELD\n");
        File.AppendAllText(rutaArchivo, $"   Modo de juego: {nombreModo.Replace("_", " ")}\n");
        File.AppendAllText(rutaArchivo, "========================================\n\n");

        // Se desuscribe antes de suscribir para evitar duplicados si el objeto se reinicia en la misma sesión
        Application.logMessageReceived -= RegistrarLogEnArchivo;
        Application.logMessageReceived += RegistrarLogEnArchivo;
        tiempoInicioPartida = Time.time;
    }

    void RegistrarLogEnArchivo(string mensaje, string stackTrace, LogType tipo)
    {
        // Solo se persisten logs y warnings; los errores y excepciones se omiten del reporte
        if (tipo == LogType.Log || tipo == LogType.Warning)
        {
            if (string.IsNullOrEmpty(rutaArchivo)) return;
            string hora = DateTime.Now.ToString("HH:mm:ss");
            File.AppendAllText(rutaArchivo, $"[{hora}] {mensaje}\n");
        }
    }

    // Método estático para que el cofre de victoria pueda volcar las métricas al log
    // sin necesidad de tener una referencia al componente
    public static void LanzarMetricasAlLog()
    {
        float tiempoJugado = Time.time - tiempoInicioPartida;
        int minutos = Mathf.FloorToInt(tiempoJugado / 60);
        int segundos = Mathf.FloorToInt(tiempoJugado % 60);

        // Si no hubo mensajes de IA el promedio es 0 para evitar división por cero
        float promedioPalabras = (mensajesIA > 0) ? (float)palabrasTotalesIA / mensajesIA : 0f;

        Debug.Log("\n========================================");
        Debug.Log("   MÉTRICAS FINALES DEL JUGADOR (TFG)");
        Debug.Log("========================================");
        Debug.Log($" * Tiempo total: {minutos} min y {segundos} seg");
        Debug.Log($" * Fracasos/Errores: {fracasosTotales}");
        Debug.Log($" * Clics Clásicos: {clicsClasicos}");
        Debug.Log($" * Mensajes a la IA: {mensajesIA}");
        Debug.Log($" * Media palabras/mensaje: {promedioPalabras:F1}");
        Debug.Log("========================================\n");
    }

    void OnDestroy()
    {
        // Es imprescindible desuscribirse al destruir el objeto; si no, Unity intentaría
        // escribir en el archivo desde un callback con referencia a un objeto ya destruido
        Application.logMessageReceived -= RegistrarLogEnArchivo;
    }
}
