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
        /// <summary>
        /// Attempts to decode a byte <paramref name="buffer"/>, which ideally represents a valid BeatLeader replay.
        /// </summary>
        /// <param name="buffer">Buffer of bytes, ideally representing an encoded replay.</param>
        /// <param name="replay">If the decode operation was successful, <paramref name="replay"/> is assigned the decoded BeatLeader replay.</param>
        /// <returns>Whether or not the operation was successful.</returns>
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

        /// <summary>
        /// Decodes a byte <paramref name="buffer"/> into a BeatLeader replay.
        /// </summary>
        /// <param name="buffer">Byte array</param>
        /// <returns><see cref="null"/> if the BeatLeader replay is invalid, a <see cref="SimpleReplay"/> otherwise.</returns>
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

        /// <summary>
        /// Decodes the first block of a BeatLeader replay: the Metadata block.
        /// Contains metadata on the game, player, and song.
        /// </summary>
        /// <param name="buffer">Encoded BeatLeader replay.</param>
        /// <param name="head">Read head, which automatically gets incremented.</param>
        /// <returns>A decoded <see cref="SimpleReplay.ReplayMetadata"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SimpleReplay.ReplayMetadata DecodeMetadata(byte[] buffer, ref int head)
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

        /// <summary>
        /// Decodes the second block of a BeatLeader replay: HMD and Controller keyframes.
        /// </summary>
        /// <param name="buffer">Encoded BeatLeader replay.</param>
        /// <param name="head">Read head, which automatically gets incremented.</param>
        /// <returns>A decoded list of <see cref="SimpleReplay.Frame"/>s.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<SimpleReplay.Frame> DecodeFrames(byte[] buffer, ref int head)
        {
            DecodeByte(buffer, ref head); // Struct type (always frame list)

            var length = DecodeInt(buffer, ref head);
            var frames = new List<SimpleReplay.Frame>(length);

            for (var i = 0; i < length; i++)
                frames.Add(DecodeFrame(buffer, ref head));

            return frames;
        }

        /// <summary>
        /// Decodes a single frame of keyframe data.
        /// </summary>
        /// <param name="buffer">Encoded BeatLeader replay.</param>
        /// <param name="head">Read head, which automatically gets incremented.</param>
        /// <returns>A decoded <see cref="SimpleReplay.Frame"/>.</returns>
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

        /// <summary>
        /// Decodes a Unity <see cref="Pose"/>, which contains a position and rotation component.
        /// </summary>
        /// <param name="buffer">Encoded BeatLeader replay.</param>
        /// <param name="head">Read head, which automatically gets incremented.</param>
        /// <returns>A decoded <see cref="Pose"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Pose DecodePose(byte[] buffer, ref int head)
            => new(DecodeVector3(buffer, ref head), DecodeQuarternion(buffer, ref head));

        /// <summary>
        /// Decodes a singular <see cref="Vector3"/>.
        /// </summary>
        /// <param name="buffer">Encoded BeatLeader replay.</param>
        /// <param name="head">Read head, which automatically gets incremented.</param>
        /// <returns>A decoded <see cref="Vector3"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 DecodeVector3(byte[] buffer, ref int head)
            => new(DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head));

        /// <summary>
        /// Decodes a singular <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="buffer">Encoded BeatLeader replay.</param>
        /// <param name="head">Read head, which automatically gets incremented.</param>
        /// <returns>A decoded <see cref="Quaternion"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Quaternion DecodeQuarternion(byte[] buffer, ref int head)
            => new(DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head), DecodeFloat(buffer, ref head));

        /// <summary>
        /// Decodes the Player name from a BeatLeader replay.
        /// </summary>
        /// <param name="buffer">Encoded BeatLeader replay.</param>
        /// <param name="head">Read head, which automatically gets incremented.</param>
        /// <returns>Decoded player name</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string DecodePlayerName(byte[] buffer, ref int head)
        {
            var length = DecodeInt(buffer, ref head);

            // WTF?????? what is the purpose of this, beatleader??
            var lengthOffset = 0;
            var offsetInt = BitConverter.ToInt32(buffer, length + head);
            while (offsetInt != 5 && offsetInt != 6 && offsetInt != 8)
            {
                lengthOffset++;
                offsetInt = BitConverter.ToInt32(buffer, length + head + lengthOffset);
            } 

            var result = Encoding.UTF8.GetString(buffer, head, length + lengthOffset);
            head += length + lengthOffset;
            return result;
        }

        /// <summary>
        /// Decodes a singular <see cref="string"/>.
        /// </summary>
        /// <param name="buffer">Encoded BeatLeader replay.</param>
        /// <param name="head">Read head, which automatically gets incremented.</param>
        /// <returns>Decoded <see cref="string"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string DecodeString(byte[] buffer, ref int head)
        {
            var strLength = DecodeInt(buffer, ref head);

            var result = Encoding.UTF8.GetString(buffer, head, strLength);
            head += strLength;
            return result;
        }

        /// <summary>
        /// Decodes a singular <see cref="int"/>.
        /// </summary>
        /// <param name="buffer">Encoded BeatLeader replay.</param>
        /// <param name="head">Read head, which automatically gets incremented.</param>
        /// <returns>Decoded <see cref="int"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int DecodeInt(byte[] buffer, ref int head)
        {
            var result = BitConverter.ToInt32(buffer, head);
            head += 4;
            return result;
        }

        /// <summary>
        /// Decodes a singular <see cref="float"/>.
        /// </summary>
        /// <param name="buffer">Encoded BeatLeader replay.</param>
        /// <param name="head">Read head, which automatically gets incremented.</param>
        /// <returns>Decoded <see cref="float"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float DecodeFloat(byte[] buffer, ref int head)
        {
            var result = BitConverter.ToSingle(buffer, head);
            head += 4;
            return result;
        }

        /// <summary>
        /// Returns the byte at the read head.
        /// </summary>
        /// <param name="buffer">Encoded BeatLeader replay.</param>
        /// <param name="head">Read head, which automatically gets incremented.</param>
        /// <returns>Current byte.</returns>
        // haha
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte DecodeByte(byte[] buffer, ref int head) => buffer[head++];

        /// <summary>
        /// Returns <see cref="true"/> if the byte at the read head is non-zero, <see cref="false"/> otherwise.
        /// </summary>
        /// <param name="buffer">Encoded BeatLeader replay.</param>
        /// <param name="head">Read head, which automatically gets incremented.</param>
        /// <returns><see cref="true"/> if the byte at the read head is non-zero, <see cref="false"/> otherwise.</returns>
        // hahahaha
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DecodeBool(byte[] buffer, ref int head) => buffer[head++] > 0;
    }
}
