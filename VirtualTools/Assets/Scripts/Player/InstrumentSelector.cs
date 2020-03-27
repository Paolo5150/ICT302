using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSelector : MonoBehaviour
{
    public Camera raycastingCamera;
    public float raycastLength = 150.0f;
    public Color selectableOutlineColor = Color.white;

    private int m_layerMask;
    private Instrument m_currentlyPointingInstrument;

    // Start is called before the first frame update
    void Start()
    {
        m_layerMask = LayerMask.GetMask("Instrument");
    }

    // Update is called once per frame
    void Update()
    {
        RaycastFromCamera();
        if(Input.GetButton("Fire1"))
        {
            if(m_currentlyPointingInstrument != null)
            {
                Debug.Log("Selected " + m_currentlyPointingInstrument.gameObject.name);
            }
        }

    }

    void RaycastFromCamera()
    {
        if(raycastingCamera != null)
        {
            RaycastHit hit;
            Ray ray = new Ray(raycastingCamera.transform.position, raycastingCamera.transform.forward);

            if (Physics.Raycast(ray, out hit, raycastLength, m_layerMask))
            {
                Transform objectHit = hit.transform;

                // If an object on layer Instrument was hit, it must be inheriting "IInstrumentSelectable"
                Instrument instrument = objectHit.gameObject.GetComponent<Instrument>();

                if(instrument != null && instrument != m_currentlyPointingInstrument)
                {
                    if (m_currentlyPointingInstrument != null)
                        m_currentlyPointingInstrument.OnReleasedPointing();

                     instrument.SetOutlineColor(selectableOutlineColor);
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
