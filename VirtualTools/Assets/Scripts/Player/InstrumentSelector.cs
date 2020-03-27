using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSelector : MonoBehaviour
{
    public Camera raycastingCamera;

    int m_layerMask;

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
                if (instrument)
                    instrument.OnPointing();
            }
        }
    }
}
