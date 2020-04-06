using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    InputField m_idField;
    InputField m_pswField;
    Button m_loginButton;

    // Start is called before the first frame update
    void Start()
    {
        m_idField = GameObject.Find("IdField").GetComponent<InputField>();
        m_pswField = GameObject.Find("PswField").GetComponent<InputField>();
        m_loginButton = GameObject.Find("LogInButton").GetComponent<Button>();
        m_loginButton.onClick.AddListener(Click);

    }

    IEnumerator StartNextScene()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(1);             
    }

    IEnumerator ExitAfterTime()
    {
        yield return new WaitForSeconds(4);
        Application.Quit();             
    }

    private void Click()
    {
        m_loginButton.gameObject.SetActive(false);
        Text dogText = GameObject.Find("DogText").GetComponent<Text>();
        dogText.text = "One moment please... "; 

        string id = m_idField.text;
        string psw = m_pswField.text;

        // TODO:Validate data here

        WWWForm form = new WWWForm();
        form.AddField("MurdochUserNumber", id);
        form.AddField("Password", psw);

        NetworkManager.Instance.SendRequest(form, "login.php", (string reply) => {
            Debug.Log("Reply: " + reply);
            JSONObject replyObj = JSONObject.Create(reply);
            string status = "";

            replyObj.GetField(ref status, "Status");

            if (status == "ok")
            {
                // Extract data
                JSONObject data = replyObj.GetField("Data");
                string firstName = "";
                data.GetField(ref firstName, "FirstName");
                
                dogText.text = "Welcome " + firstName + " the simulation will start in a moment";
                StartCoroutine(StartNextScene());                 
            }
            else
            {
                string errorMessage = "";
                replyObj.GetField(ref errorMessage, "Message");
                dogText.text = errorMessage;
                m_loginButton.gameObject.SetActive(true);

            }
        },
        
        () => {
            dogText.text = "An error occurred :)";
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
