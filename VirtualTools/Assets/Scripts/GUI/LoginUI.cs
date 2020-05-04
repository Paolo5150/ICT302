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
        Logger.LogToFile("Login START");

        m_idField = GameObject.Find("IdField").GetComponent<InputField>();
        m_pswField = GameObject.Find("PswField").GetComponent<InputField>();
        m_loginButton = GameObject.Find("LogInButton").GetComponent<Button>();
        m_loginButton.onClick.AddListener(Click);
        PlayerPrefs.DeleteAll(); //Clear player prefs
    }

    IEnumerator StartNextScene()
    {
        yield return new WaitForSeconds(2);
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
        form.AddField("IsSim", 1);

        Logger.LogToFile("Send login form");
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

                string lastName = "";
                data.GetField(ref lastName, "LastName");

                string murdochUserNumber = "";
                data.GetField(ref murdochUserNumber, "MurdochUserNumber");

                dogText.text = "Welcome " + firstName + " the simulation will start in a moment";

                string aMode = "";
                data.GetField(out aMode, "AssessmentMode", "false");
                bool assessmentMode = aMode.Equals("true");

                UnityEngine.Debug.Log("AM " + assessmentMode);
                string order = "";
                if(data.HasField("InstrumentLayout"))
                {
                    data.GetField(out order, "InstrumentLayout", "");
                }
                Debug.Log("Order: " + order);
                //Save info to PlayerPrefs
                PlayerPrefs.SetString("FirstName", firstName);
                PlayerPrefs.SetString("LastName", lastName);
                PlayerPrefs.SetString("MurdochUserNumber", murdochUserNumber);
                PlayerPrefs.SetInt("AssessmentMode", assessmentMode? 1 : 0);
                PlayerPrefs.SetString("InstrumentOrder", order);

                PlayerPrefs.Save();

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
            m_loginButton.gameObject.SetActive(true);
        },
        ()=> {
            // If all attempts to connect fail
            Application.Quit();
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Quit()
    {
        Application.Quit();
    }
}
