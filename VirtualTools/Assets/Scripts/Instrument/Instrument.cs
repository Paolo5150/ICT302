using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour, IInstrumentSelectable
{
    private Renderer m_renderer;
    public Vector3 originalPosition;
    public Quaternion originalRotation;


    // Start is called before the first frame update
    void Start()
    {
        m_renderer = GetComponent<MeshRenderer>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetOutlineColor(Color color)
    {
        m_renderer.material.SetColor("_OutColor", color);
    }

    public void OnPointing()
    {
        SetEnableOutline(true);
        
    }

    public void OnSelected()
    {
        throw new System.NotImplementedException();
    }

    public void SetEnableOutline(bool enabled)
    {
        if(enabled)
            m_renderer.material.SetFloat("_Outline", 0.1f);
        else
            m_renderer.material.SetFloat("_Outline", 0.0f);

    }

    public void OnReleasedPointing()
    {
        SetEnableOutline(false);

    }
}
