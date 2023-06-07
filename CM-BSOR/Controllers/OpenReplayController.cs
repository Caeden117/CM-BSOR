using CM_BSOR.Helpers;
using SFB;
using System;
using System.IO;
using System.IO.Pipes;
using UnityEngine;

namespace CM_BSOR.Controllers
{
    internal class OpenReplayController
    {
        public OpenReplayController()
        {
            ExtensionButtons.AddButton(ImageLoader.IconSprite, "Open BeatLeader Replay", PromptForReplay);
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
#if NETSTANDARD2_1_OR_GREATER
            Debug.LogError("WHAT THE FUCK");
            Span<byte> buffer = stackalloc byte[length];
            fileStream.Read(buffer);
#else
            var buffer = new byte[length];
            fileStream.Read(buffer, 0, length);
#endif

            if (!BLReplaySimpleDecoder.TryDecode(buffer, out var replay)) return;

            Debug.LogError($"Replay loaded with {replay.Frames.Count} frames");
        }
    }
}
