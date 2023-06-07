using CM_BSOR.Controllers;
using CM_BSOR.Helpers;

namespace CM_BSOR
{
    [Plugin("ChroMapper BeatLeader Open Replay Viewer")]
    public class Plugin
    {
        private OpenReplayController openReplayController = null!;

        [Init]
        public void Init()
        {
            openReplayController = new();
        }
    }
}
