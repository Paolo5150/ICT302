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

    private void Click()
    {
        string id = m_idField.text;
        string psw = m_pswField.text;

        // Validate data

        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("password", psw);

        NetworkManager.Instance.SendRequest(form, "login.php", (string reply) => {
            
            if(reply == "ok")
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                Text dogText = GameObject.Find("DogText").GetComponent<Text>();
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
