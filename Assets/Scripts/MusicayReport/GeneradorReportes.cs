using UnityEngine;
using System.IO;
using System; 

public class GeneradorReportes : MonoBehaviour
{
    private string rutaArchivo;
    public static float tiempoInicioPartida; 

    // === CONTADORES GLOBALES ===
    public static int clicsClasicos = 0;
    public static int mensajesIA = 0;
    public static int palabrasTotalesIA = 0;
    public static int fracasosTotales = 0;

    void Start()
    {
        // Reseteo total de los contadores estáticos
        clicsClasicos = 0; mensajesIA = 0; palabrasTotalesIA = 0; fracasosTotales = 0;

        int modoGuardado = PlayerPrefs.GetInt("ModoIA", 0);
        string nombreModo = (modoGuardado == 1) ? "Modo_IA" : "Modo_Clasico";
        string fechaExacta = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        rutaArchivo = Application.persistentDataPath + $"/Reporte_{nombreModo}_{fechaExacta}.txt";
        
        File.WriteAllText(rutaArchivo, "========================================\n");
        File.AppendAllText(rutaArchivo, $"   REPORTE DE PARTIDA - MOONFIELD\n");
        File.AppendAllText(rutaArchivo, $"   Modo de juego: {nombreModo.Replace("_", " ")}\n");
        File.AppendAllText(rutaArchivo, "========================================\n\n");

        // Limpiamos suscripciones fantasma de la partida anterior por si acaso
        Application.logMessageReceived -= RegistrarLogEnArchivo; 
        Application.logMessageReceived += RegistrarLogEnArchivo;
        tiempoInicioPartida = Time.time; 
    }

    void RegistrarLogEnArchivo(string mensaje, string stackTrace, LogType tipo)
    {
        if (tipo == LogType.Log || tipo == LogType.Warning)
        {
            if (string.IsNullOrEmpty(rutaArchivo)) return;
            string hora = DateTime.Now.ToString("HH:mm:ss");
            File.AppendAllText(rutaArchivo, $"[{hora}] {mensaje}\n");
        }
    }

    // Sin bloqueadores, si el cofre lo llama, se imprime sí o sí.
    public static void LanzarMetricasAlLog()
    {
        float tiempoJugado = Time.time - tiempoInicioPartida;
        int minutos = Mathf.FloorToInt(tiempoJugado / 60);
        int segundos = Mathf.FloorToInt(tiempoJugado % 60);

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
        Application.logMessageReceived -= RegistrarLogEnArchivo;
    }
}