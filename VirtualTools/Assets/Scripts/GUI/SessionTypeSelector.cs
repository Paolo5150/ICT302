using UnityEngine;

public class SessionTypeSelector : MonoBehaviour
{
	public void BeginSelectByNameSession()
	{
        SessionManager.Instance.StartSessionByType(Session.SESSION_TYPE.SELECT_BY_NAME);
		GUIManager.Instance.ConfigureCursor(false, CursorLockMode.Locked);
		GUIManager.Instance.GetMainCanvas().SetEscapeMenu(false);
		GUIManager.Instance.GetMainCanvas().SetSceneSelector(false);
	}

	public void BeginSelectByPurposeSession()
	{
        SessionManager.Instance.StartSessionByType(Session.SESSION_TYPE.SELECT_BY_PURPOSE);
        GUIManager.Instance.ConfigureCursor(false, CursorLockMode.Locked);
		GUIManager.Instance.GetMainCanvas().SetEscapeMenu(false);
		GUIManager.Instance.GetMainCanvas().SetSceneSelector(false);
	}

	public void BeginSelectInstrumentPositionSession()
	{
        SessionManager.Instance.StartSessionByType(Session.SESSION_TYPE.INSTRUMENT_POSITIONING);
        GUIManager.Instance.ConfigureCursor(false, CursorLockMode.Locked);
		GUIManager.Instance.GetMainCanvas().SetEscapeMenu(false);
		GUIManager.Instance.GetMainCanvas().SetSceneSelector(false);
	}
}
