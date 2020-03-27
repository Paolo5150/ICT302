using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour, IInstrumentSelectable
{

    private Renderer m_renderer;

    // Start is called before the first frame update
    void Start()
    {
        m_renderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointing()
    {
        m_renderer.material.SetFloat("_Outline", 0.1f);
    }

    public void OnSelected()
    {
        throw new System.NotImplementedException();
    }

    public void OnReleasedPointing()
    {
        m_renderer.material.SetFloat("_Outline", 0.0f);
    }
}
