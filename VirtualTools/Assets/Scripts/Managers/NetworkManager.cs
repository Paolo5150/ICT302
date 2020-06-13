using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{

    public const string REMOTE_SERVER_ADDRESS = "http://vegas.murdoch.edu.au/vinst/server/";
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

    /// <summary>
    /// Send a request to the server, if fails, will try again for a total of 5 times
    /// </summary>
    /// <param name="form">The form to be submitted</param>
    /// <param name="targetScript">The php script to send to</param>
    /// <param name="onSuccess">Success request callback</param>
    /// <param name="onFail">Failed request callback</param>
    /// <param name="onAttemptsFailed">All attempts failed callback</param>
    /// <returns></returns>
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
                    Logger.LogToFile("Network error " + www.error);
                    onFail();
                    Debug.Log("Error " + www.error);
                    www.Abort();
                    www.Dispose();
                    attempts++;

                }
                else
                {
                    //Debug.Log("Server OK " + www.downloadHandler.text);
                    Logger.LogToFile("Successful reply " + www.downloadHandler.text);
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
