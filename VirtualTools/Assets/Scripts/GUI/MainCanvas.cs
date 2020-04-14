using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    private GameObject m_dog;
    private GameObject m_results;

    public void Init()
    {
        m_dog = transform.GetChild(0).gameObject;
        m_results = transform.GetChild(1).gameObject;

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DogSpeak(string text)
    {
        SetDogEnabled(true);
        m_dog.transform.GetComponentInChildren<Text>().text = text;
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

    public void DisplayResults(bool success, string name, string studentNumber, string date, string startTime, string endTime, int retries)
    {
        m_results.SetActive(true);

        if (success)
            m_results.transform.GetChild(0).GetComponent<Text>().text = "Success!";
        else
            m_results.transform.GetChild(0).GetComponent<Text>().text = "Failed";

        m_results.transform.GetChild(1).GetComponent<Text>().text = "Name: " + name;
        m_results.transform.GetChild(2).GetComponent<Text>().text = "Student Number: " + studentNumber;
        m_results.transform.GetChild(3).GetComponent<Text>().text = "Date: " + date;
        m_results.transform.GetChild(4).GetComponent<Text>().text = "Start Time: " + startTime;
        m_results.transform.GetChild(5).GetComponent<Text>().text = "End Time: " + endTime;
        m_results.transform.GetChild(6).GetComponent<Text>().text = "Retries: " + retries.ToString();
    }
}
