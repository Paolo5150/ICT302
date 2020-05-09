using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentPositionTaskSlot : MonoBehaviour
{
    public Instrument.INSTRUMENT_TAG CurrentInstrument;
    public Instrument.INSTRUMENT_TAG CorrectInstrument;

    public void OnPointing()
    {
        foreach (Material mat in GetComponent<Renderer>().materials)
        {
            mat.color = new Color(0, 1, 0, 0.5f);
        }
    }

    public void OnReleasedPointing()
    {
        foreach (Material mat in GetComponent<Renderer>().materials)
        {
            mat.color = new Color(1,1,1,0.5f);
        }
    }

    /*

    private void Start()
    {
        for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++)
        {
            GetComponent<Renderer>().materials[i] = InstrumentPositionTaskLocManager.Instance.Material;
        }
    }

    public void OnPointing()
    {
        for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++)
        {
            GetComponent<Renderer>().materials[i] = InstrumentPositionTaskLocManager.Instance.MaterialSelected;
        }
    }

    public void OnReleasedPointing()
    {
        for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++)
        {
            GetComponent<Renderer>().materials[i] = InstrumentPositionTaskLocManager.Instance.Material;
        }
    }
    */
}
