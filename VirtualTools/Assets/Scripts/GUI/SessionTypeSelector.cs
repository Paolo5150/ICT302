using UnityEngine;

public class SessionTypeSelector : MonoBehaviour
{
	public void BeginSelectByNameSession()
	{
		SessionManager.Instance.StartSelectByNameSession();
		GUIManager.Instance.ConfigureCursor(false, CursorLockMode.Locked);
		GUIManager.Instance.GetMainCanvas().SetEscapeMenu(false);
		GUIManager.Instance.GetMainCanvas().SetSceneSelector(false);
	}

	public void BeginSelectByPurposeSession()
	{
		SessionManager.Instance.StartSelectByPurposeSession();
		GUIManager.Instance.ConfigureCursor(false, CursorLockMode.Locked);
		GUIManager.Instance.GetMainCanvas().SetEscapeMenu(false);
		GUIManager.Instance.GetMainCanvas().SetSceneSelector(false);
	}

	public void BeginSelectInstrumentPositionSession()
	{
		SessionManager.Instance.StartInstrumentPositioningSession();
		GUIManager.Instance.ConfigureCursor(false, CursorLockMode.Locked);
		GUIManager.Instance.GetMainCanvas().SetEscapeMenu(false);
		GUIManager.Instance.GetMainCanvas().SetSceneSelector(false);
	}
}
