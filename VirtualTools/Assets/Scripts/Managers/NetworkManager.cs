using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{

    public static string SERVER_ADDRESS = "https://unreckoned-worry.000webhostapp.com/";

    private static NetworkManager m_instance;

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

    public void SendRequest(WWWForm form, string targetScript, Action<String> onSuccess, Action onFail)
    {
        StartCoroutine(SendPostRequest(form, targetScript, onSuccess, onFail));
    }

    private IEnumerator SendPostRequest(WWWForm form, string targetScript, Action<String> onSuccess, Action onFail)
    {
        Debug.Log("Sending to: " + SERVER_ADDRESS + targetScript);
        UnityWebRequest www = UnityWebRequest.Post(SERVER_ADDRESS + targetScript, form);
        yield return www.SendWebRequest();
        

        if (www.isNetworkError || www.isHttpError)
        {
            onFail();
            Debug.Log(www.error);
        }
        else
        {
            onSuccess(www.downloadHandler.text);
        }
    }
}
