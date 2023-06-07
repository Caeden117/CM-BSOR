using CM_BSOR.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace CM_BSOR.Helpers
{
    // Custom implementation because, as of the time of coding (2023/06/06), BeatLeader's C# implementation is unlicensed.
    public static class BLReplaySimpleDecoder
    {
        public static bool TryDecode(byte[] buffer, out SimpleReplay replay)
        {
            replay = null!;

            try
            {
                replay = Decode(buffer)!;
                return replay != null;
            }
            catch
            {
                return false;
            }
        }

        public static SimpleReplay? Decode(byte[] buffer)
        {
            var head = 0;
            var magicNumber = DecodeInt(buffer, ref head);
            var version = DecodeByte(buffer, ref head);

            if (magicNumber != SimpleReplay.MAGIC_NUMBER || version != 1) return null;

            return new()
            {
                Metadata = DecodeMetadata(buffer, ref head),
                Frames = DecodeFrames(buffer, ref head),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SimpleReplay.ReplayMetadata DecodeMetadata(byte[] buffer, ref int head)
        {
            DecodeByte(buffer, ref head); // Struct type (always metadata)

            return new()
            {
                BeatLeaderVersion = DecodeString(buffer, ref head),
                GameVersion = DecodeString(buffer, ref head),
                Timestamp = DecodeString(buffer, ref head),

                PlayerID = DecodeString(buffer, ref head),
                PlayerName = DecodePlayerName(buffer, ref head),
                Platform = DecodeString(buffer, ref head),

                TrackingSystem = DecodeString(buffer, ref head),
                HMD = DecodeString(buffer, ref head),
                Controller = DecodeString(buffer, ref head),

                SongHash = DecodeString(buffer, ref head),
                SongName = DecodeString(buffer, ref head),
                Mapper = DecodeString(buffer, ref head),
                Difficulty = DecodeString(buffer, ref head),

                Score = DecodeInt(buffer, ref head),
                GameMode = DecodeString(buffer, ref head),
                Environment = DecodeString(buffer, ref head),
                Modifiers = DecodeString(buffer, ref head),
                JumpDistance = DecodeFloat(buffer, ref head),
                LeftHanded = DecodeBool(buffer, ref head),
                Height = DecodeFloat(buffer, ref head),

                StartTime = DecodeFloat(buffer, ref head),
                EndTime = DecodeFloat(buffer, ref head),
                Speed = DecodeFloat(buffer, ref head),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<SimpleReplay.Frame> DecodeFrames(byte[] buffer, ref int head)
        {
            DecodeByte(buffer, ref head); // Struct type (always frame list)

            var length = DecodeInt(buffer, ref head);
            var frames = new List<SimpleReplay.Frame>(length);

            for (var i = 0; i < length; i++)
                frames.Add(DecodeFrame(buffer, ref head));

            return frames;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SimpleReplay.Frame DecodeFrame(byte[] buffer, ref int head)
            => new()
            {
                Time = DecodeFloat(buffer, ref head),
                FPS = DecodeInt(buffer, ref head),
                Head = DecodePose(buffer, ref head),
                LeftHand = DecodePose(buffer, ref head),
                RightHand = DecodePose(buffer, ref head),
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Pose DecodePose(byte[] buffer, ref int head)
            => new(DecodeVector3(buffer, ref head), DecodeQuarternion(buffer, ref head));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 DecodeVector3(byte[] buffer, ref int head)
            => new(DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Quaternion DecodeQuarternion(byte[] buffer, ref int head)
            => new(DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // What is special with this method?
        private static string DecodePlayerName(byte[] buffer, ref int head)
        {
            /*
            var strLength = DecodeInt(buffer, ref head);

            // WTF?????? what is the purpose
            var offsetHead = head + strLength - 1;
            int offsetInt;
            do
            {
                offsetInt = DecodeInt(buffer, ref offsetHead);
            } while (offsetInt != 5 && offsetHead != 6 && offsetInt != 8);

            var result = Encoding.UTF8.GetString(buffer, head, offsetHead - head);
            head += offsetHead - head;
            return result;*/
            int length = BitConverter.ToInt32(buffer, head);
            int lengthOffset = 0;
            if (length > 0)
            {
                while (BitConverter.ToInt32(buffer, length + head + 4 + lengthOffset) != 6
                    && BitConverter.ToInt32(buffer, length + head + 4 + lengthOffset) != 5
                    && BitConverter.ToInt32(buffer, length + head + 4 + lengthOffset) != 8)
                {
                    lengthOffset++;
                }
            }
            string @string = Encoding.UTF8.GetString(buffer, head + 4, length + lengthOffset);
            head += length + 4 + lengthOffset;
            return @string;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string DecodeString(byte[] buffer, ref int head)
        {
            var strLength = DecodeInt(buffer, ref head);

            var result = Encoding.UTF8.GetString(buffer, head, strLength);
            head += strLength;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int DecodeInt(byte[] buffer, ref int head)
        {
            var result = BitConverter.ToInt32(buffer, head);
            head += 4;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float DecodeFloat(byte[] buffer, ref int head)
        {
            var result = BitConverter.ToSingle(buffer, head);
            head += 4;
            return result;
        }

        // haha
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte DecodeByte(byte[] buffer, ref int head) => buffer[head++];

        // hahahaha
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DecodeBool(byte[] buffer, ref int head) => buffer[head++] > 0;
    }
}
