using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    private GameObject m_dog;

    public void Init()
    {
        m_dog = transform.GetChild(0).gameObject;

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
        int index = 0;
        while(index < instructions.Length)
        {
            DogSpeak(instructions[index]);
            if (Input.GetButton("Fire1"))
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
}
