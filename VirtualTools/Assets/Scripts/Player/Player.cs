using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoBehaviour
{
    public float raycastLength = 150.0f;
    public Color selectableOutlineColor = Color.white;
    public float itemViewRotationSpeed = 50.0f;
    public float itemViewMovementSpeed = 50.0f;
    public float itemViewMovementLimitRange = 50.0f;


    private InstrumentSelector m_instrumentSelector;
    private FirstPersonController m_firstPersonController;
    private GameObject m_zoomViewSpot;
    private Instrument m_currentlyPointingInstrument;

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
        UpdatePointingInstrument();

        if (Input.GetButton("Fire1"))
        {
            if (m_currentlyPointingInstrument != null)
            {
                SetPlayerMode(PlayerMode.VIEWING);
                StartCoroutine(m_instrumentSelector.LerpToPosition(m_currentlyPointingInstrument.gameObject, m_zoomViewSpot.transform.position));
            }
        }
    }

    private void ViewMode()
    {
        // Manipulate object being viewed
        m_currentlyPointingInstrument.gameObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0) * Time.deltaTime * itemViewRotationSpeed, Space.World);

        float distance = (m_zoomViewSpot.transform.position - m_currentlyPointingInstrument.gameObject.transform.position).magnitude;
        if(distance < itemViewMovementLimitRange)
        {
            m_currentlyPointingInstrument.gameObject.transform.position += transform.forward * itemViewMovementSpeed * Input.GetAxis("Vertical");
            m_currentlyPointingInstrument.gameObject.transform.position += transform.right * itemViewMovementSpeed * Input.GetAxis("Horizontal");
        }
        else
        {
            Vector3 goBack = m_zoomViewSpot.transform.position - m_currentlyPointingInstrument.gameObject.transform.position;
            m_currentlyPointingInstrument.gameObject.transform.position += goBack.normalized * 0.2f;
        }



        if (Input.GetButton("Fire2"))
        {
            StartCoroutine(m_instrumentSelector.LerpToPosition(m_currentlyPointingInstrument.gameObject, m_currentlyPointingInstrument.originalPosition));
            m_currentlyPointingInstrument.gameObject.transform.rotation = m_currentlyPointingInstrument.originalRotation;
            SetPlayerMode(PlayerMode.FREE);
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
