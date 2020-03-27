using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSelector
{    
    private Color m_selectableOutlineColor = Color.white;
    private int m_layerMask;
    private Camera m_raycastingCamera;
    private Vector3 m_instrumentOriginalPosition;

    public InstrumentSelector()
    {
        m_layerMask = LayerMask.GetMask("Instrument");
    }

    public void SetSelectableOutlineColor(Color color)
    {
        m_selectableOutlineColor = color;
    }

    public void SetRaycastingCamera(Camera camera)
    {
        m_raycastingCamera = camera;
    }

    public Instrument RaycastFromCamera(float raycastLength)
    {
        Instrument toReturn = null;
        if(m_raycastingCamera != null)
        {
            RaycastHit hit;
            Ray ray = new Ray(m_raycastingCamera.transform.position, m_raycastingCamera.transform.forward);

            if (Physics.Raycast(ray, out hit, raycastLength, m_layerMask))
            {
                Transform objectHit = hit.transform;

                // If an object on layer Instrument was hit, it must be inheriting "IInstrumentSelectable"
                Instrument instrument = objectHit.gameObject.GetComponent<Instrument>();
                toReturn = instrument;   
            }
        }
        return toReturn;
    }

    public IEnumerator SetIntrumentToView(GameObject instrument, Transform zoomViewSpotTransform)
    {
        m_instrumentOriginalPosition = new Vector3(instrument.transform.position.x, instrument.transform.position.y, instrument.transform.position.z);

        float lerpValue = 0;
        float distance = (instrument.transform.position - zoomViewSpotTransform.position).magnitude;
        while (distance > 0.5f)
        {
            lerpValue += Time.deltaTime * 0.5f;
            Vector3 newPos = Vector3.Lerp(instrument.transform.position, zoomViewSpotTransform.position, lerpValue);
            instrument.transform.position = newPos;
            distance = (instrument.transform.position - zoomViewSpotTransform.position).magnitude;
            yield return null;
        }
        //instrument.transform.transform.localPosition = new Vector3(0, 0, 0);
        //instrument.transform.transform.localRotation = Quaternion.identity;
    }

    public IEnumerator UnsetIntrumentToView(GameObject instrument, Vector3 originalPos)
    {
        m_instrumentOriginalPosition = new Vector3(instrument.transform.position.x, instrument.transform.position.y, instrument.transform.position.z);

        float lerpValue = 0;
        float distance = (instrument.transform.position - originalPos).magnitude;
        while (distance > 0.5f)
        {
            lerpValue += Time.deltaTime * 0.5f;
            Vector3 newPos = Vector3.Lerp(instrument.transform.position, originalPos, lerpValue);
            instrument.transform.position = newPos;
            distance = (instrument.transform.position - originalPos).magnitude;
            yield return null;
        }
        //instrument.transform.transform.localPosition = new Vector3(0, 0, 0);
        //instrument.transform.transform.localRotation = Quaternion.identity;
    }
}
