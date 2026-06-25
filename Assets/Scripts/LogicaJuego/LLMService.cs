using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using System.IO;

// Servicio que encapsula la comunicación con la API de OpenAI.
// Carga la clave en local desde StreamingAssets para no exponerla en el código fuente,
// y expone una corrutina que los NPCs invocan para obtener respuestas generadas por el modelo.
public class LLMService : MonoBehaviour
{
    private string apiUrl = "https://api.openai.com/v1/chat/completions";
    private string apiKey = "";

    void Start()
    {
        LoadApiKey();
    }

    void LoadApiKey()
    {
        // StreamingAssets es la única carpeta de Unity accesible en tiempo de ejecución
        // desde todas las plataformas sin necesidad de AssetBundles
        string path = Path.Combine(Application.streamingAssetsPath, "secrets.json");
        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            SecretsData data = JsonUtility.FromJson<SecretsData>(jsonContent);
            apiKey = data.openai_api_key;
            Debug.Log($"[SISTEMA] Clave cargada.");
        }
        else
        {
            Debug.LogError("[SISTEMA] ERROR: No está el archivo secrets.json");
        }
    }

    public IEnumerator CallAI(string systemPrompt, string userMessage, System.Action<string> callback = null)
    {
        if (string.IsNullOrEmpty(apiKey)) yield break;

        OpenAIRequest requestData = new OpenAIRequest();
        requestData.model = "gpt-4o-mini";
        requestData.messages = new Message[]
        {
            new Message { role = "system", content = systemPrompt },
            new Message { role = "user", content = userMessage }
        };

        // max_completion_tokens reemplaza al antiguo max_tokens en los modelos gpt-4o en adelante
        requestData.max_completion_tokens = 1000;

        string jsonBody = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                OpenAIResponse response = JsonUtility.FromJson<OpenAIResponse>(request.downloadHandler.text);
                string reply = response.choices[0].message.content;

                callback?.Invoke(reply);
            }
            else
            {
                Debug.LogError($"[SISTEMA] Error API: {request.error}\n{request.downloadHandler.text}");
            }
        }
    }
}

// Las siguientes clases son los moldes que JsonUtility necesita para serializar
// la petición a JSON y deserializar la respuesta de la API con la misma estructura

[System.Serializable]
public class SecretsData { public string openai_api_key; }

[System.Serializable]
public class OpenAIRequest
{
    public string model;
    public Message[] messages;
    public int max_completion_tokens;
}

[System.Serializable]
public class Message { public string role; public string content; }

[System.Serializable]
public class OpenAIResponse { public Choice[] choices; }

[System.Serializable]
public class Choice { public Message message; }
