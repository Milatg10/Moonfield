using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Simula el paso del tiempo en el juego: avanza minutos y horas en tiempo real,
// actualiza la UI con el día y la hora actuales, y tinta una capa de imagen
// según un gradiente de color para representar el ciclo de día y noche.
public class RelojJuego : MonoBehaviour
{
    [Header("Textos de la UI")]
    public TextMeshProUGUI textoDia;
    public TextMeshProUGUI textoHora;

    [Header("Ciclo Día y Noche")]
    public Image capaNoche;
    public Gradient colorDelCielo;

    [Header("Tiempo Actual")]
    public int diaActual = 1;
    public int horaActual = 8;
    public float minutoActual = 0f;

    [Header("Ajustes")]
    public float velocidadReloj = 10f;
    // Permite congelar el tiempo durante diálogos u otras secuencias que requieran pausar el juego
    public bool elTiempoPasa = true;

    void Update()
    {
        if (!elTiempoPasa) return;

        minutoActual += Time.deltaTime * velocidadReloj;

        if (minutoActual >= 60f)
        {
            minutoActual = 0f;
            horaActual++;

            if (horaActual >= 24)
            {
                horaActual = 0;
                diaActual++;
            }
        }

        ActualizarUI();

        if (capaNoche != null)
        {
            // Se normaliza la hora actual a un valor entre 0 y 1 para muestrear el gradiente
            float tiempoTotal = horaActual + (minutoActual / 60f);
            float porcentajeDia = tiempoTotal / 24f;

            capaNoche.color = colorDelCielo.Evaluate(porcentajeDia);
        }
    }

    void ActualizarUI()
    {
        textoDia.text = "Día " + diaActual;
        // El formato {0:00} garantiza que la hora y los minutos siempre ocupen dos dígitos (ej. 08:05)
        textoHora.text = string.Format("{0:00}:{1:00}", horaActual, (int)minutoActual);
    }
}
