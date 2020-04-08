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
        ADDSON_BROWN_FORCEPS,
        SCALPEL,
        METZEMBAUM_SCISSOR,
        ROCHESTER_CARMALT_FORCEPS,
        MAYO_HEGAR_NEEDLE_DRIVER,
        SUTURE_SCISSOR,
        MAYO_SCISSOR,
        TOWEL_CLAMPS
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
            case INSTRUMENT_TAG.MAYO_HEGAR_NEEDLE_DRIVER:
                return "Mayo Hegar Needle Driver";
            case INSTRUMENT_TAG.MAYO_SCISSOR:
                return "Mayo Scissor";
            case INSTRUMENT_TAG.METZEMBAUM_SCISSOR:
                return "Metzembaum Scissor";
            case INSTRUMENT_TAG.ROCHESTER_CARMALT_FORCEPS:
                return "Rochester Carmalt Forceps";
            case INSTRUMENT_TAG.SCALPEL:
                return "Scalpel";
            case INSTRUMENT_TAG.SCISSOR:
                return "Scissor";
            case INSTRUMENT_TAG.SUTURE_SCISSOR:
                return "Suture Scissor";
            case INSTRUMENT_TAG.TOWEL_CLAMPS:
                return "Towel Clamps";
            default:
                return "";
        }
    } 
}
