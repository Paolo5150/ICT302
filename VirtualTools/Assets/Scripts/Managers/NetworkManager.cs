using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{

    public static string REMOTE_SERVER_ADDRESS = "https://unreckoned-worry.000webhostapp.com/server/";
    //public const string REMOTE_SERVER_ADDRESS = "http://virtualinstruments.unaux.com/server/";
    public static string LOCAL_SERVER_ADDRESS = "http://localhost/ICT302-WebApp/server/";

    private static NetworkManager m_instance;

    public enum SERVER
    {
        LOCAL,
        REMOTE
    }

    public SERVER server;

    public string currentServer;

    public static NetworkManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject coreGameObject = GameObject.Find("Managers");
                m_instance = coreGameObject.AddComponent<NetworkManager>();
            }

            return m_instance;
        }
    }

    protected virtual void Awake()
    {
        if (m_instance == null)
            m_instance = GetComponent<NetworkManager>();
        else
            DestroyImmediate(this);
    }

    private void Start()
    {
        currentServer = server == SERVER.LOCAL ? LOCAL_SERVER_ADDRESS : REMOTE_SERVER_ADDRESS;
    }

    public void SendRequest(WWWForm form, string targetScript, Action<String> onSuccess, Action onFail, Action onAttemptsFailed)
    {
        StartCoroutine(SendPostRequest(form, targetScript, onSuccess, onFail, onAttemptsFailed));      
    }

    private IEnumerator SendPostRequest(WWWForm form, string targetScript, Action<String> onSuccess, Action onFail, Action onAttemptsFailed)
    {
        int attempts = 0;

        while(attempts < 5)
        {
            Logger.LogToFile("Sending to: " + currentServer + targetScript + ", attempt: " + attempts);

            using (UnityWebRequest www = UnityWebRequest.Post(currentServer + targetScript, form))
            {
                www.timeout = 5;

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    onFail();
                    Debug.Log("Error " + www.error);
                    Logger.LogToFile("Network error " + www.error);
                    www.Abort();
                    www.Dispose();
                    attempts++;

                }
                else
                {
                    Debug.Log("Server OK " + www.downloadHandler.text);
                    onSuccess(www.downloadHandler.text);
                    www.Dispose();
                    yield break;

                }

            }
        }
        Logger.LogToFile("Finished send form coroutine");
        onAttemptsFailed();
        yield break;
       

    }
}
