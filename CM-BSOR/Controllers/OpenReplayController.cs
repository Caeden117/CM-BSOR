using CM_BSOR.Helpers;
using SFB;
using System.IO;
using UnityEngine;

namespace CM_BSOR.Controllers
{
    public class OpenReplayController
    {
        private ReplayPlaybackController playbackController = null!;

        public OpenReplayController()
        {
            ExtensionButtons.AddButton(ImageLoader.LoadImage(), "Open BeatLeader Replay", PromptForReplay);
        }

        private void PromptForReplay()
        {
            StandaloneFileBrowser.OpenFilePanelAsync("Open BeatLeader Replay", Directory.GetCurrentDirectory(), "bsor", false, OnSelectReplay);
        }

        private void OnSelectReplay(string[] obj)
        {
            if (obj == null || obj.Length == 0) return;

            var fileLocation = obj[0];

            if (!File.Exists(fileLocation)) return;

            using var fileStream = File.OpenRead(fileLocation);
            
            var length = (int)fileStream.Length;
            var buffer = new byte[length];
            fileStream.Read(buffer, 0, length);

            var replay = BLReplaySimpleDecoder.Decode(buffer)!;

            if (playbackController == null)
            {
                playbackController = new GameObject("Replay Playback").AddComponent<ReplayPlaybackController>();
            }

            playbackController.AssignReplay(replay);
        }
    }
}
