using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour, IInstrumentSelectable
{
    private Renderer m_renderer;
    public Vector3 originalPosition;
    public Quaternion originalRotation;

    public enum INSTRUMENT_TAG
    {
        SCALP,
        SCISSOR,
        ADDSON_BROWN_FORCEPS
    }

    public INSTRUMENT_TAG instrumentTag;
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

    public static string GetName(INSTRUMENT_TAG tag)
    {
        switch (tag)
        {
            case INSTRUMENT_TAG.ADDSON_BROWN_FORCEPS:
                return "Addson-Brown Forceps";
            case INSTRUMENT_TAG.SCALP:
                return "Scalp";
            case INSTRUMENT_TAG.SCISSOR:
                return "Scissor";
            default:
                return "";
        }
    } 
}
