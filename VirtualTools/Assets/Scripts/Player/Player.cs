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
                m_instrumentSelector.GetCurrentlyPointingIstrument().SetEnableOutline(false);
                break;
        }
        m_playerMode = mode;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_playerMode == PlayerMode.FREE)
        {
            m_instrumentSelector.RaycastFromCamera(raycastLength);
            if (Input.GetButton("Fire1"))
            {
                if (m_instrumentSelector.GetCurrentlyPointingIstrument() != null)
                {
                    SetPlayerMode(PlayerMode.VIEWING);
                    m_instrumentSelector.GetCurrentlyPointingIstrument().gameObject.transform.SetParent(m_zoomViewSpot.gameObject.transform);
                    m_instrumentSelector.GetCurrentlyPointingIstrument().gameObject.transform.transform.localPosition = new Vector3(0, 0, 0);
                    m_instrumentSelector.GetCurrentlyPointingIstrument().gameObject.transform.transform.localRotation = Quaternion.identity;
                }
            }
        }
        else
        {

        }

    }
}
