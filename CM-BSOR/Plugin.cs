using CM_BSOR.Controllers;
using CM_BSOR.Helpers;

namespace CM_BSOR
{
    [Plugin("ChroMapper BeatLeader Open Replay Viewer")]
    public class Plugin
    {
        private OpenReplayController openReplayController = new();

        [Init]
        public void Init() => ImageLoader.LoadImages();
    }
}
