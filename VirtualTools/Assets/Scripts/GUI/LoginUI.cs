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
        Logger.Init();
        Logger.LogToFile("Login START");

        m_idField = GameObject.Find("IdField").GetComponent<InputField>();
        m_pswField = GameObject.Find("PswField").GetComponent<InputField>();
        m_loginButton = GameObject.Find("LogInButton").GetComponent<Button>();
        m_loginButton.onClick.AddListener(LoginClick);
        PlayerPrefs.DeleteAll(); //Clear player prefs
    }

    IEnumerator StartNextScene()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);             
    }

    private void LoginClick()
    {
        m_loginButton.gameObject.SetActive(false);
        Text dogText = GameObject.Find("DogText").GetComponent<Text>();
        dogText.text = "One moment please... "; 

        string id = m_idField.text;
        string psw = m_pswField.text;

        WWWForm form = new WWWForm();
        form.AddField("MurdochUserNumber", id);
        form.AddField("Password", psw);
        form.AddField("IsSim", 1);

        Logger.LogToFile("Sending login form");
        NetworkManager.Instance.SendRequest(form, "login.php", (string reply) => {

            // Reply callback
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
                if(data.HasField("Layout"))
                {
                    data.GetField(out order, "Layout", "");
                    order = order.Substring(2,order.Length - 4);
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
            dogText.text = "An error occurred. Please try again later.";
            m_loginButton.gameObject.SetActive(true);
        },
        ()=> {
            // If all attempts to connect fail
            Logger.LogToFile("All attempts failed to log in, application will quit");
            Application.Quit();
        });

    }

    public void Quit()
    {
        Application.Quit();
    }
}
