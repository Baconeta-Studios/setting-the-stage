using UnityEngine;
using Utils;

public class ExitGameHelper : EverlastingSingleton<ExitGameHelper>
{
    public void ExitGame()
    {
        Application.Quit();
    }
}
