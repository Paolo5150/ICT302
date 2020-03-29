using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoBehaviour
{
    public delegate void OnInstrumentSelected(Instrument.INSTRUMENT_TAG instrumentTag);
    public static event OnInstrumentSelected instrumentSelectedEvent;

    public float raycastLength = 150.0f;
    public Color selectableOutlineColor = Color.white;
    public float itemViewRotationSpeed = 50.0f;
    public float itemViewMovementSpeed = 50.0f;
    public float itemViewMovementLimitRange = 50.0f;


    private InstrumentSelector m_instrumentSelector;
    private FirstPersonController m_firstPersonController;
    private GameObject m_zoomViewSpot;
    private Instrument m_currentlyPointingInstrument;
    private bool m_pickingEnabled;
    private bool m_viewingEnabled;


    public enum PlayerMode
    {
        VIEWING,
        FREE
    }

    private PlayerMode m_playerMode;

    // Start is called before the first frame update
    void Start()
    {
        // Initialization is done in Init(), called by the game manager
    }

    public void Init()
    {
        m_playerMode = PlayerMode.FREE;

        m_instrumentSelector = new InstrumentSelector();
        m_firstPersonController = GetComponent<FirstPersonController>();

        Camera cam = GetComponentInChildren<Camera>();
        m_instrumentSelector.SetRaycastingCamera(cam);

        m_zoomViewSpot = transform.GetChild(1).gameObject; //Might need to change this
        m_instrumentSelector.SetSelectableOutlineColor(selectableOutlineColor);
        m_pickingEnabled = true;
        m_viewingEnabled = true;
    }

    public void SetPickingEnabled(bool enabled)
    {
        m_pickingEnabled = enabled;
    }

    public void SetViewingEnabled(bool enabled)
    {
        m_viewingEnabled = enabled;
    }

    private void SetPlayerMode(PlayerMode mode)
    {
        switch(mode)
        {
            case PlayerMode.FREE:
                m_firstPersonController.enabled = true;
                m_pickingEnabled = true;
                m_viewingEnabled = true;
                break;
            case PlayerMode.VIEWING:                    
                m_firstPersonController.enabled = false;
                m_pickingEnabled = false;
                m_currentlyPointingInstrument.SetEnableOutline(false);
                m_viewingEnabled = true;
                break;
        }
        m_playerMode = mode;
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_playerMode)
        {
            case PlayerMode.FREE:
                FreeMode();
                break;
            case PlayerMode.VIEWING:
                ViewMode();
                break;
        }
    }

    private void FreeMode()
    {
        if (m_pickingEnabled)
        {
            UpdatePointingInstrument();

            if (Input.GetButtonDown("Fire1"))
            {
                if (m_currentlyPointingInstrument != null)
                {
                    SetPlayerMode(PlayerMode.VIEWING);
                    StartCoroutine(m_instrumentSelector.LerpToPosition(m_currentlyPointingInstrument.gameObject, m_zoomViewSpot.transform.position));
                }
            }
        }
    }

    public void ResetPlayerMode()
    {
        // Put item back
        if(m_currentlyPointingInstrument != null)
        {
            StartCoroutine(m_instrumentSelector.LerpToPosition(m_currentlyPointingInstrument.gameObject, m_currentlyPointingInstrument.originalPosition));
            m_currentlyPointingInstrument.gameObject.transform.rotation = m_currentlyPointingInstrument.originalRotation;
            m_currentlyPointingInstrument = null;
        }

        SetPlayerMode(PlayerMode.FREE);

    }

    private void ViewMode()
    {
        if(m_currentlyPointingInstrument != null && m_viewingEnabled)
        {
            // Manipulate object being viewed
            m_currentlyPointingInstrument.gameObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0) * Time.deltaTime * itemViewRotationSpeed, Space.World);

            float distance = (m_zoomViewSpot.transform.position - m_currentlyPointingInstrument.gameObject.transform.position).magnitude;
            if (distance < itemViewMovementLimitRange)
            {
                m_currentlyPointingInstrument.gameObject.transform.position += transform.forward * itemViewMovementSpeed * Input.GetAxis("Vertical");
                m_currentlyPointingInstrument.gameObject.transform.position += transform.right * itemViewMovementSpeed * Input.GetAxis("Horizontal");
            }
            else
            {
                Vector3 goBack = m_zoomViewSpot.transform.position - m_currentlyPointingInstrument.gameObject.transform.position;
                m_currentlyPointingInstrument.gameObject.transform.position += goBack.normalized * 0.2f;
            }

            // Confirm selection
            if (Input.GetButtonDown("Fire1"))
            {
                instrumentSelectedEvent(m_currentlyPointingInstrument.instrumentTag);

            }
            // Put item back
            if (Input.GetButtonDown("Fire2"))
            {
                StartCoroutine(m_instrumentSelector.LerpToPosition(m_currentlyPointingInstrument.gameObject, m_currentlyPointingInstrument.originalPosition));
                m_currentlyPointingInstrument.gameObject.transform.rotation = m_currentlyPointingInstrument.originalRotation;
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

    public void SetMovementEnabled(bool enabled)
    {
        m_firstPersonController.enabled = enabled;
    }
}
