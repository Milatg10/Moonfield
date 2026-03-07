using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using System.IO;

public class LLMService : MonoBehaviour
{
    // Usamos el endpoint MODERNO de Chat (no el antiguo de Davinci)
    private string apiUrl = "https://api.openai.com/v1/chat/completions";
    private string apiKey = "";

    void Start()
    {
        LoadApiKey();

        // Comentarla para no gastar tokens en pruebas
        //StartCoroutine(CallAI("Eres un NPC de un juego RPG.", "¡Hola! ¿Qué vendes?"));
    }

    void LoadApiKey()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "secrets.json");
        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            SecretsData data = JsonUtility.FromJson<SecretsData>(jsonContent);
            apiKey = data.openai_api_key;
            Debug.Log("✅ Clave cargada.");
        }
        else
        {
            Debug.LogError("❌ ERROR: No está el archivo secrets.json");
        }
    }

    // Esta función es la que usarás desde tus NPCs
    public IEnumerator CallAI(string systemPrompt, string userMessage, System.Action<string> callback = null)
    {
        if (string.IsNullOrEmpty(apiKey)) yield break;

        // 1. CREAMOS EL PAQUETE DE DATOS (Usando las clases de abajo)
        OpenAIRequest requestData = new OpenAIRequest();
        requestData.model = "gpt-4o-mini"; // Tu modelo elegido
        requestData.messages = new Message[]
        {
            new Message { role = "system", content = systemPrompt },
            new Message { role = "user", content = userMessage }
        };
        
        // CORRECCIÓN APLICADA: Usamos el nombre nuevo del parámetro
        requestData.max_completion_tokens = 1000; 

        // Convertimos C# a Texto JSON
        string jsonBody = JsonUtility.ToJson(requestData);

        // 2. ENVIAMOS A INTERNET
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            // Esperamos...
            yield return request.SendWebRequest();

            // 3. RECIBIMOS RESPUESTA
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("📦 JSON CRUDO: " + request.downloadHandler.text);
                // Convertimos Texto JSON a C#
                OpenAIResponse response = JsonUtility.FromJson<OpenAIResponse>(request.downloadHandler.text);
                string reply = response.choices[0].message.content;
                
                Debug.Log("<color=green>🤖 IA Dice:</color> " + reply);
                
                // Esto sirve para devolver el texto al NPC que lo pidió
                if (callback != null) callback(reply);
            }
            else
            {
                Debug.LogError("❌ Error API: " + request.error + "\n" + request.downloadHandler.text);
            }
        }
    }
}

// --- ESTRUCTURAS DE DATOS (MOLDES) ---
// No las borres, Unity las necesita para "traducir" a JSON

[System.Serializable]
public class SecretsData { public string openai_api_key; }

[System.Serializable]
public class OpenAIRequest
{
    public string model;
    public Message[] messages;
    public int max_completion_tokens; // Nombre corregido para GPT-5/GPT-4o
}

[System.Serializable]
public class Message { public string role; public string content; }

[System.Serializable]
public class OpenAIResponse { public Choice[] choices; }

[System.Serializable]
public class Choice { public Message message; }