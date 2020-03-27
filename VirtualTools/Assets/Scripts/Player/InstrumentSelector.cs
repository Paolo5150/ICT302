using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSelector : MonoBehaviour
{
    public Camera raycastingCamera;

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
    }

    void RaycastFromCamera()
    {
        if(raycastingCamera != null)
        {
            RaycastHit hit;
            Ray ray = new Ray(raycastingCamera.transform.position, raycastingCamera.transform.forward);

            if (Physics.Raycast(ray, out hit, 100.0f, m_layerMask))
            {
                Transform objectHit = hit.transform;
                Debug.Log("HITTING " + objectHit.gameObject.name);

                // If an object on layer Instrument was hit, it must be inheriting "IInstrumentSelectable"
                Instrument instrument = objectHit.gameObject.GetComponent<Instrument>();

                if(instrument != null && instrument != m_currentlyPointingInstrument)
                {
                    if (m_currentlyPointingInstrument != null)
                        m_currentlyPointingInstrument.OnReleasedPointing();
          
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
