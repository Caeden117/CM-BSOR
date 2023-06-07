using CM_BSOR.Controllers;
using CM_BSOR.Helpers;

namespace CM_BSOR
{
    [Plugin(nameof(CM_BSOR))]
    public class Plugin
    {
        private OpenReplayController openReplayController = new();

        [Init]
        public void Init() => ImageLoader.LoadImages();
    }
}
