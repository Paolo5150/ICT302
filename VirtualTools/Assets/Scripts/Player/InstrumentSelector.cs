using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSelector
{    
    private Color m_selectableOutlineColor = Color.white;
    private int m_layerMask;
    private Instrument m_currentlyPointingInstrument;
    private Camera m_raycastingCamera;

    public InstrumentSelector()
    {
        m_layerMask = LayerMask.GetMask("Instrument");
    }

    public void SetSelectableOutlineColor(Color color)
    {
        m_selectableOutlineColor = color;
    }
    public Instrument GetCurrentlyPointingIstrument()
    {
        return m_currentlyPointingInstrument;
    }
    public void SetRaycastingCamera(Camera camera)
    {
        m_raycastingCamera = camera;
    }

    public void RaycastFromCamera(float raycastLength)
    {
        if(m_raycastingCamera != null)
        {
            RaycastHit hit;
            Ray ray = new Ray(m_raycastingCamera.transform.position, m_raycastingCamera.transform.forward);

            if (Physics.Raycast(ray, out hit, raycastLength, m_layerMask))
            {
                Transform objectHit = hit.transform;

                // If an object on layer Instrument was hit, it must be inheriting "IInstrumentSelectable"
                Instrument instrument = objectHit.gameObject.GetComponent<Instrument>();

                if(instrument != null && instrument != m_currentlyPointingInstrument)
                {
                    if (m_currentlyPointingInstrument != null)
                        m_currentlyPointingInstrument.OnReleasedPointing();

                     instrument.SetOutlineColor(m_selectableOutlineColor);
                     instrument.OnPointing();
                     m_currentlyPointingInstrument = instrument;   
                }
            }
            else
            {
                if (m_currentlyPointingInstrument != null)
                {
                    m_currentlyPointingInstrument.OnReleasedPointing();
                    m_currentlyPointingInstrument = null;
                }
            }
        }
    }
}
