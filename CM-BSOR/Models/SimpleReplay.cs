using System;
using System.Collections.Generic;
using UnityEngine;

namespace CM_BSOR.Models
{
    /// <summary>
    /// Simplified Beat Leader Replay data structure for playback
    /// </summary>
    public class SimpleReplay
    {
        public static int MAGIC_NUMBER = 0x442D3D69;

        public ReplayMetadata Metadata = null!;
        public List<Frame> Frames = null!;

        public class ReplayMetadata
        {
            public string BeatLeaderVersion = null!;
            public string GameVersion = null!;
            public string Timestamp = null!;

            public string PlayerID = null!;
            public string PlayerName = null!;
            public string Platform = null!;

            public string TrackingSystem = null!;
            public string HMD = null!;
            public string Controller = null!;

            public string SongHash = null!;
            public string SongName = null!;
            public string Mapper = null!;
            public string Difficulty = null!;

            public int Score = -1;
            public string GameMode = null!;
            public string Environment = null!;
            public string Modifiers = null!;
            public float JumpDistance = -1;
            public bool LeftHanded = false;
            public float Height = -1;

            public float StartTime = -1;
            public float EndTime = -1;
            public float Speed = -1;
        }

        public class Frame
        {
            public float Time;
            public int FPS;
            public Pose Head;
            public Pose LeftHand;
            public Pose RightHand;
        }
    }
}
