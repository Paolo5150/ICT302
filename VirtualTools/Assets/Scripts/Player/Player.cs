using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoBehaviour
{
    public float raycastLength = 150.0f;
    public Color selectableOutlineColor = Color.white;

    private InstrumentSelector m_instrumentSelector;
    private FirstPersonController m_firstPersonController;
    private GameObject m_zoomViewSpot;
    private Instrument m_currentlyPointingInstrument;
    private Transform m_currentlyViewingTrans;

    public enum PlayerMode
    {
        VIEWING,
        FREE
    }

    private PlayerMode m_playerMode;

    // Start is called before the first frame update
    void Start()
    {
        m_playerMode = PlayerMode.FREE;

        m_instrumentSelector = new InstrumentSelector();
        m_firstPersonController = GetComponent<FirstPersonController>();

        Camera cam = GetComponentInChildren<Camera>();
        m_instrumentSelector.SetRaycastingCamera(cam);

        m_zoomViewSpot = transform.GetChild(1).gameObject; //Might need to change this
        m_instrumentSelector.SetSelectableOutlineColor(selectableOutlineColor);
    }

    private void SetPlayerMode(PlayerMode mode)
    {
        switch(mode)
        {
            case PlayerMode.FREE:
                m_firstPersonController.enabled = true;
                break;
            case PlayerMode.VIEWING:
                m_firstPersonController.enabled = false;
                m_currentlyPointingInstrument.SetEnableOutline(false);
                break;
        }
        m_playerMode = mode;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_playerMode == PlayerMode.FREE)
        {
            UpdatePointingInstrument();

            if (Input.GetButton("Fire1"))
            {
                if (m_currentlyPointingInstrument != null)
                { 
                    SetPlayerMode(PlayerMode.VIEWING);
                    StartCoroutine(m_instrumentSelector.LerpToPosition(m_currentlyPointingInstrument.gameObject, m_zoomViewSpot.transform.position,Quaternion.identity));
                }
            }
        }
        else
        {
            if (Input.GetButton("Fire2"))
            {
                StartCoroutine(m_instrumentSelector.LerpToPosition(m_currentlyPointingInstrument.gameObject, m_currentlyPointingInstrument.originalPosition, Quaternion.identity));
                SetPlayerMode(PlayerMode.FREE);
            }
        }

    }

    private void UpdatePointingInstrument()
    {
        Instrument instrument = m_instrumentSelector.RaycastFromCamera(raycastLength);
        // If I'm looking at an intrument and it's not the one i was already looking at
        if (instrument != null && instrument != m_currentlyPointingInstrument)
        {
            if (m_currentlyPointingInstrument != null)
                m_currentlyPointingInstrument.OnReleasedPointing();

            instrument.SetOutlineColor(selectableOutlineColor);
            instrument.OnPointing();
            m_currentlyPointingInstrument = instrument;
        }
        else if (instrument == null) // If no hit
        {
            if (m_currentlyPointingInstrument != null)
            {
                m_currentlyPointingInstrument.OnReleasedPointing();
                m_currentlyPointingInstrument = null;
            }
        }
    }
}
