using UnityEngine;

public abstract class Popup : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        PopupManager.Instance.Register(this);
    }
    
    protected virtual void OnDisable()
    {
        PopupManager.Instance.Unregister(this);
    }

    // Can be called by PopupManager or any other class (and overridden for unique behaviour)
    public void Close()
    {
        HideGameObject();
    }

    private void HideGameObject()
    {
        gameObject.SetActive(false);
    }
}