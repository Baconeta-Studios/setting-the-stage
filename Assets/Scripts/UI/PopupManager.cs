using System.Collections.Generic;
using Utils;

public class PopupManager : EverlastingSingleton<PopupManager>
{
    private readonly List<Popup> _activePopups = new();

    public void Register(Popup popup)
    {
        if (!_activePopups.Contains(popup))
            _activePopups.Add(popup);
    }

    public void Unregister(Popup popup)
    {
        _activePopups.Remove(popup);
    }

    public void CloseAll()
    {
        foreach (var popup in _activePopups.ToArray()) // copy in case popups unregister during close
        {
            popup.Close();
        }
    }
}