using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    private GameObject m_dog;
    private GameObject m_controlHints;
    private GameObject m_escapeMenu;
    private GameObject m_sceneSelector;

    private GameObject m_results;
    private Text m_nameText;
    private Text m_studentNumberText;
    private Text m_dateText;
    private Text m_startText;
    private Text m_endText;
    private Text m_retriesText;

    public void Init()
    {
        m_dog = GameObject.Find("Dog");
        m_controlHints = GameObject.Find("ControlsHint");
        m_escapeMenu = GameObject.Find("EscapeMenu");
        m_sceneSelector = GameObject.Find("SceneSelector");

        m_results = GameObject.Find("ResultsPanel");
        m_nameText = m_results.transform.Find("Name").GetComponent<Text>();
        m_studentNumberText = m_results.transform.Find("StudentNumber").GetComponent<Text>();
        m_dateText = m_results.transform.Find("Date").GetComponent<Text>();
        m_startText = m_results.transform.Find("StartTime").GetComponent<Text>();
        m_endText = m_results.transform.Find("EndTime").GetComponent<Text>();
        m_retriesText = m_results.transform.Find("Retries").GetComponent<Text>();



        m_results.SetActive(false);
        m_dog.SetActive(false);
        m_controlHints.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHintActive(bool active)
    {
        m_controlHints.SetActive(active);
    }

    public void DogSpeak(string text)
    {
        SetDogEnabled(true);
        m_dog.transform.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    public void SetDogEnabled(bool enabled)
    {
        m_dog.SetActive(enabled);
    }

    private IEnumerator PopUpMessage(float seconds, string text)
    {
        DogSpeak(text);
        yield return new WaitForSeconds(seconds);
        SetDogEnabled(false);
    }
    public void DogPopUp(float seconds, string text)
    {
        StartCoroutine(PopUpMessage(seconds, text));
    }

    public void HideSceneSelectionGUI()
    {
        m_sceneSelector.SetActive(false);
        m_escapeMenu.SetActive(false);
    }

    private IEnumerator InstructionSequence(string[] instructions, Action action)
    {
        yield return new WaitForSeconds(0.2f);

        int index = 0;
        while(index < instructions.Length)
        {
            DogSpeak(instructions[index]);
            if (Input.GetButtonDown("Fire1"))
            {
                index++;
                if (index >= instructions.Length) break;

                DogSpeak(instructions[index]);
                yield return new WaitForSeconds(0.5f);
            }
            else
                yield return null;
        }
        SetDogEnabled(false);
        action();
    }

    public void DogInstructionSequence(string[] instructions, Action action)
    {
        StartCoroutine(InstructionSequence(instructions, action));
    }

    public void SetEscapeMenu(bool enabled)
    {
        m_escapeMenu.SetActive(enabled);
    }

    public void SetSceneSelector(bool enabled)
    {
        m_sceneSelector.SetActive(enabled);
    }

    public void ToggleEscapeMenu(bool includeSceneSelector = false)
    {
        m_escapeMenu.SetActive(!m_escapeMenu.activeSelf);
        SetSceneSelector(m_escapeMenu.activeSelf); //Hide or show the scene selector depending on whether the escape menu is hidden or shown, respectively
    }

    public void DisplayResults(bool success, string name, string studentNumber, string date, string startTime, string endTime, int retries)
    {
        m_results.SetActive(true);

        if (success)
            m_results.transform.GetChild(0).GetComponent<Text>().text = "Success!";
        else
            m_results.transform.GetChild(0).GetComponent<Text>().text = "Failed";

        m_nameText.text = name;
        m_studentNumberText.text = studentNumber;
        m_dateText.text = date;
        m_startText.text = startTime;
        m_endText.text = endTime;
        m_retriesText.text = "" +  retries; //LAziest int->string conversion ever

    }

    public bool GetResultsPanelEnabled()
    {
        return m_results.activeSelf;
    }
}
