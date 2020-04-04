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
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);
    }

    private void Click()
    {
        Text dogText = GameObject.Find("DogText").GetComponent<Text>();
        dogText.text = "One moment please... "; 

        string id = m_idField.text;
        string psw = m_pswField.text;

        // Validate data

        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("password", psw);

        NetworkManager.Instance.SendRequest(form, "login.php", (string reply) => {

            Debug.Log(reply);
            JSONObject test = JSONObject.Create(reply);
            string status = "";

            test.GetField(ref status, "Status");

            dogText.text = "Status: " + status;


              if(status == "ok")
              {
                // Extract data
                JSONObject data = test.GetField("Data");
                string firstName = "";
                data.GetField(ref firstName, "FirstName");
                dogText.text = "Welcome " + firstName + ", the simulation will start shortly! "; //Here we can put the name

                StartCoroutine(StartNextScene());
              }
              else
              {
                  dogText.text = "ID or password incorrect.";
              }
        },
        
        () => {

        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
