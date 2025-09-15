using Utils;

namespace Managers
{
    public class GameDebugManager : EverlastingSingleton<GameDebugManager>
    {
        public bool isDemo = false;
        public bool cheatsEnabled;
    }
}