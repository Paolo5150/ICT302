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
    public GameObject m_zoomViewSpot;


    private InstrumentSelector m_instrumentSelector;
    private FirstPersonController m_firstPersonController;
    private Instrument m_currentlyPointingInstrument;
    private bool m_isQuitting = false; //Used when the user presses the cancel axis to mimic GetButtonDown
    public bool m_pickingEnabled;
    public bool m_viewingEnabled;

    public enum PlayerMode
    {
        VIEWING,
        PICKING
    }

    private PlayerMode m_playerMode;
    private static Player m_instance;
    public static Player Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject coreGameObject = GameObject.Find("Player");
                m_instance = coreGameObject.AddComponent<Player>();
            }

            return m_instance;
        }
    }

    protected virtual void Awake()
    {
        if (m_instance == null)
            m_instance = GetComponent<Player>();
        else
            DestroyImmediate(this);
    }


    // Start is called before the first frame update
    void Start()
    {
        // Initialization is done in Init(), called by the game manager
    }

    public void Init()
    {
        m_playerMode = PlayerMode.PICKING;

        m_instrumentSelector = new InstrumentSelector();
        m_firstPersonController = GetComponent<FirstPersonController>();

        Camera cam = GetComponentInChildren<Camera>();
        m_instrumentSelector.SetRaycastingCamera(cam);


        m_instrumentSelector.SetSelectableOutlineColor(selectableOutlineColor);
        m_pickingEnabled = true;
        m_viewingEnabled = true;
    }

    public void FreezePlayer(bool freeze)
    {
        GetComponent<FirstPersonController>().enabled = !freeze;
        enabled = !freeze;
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
            case PlayerMode.PICKING:
                m_firstPersonController.enabled = true;

                break;
            case PlayerMode.VIEWING:                    
                m_firstPersonController.enabled = false;
                break;
        }
        m_playerMode = mode;
    }

    private void ProcessInput()
    {
        if (Input.GetAxis("Cancel") == 1 && !m_isQuitting)
        {
            m_isQuitting = true;
            SessionManager.Instance.OnQuit();
            Application.Quit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();

        switch (m_playerMode)
        {
            case PlayerMode.PICKING:
                PickingMode();
                GUIManager.Instance.GetMainCanvas().SetHintActive(false);

                break;
            case PlayerMode.VIEWING:
                ViewMode();
                GUIManager.Instance.GetMainCanvas().SetHintActive(true);

                break;
        }
    }

    private void PickingMode()
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

    public void ResetItemAndPlayerToFree()
    {
        // Put item back
        if(m_currentlyPointingInstrument != null)
        {
            StartCoroutine(m_instrumentSelector.LerpToPosition(m_currentlyPointingInstrument.gameObject, m_currentlyPointingInstrument.originalPosition));
            m_currentlyPointingInstrument.gameObject.transform.rotation = m_currentlyPointingInstrument.originalRotation;
            m_currentlyPointingInstrument = null;
        }

        SetPlayerMode(PlayerMode.PICKING);
    }

    private void ViewMode()
    {
        if(m_currentlyPointingInstrument != null && m_viewingEnabled)
        {

            m_currentlyPointingInstrument.OnReleasedPointing();
            // Manipulate object being viewed
            m_currentlyPointingInstrument.gameObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0) * Time.deltaTime * itemViewRotationSpeed, Space.World);

            Vector3 dir =  m_currentlyPointingInstrument.gameObject.transform.position - m_zoomViewSpot.transform.position;
            float distance = dir.magnitude;
            if (distance < itemViewMovementLimitRange)
            {
                m_currentlyPointingInstrument.gameObject.transform.position += Camera.main.transform.forward * itemViewMovementSpeed * Input.GetAxis("Vertical");
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
                SetPlayerMode(PlayerMode.PICKING);
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
