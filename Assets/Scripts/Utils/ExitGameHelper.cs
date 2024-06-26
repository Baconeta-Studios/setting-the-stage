using UnityEngine;
using Utils;

public class ExitGameHelper : EverlastingSingleton<ExitGameHelper>
{
    [SerializeField] private GameObject _exitButton;

    public void OnEnable()
    {
        // The exit button will be hidden on mobile and WebGL platforms.
        if (UnityHelper.IsMobile || UnityHelper.IsWebGL)
        {
            _exitButton.SetActive(false);
        } else {
            _exitButton.SetActive(true);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
