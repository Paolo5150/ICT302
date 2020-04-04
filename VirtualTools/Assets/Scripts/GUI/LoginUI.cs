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
            Debug.Log(reply);

            JSONObject replyObj = JSONObject.Create(reply);
            string status = "";

            replyObj.GetField(ref status, "Status");

            dogText.text = "Status: " + status;
            if (status == "ok")
            {
                // Extract data
                JSONObject data = replyObj.GetField("Data");
                string firstName = "";
                data.GetField(ref firstName, "FirstName");
                
                int firstLoginComplete = 0;
                data.GetField(ref firstLoginComplete, "FirstLoginCompleted");

                int enabled = 0;
                data.GetField(ref enabled, "AccountActive");

                if(enabled == 0)
                {
                    if(firstLoginComplete == 0)
                    {
                        dogText.text = "IMPORTANT: An email has been sent to you, please follow the link to reset your password. Come back and log in here after you've done that.";
                        StartCoroutine(ExitAfterTime());
                    }
                    else
                    {
                        dogText.text = "Your account is not valid!";
                        StartCoroutine(ExitAfterTime());

                    }
                }
                else
                {
                    dogText.text = "Welcome " + firstName + " the simulation will start in a moment";
                    StartCoroutine(StartNextScene());
                }
                
            }
            else
            {
                dogText.text = "ID or password incorrect.";
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
