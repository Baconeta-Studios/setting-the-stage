using Utils;

namespace Managers
{
    public class GameDebugManager : Singleton<GameDebugManager>
    {
        public bool isDemo = false;
        public bool cheatsEnabled;
    }
}