using UnityEngine;
using UnityEngine.UI; // Para poder usar Imágenes
using TMPro;

public class RelojJuego : MonoBehaviour
{
    [Header("Textos de la UI")]
    public TextMeshProUGUI textoDia;
    public TextMeshProUGUI textoHora;

    [Header("Ciclo Día y Noche")]
    public Image capaNoche;        // La imagen que acabamos de crear
    public Gradient colorDelCielo; // La barrita mágica de colores

    [Header("Tiempo Actual")]
    public int diaActual = 1;
    public int horaActual = 8;
    public float minutoActual = 0f;

    [Header("Ajustes")]
    public float velocidadReloj = 10f; 
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

        // --- ¡LA MAGIA DE LA LUZ! ---
        if (capaNoche != null)
        {
            // Calculamos en qué porcentaje del día estamos (0.0 a 1.0)
            float tiempoTotal = horaActual + (minutoActual / 60f);
            float porcentajeDia = tiempoTotal / 24f;

            // Le decimos a la capa que coja el color exacto de la barrita
            capaNoche.color = colorDelCielo.Evaluate(porcentajeDia);
        }
    }

    void ActualizarUI()
    {
        textoDia.text = "Día " + diaActual;
        textoHora.text = string.Format("{0:00}:{1:00}", horaActual, (int)minutoActual);
    }
}