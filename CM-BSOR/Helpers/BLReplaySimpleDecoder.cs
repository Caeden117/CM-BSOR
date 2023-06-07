using CM_BSOR.Models;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CM_BSOR.Helpers
{
    // Custom implementation because, as of the time of coding (2023/06/06), BeatLeader's C# implementation is unlicensed.
    public static class BLReplaySimpleDecoder
    {
        // FUUUUUUCK THE UNITY RUNTIME DOESNT SUPPORT .NET STANDARD 2.1 AAAAAAHHHH
#if NETSTANDARD2_1_OR_GREATER
        public static bool TryDecode(Span<byte> buffer, out SimpleReplay replay)
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

        public static SimpleReplay? Decode(Span<byte> buffer)
        {
            var head = 0;
            var magicNumber = DecodeInt(buffer, ref head);

            if (magicNumber != SimpleReplay.MAGIC_NUMBER) return null;

            return new()
            {
                Metadata = DecodeMetadata(buffer, ref head),
                Frames = DecodeFrames(buffer, ref head),
            };
        }

        public static SimpleReplay.ReplayMetadata DecodeMetadata(Span<byte> buffer, ref int head)
        {
            DecodeByte(buffer, ref head);

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

        public static List<SimpleReplay.Frame> DecodeFrames(Span<byte> buffer, ref int head)
        {
            DecodeByte(buffer, ref head);

            var length = DecodeInt(buffer, ref head);
            var frames = new List<SimpleReplay.Frame>(length);

            for (var i = 0; i < length; i++)
                frames.Add(DecodeFrame(buffer, ref head));

            return frames;
        }

        private static SimpleReplay.Frame DecodeFrame(Span<byte> buffer, ref int head)
            => new()
            {
                Time = DecodeFloat(buffer, ref head),
                FPS = DecodeInt(buffer, ref head),
                Head = DecodePose(buffer, ref head),
                LeftHand = DecodePose(buffer, ref head),
                RightHand = DecodePose(buffer, ref head),
            };

        private static Pose DecodePose(Span<byte> buffer, ref int head)
            => new(DecodeVector3(buffer, ref head), DecodeQuarternion(buffer, ref head));

        private static Vector3 DecodeVector3(Span<byte> buffer, ref int head)
            => new(DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head));

        private static Quaternion DecodeQuarternion(Span<byte> buffer, ref int head)
            => new(DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head));

        // What is special with this method?
        private static string DecodePlayerName(Span<byte> buffer, ref int head)
        {
            var strLength = DecodeInt(buffer, ref head);

            // WTF?????? what is the purpose
            var offsetHead = head + strLength - 1;
            int offsetInt;
            do
            {
                offsetInt = DecodeInt(buffer, ref offsetHead);
            } while (offsetInt != 5 && offsetHead != 6 && offsetInt != 8);

            Span<byte> strSlice = buffer[head..offsetHead];
            head += offsetHead - head;
            return Encoding.UTF8.GetString(strSlice);
        }

        private static string DecodeString(Span<byte> buffer, ref int head)
        {
            var strLength = DecodeInt(buffer, ref head);

            var strSlice = buffer[head..strLength];
            head += strLength;
            return Encoding.UTF8.GetString(strSlice);
        }

        private static int DecodeInt(Span<byte> buffer, ref int head)
        {
            Span<byte> slice = buffer[head..sizeof(int)];
            head += sizeof(int);
            return BitConverter.ToInt32(slice);
        }

        private static float DecodeFloat(Span<byte> buffer, ref int head)
        {
            var slice = buffer[head..sizeof(float)];
            head += sizeof(float);
            return BitConverter.ToSingle(slice);
        }

        // haha
        private static byte DecodeByte(Span<byte> buffer, ref int head) => buffer[head++];

        private static bool DecodeBool(Span<byte> buffer, ref int head) => buffer[head++] > 0;
#endif
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

            if (magicNumber != SimpleReplay.MAGIC_NUMBER) return null;

            return new()
            {
                Metadata = DecodeMetadata(buffer, ref head),
                Frames = DecodeFrames(buffer, ref head),
            };
        }

        public static SimpleReplay.ReplayMetadata DecodeMetadata(byte[] buffer, ref int head)
        {
            DecodeByte(buffer, ref head);

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

        public static List<SimpleReplay.Frame> DecodeFrames(byte[] buffer, ref int head)
        {
            DecodeByte(buffer, ref head);

            var length = DecodeInt(buffer, ref head);
            var frames = new List<SimpleReplay.Frame>(length);

            for (var i = 0; i < length; i++)
                frames.Add(DecodeFrame(buffer, ref head));

            return frames;
        }

        private static SimpleReplay.Frame DecodeFrame(byte[] buffer, ref int head)
            => new()
            {
                Time = DecodeFloat(buffer, ref head),
                FPS = DecodeInt(buffer, ref head),
                Head = DecodePose(buffer, ref head),
                LeftHand = DecodePose(buffer, ref head),
                RightHand = DecodePose(buffer, ref head),
            };

        private static Pose DecodePose(byte[] buffer, ref int head)
            => new(DecodeVector3(buffer, ref head), DecodeQuarternion(buffer, ref head));

        private static Vector3 DecodeVector3(byte[] buffer, ref int head)
            => new(DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head));

        private static Quaternion DecodeQuarternion(byte[] buffer, ref int head)
            => new(DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head));

        // What is special with this method?
        private static string DecodePlayerName(byte[] buffer, ref int head)
        {
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
            return result;
        }

        private static string DecodeString(byte[] buffer, ref int head)
        {
            var strLength = DecodeInt(buffer, ref head);

            var result = Encoding.UTF8.GetString(buffer, head, strLength);
            head += strLength;
            return result;
        }

        private static int DecodeInt(byte[] buffer, ref int head)
        {
            var result = BitConverter.ToInt32(buffer, head);
            head += sizeof(int);
            return result;
        }

        private static float DecodeFloat(byte[] buffer, ref int head)
        {
            var result = BitConverter.ToSingle(buffer, head);
            head += sizeof(float);
            return result;
        }

        // haha
        private static byte DecodeByte(byte[] buffer, ref int head) => buffer[head++];

        // hahahaha
        private static bool DecodeBool(byte[] buffer, ref int head) => buffer[head++] > 0;
    }
}
