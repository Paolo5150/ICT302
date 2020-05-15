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

    public Instrument GetInstrumentRaycastFromCamera(float raycastLength)
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

    public InstrumentPositionTaskSlot GetInstrumentPositionTaskSlotRaycastFromCamera(float raycastLength)
    {
        InstrumentPositionTaskSlot toReturn = null;
        if (m_raycastingCamera != null)
        {
            RaycastHit hit;
            Ray ray = new Ray(m_raycastingCamera.transform.position, m_raycastingCamera.transform.forward);

            if (Physics.Raycast(ray, out hit, raycastLength, m_layerMask))
            {
                Transform objectHit = hit.transform;

                InstrumentPositionTaskSlot instrument = objectHit.gameObject.GetComponent<InstrumentPositionTaskSlot>();
                toReturn = instrument;
            }
        }
        return toReturn;
    }

    public IEnumerator LerpToPosition(GameObject instrument, Vector3 viewPosition)
    {
        float lerpValue = 0;
        float distance = (instrument.transform.position - viewPosition).magnitude;
        while (distance > 0.05f)
        {
            lerpValue += Time.deltaTime * 0.5f;
            Vector3 newPos = Vector3.Lerp(instrument.transform.position, viewPosition, lerpValue);
            instrument.transform.position = newPos;
            distance = (instrument.transform.position - viewPosition).magnitude;
            yield return null;
        }
    }

    public IEnumerator LerpToRotation(GameObject instrument, Quaternion viewRotation)
    {
        float lerpValue = 0;
        int i = 1000;
        while (i > 0)
        {
            lerpValue += Time.deltaTime * 0.5f;
            Quaternion newRot = Quaternion.Lerp(instrument.transform.rotation, viewRotation, lerpValue);
            instrument.transform.rotation = newRot;
            i--;
            yield return null;
        }
    }

    public void ItemRotationManipulation(Instrument instrument, float speed)
    {
    }
}
