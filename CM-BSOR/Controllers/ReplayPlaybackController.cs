using CM_BSOR.Helpers;
using CM_BSOR.Models;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CM_BSOR.Controllers
{
    public class ReplayPlaybackController : MonoBehaviour
    {
        private GameObject headGameObject = null!;
        private Transform headTransform = null!;
        private Transform leftHandTransform = null!;
        private Transform rightHandTransform = null!;

        private AudioTimeSyncController atsc = null!;

        private SimpleReplay activeReplay = null!;
        private SimpleReplay.Frame lastFrame = null!;
        private int lastFrameIndex = -1;

        private UIModeType currentUIMode = UIModeType.Normal;

        public void AssignReplay(SimpleReplay replay)
        {
            activeReplay = replay;
            lastFrameIndex = 0;
            lastFrame = activeReplay.Frames[0];
        }

        private void Start()
        {
            // Load head and saber assets
            headGameObject = SaberLoader.LoadHead();
            headTransform = headGameObject.transform;

            (var left, var right) = SaberLoader.LoadSabers();
            leftHandTransform = left.transform;
            rightHandTransform = right.transform;

            // Assign ourselves as the parent (makes it easier to move things around)
            headTransform.SetParent(transform, true);
            leftHandTransform.SetParent(transform, true);
            rightHandTransform.SetParent(transform, true);

            // CM scale is different from Beat Saber, try to match as best as we can
            // CONTRIBUTOR! This is where scaling gets set, if you want to change the size of the player.
            transform.localScale = 4f / 3 * Vector3.one;
            transform.position = LoadInitialMap.PlatformOffset;
            headTransform.localScale = 0.75f * Vector3.one;

            // haha this is stupid but also Stable/Dev interop
            atsc = FindObjectOfType<AudioTimeSyncController>();

            UIMode.UIModeSwitched += OnUIModeSwitched;

            // Pre-emptive animations branch support
            var animationsPlayerTrack = GameObject.Find("Player Camera");
            if (animationsPlayerTrack != null)
            {
                transform.SetParent(animationsPlayerTrack.transform, true);
            }
        }

        private void Update()
        {
            if (lastFrame == null) return;
            
            var keyframes = activeReplay.Frames;
            var time = atsc.CurrentSeconds;

            var direction = time.CompareTo(lastFrame.Time);
            var newDirection = direction;

            if (direction == 0) return;

            while (newDirection == direction)
            {
                var nextFrameIndex = lastFrameIndex + direction;
                
                if (nextFrameIndex < 0 || nextFrameIndex >= keyframes.Count) return;

                lastFrame = keyframes[nextFrameIndex];
                lastFrameIndex = nextFrameIndex;
                newDirection = time.CompareTo(lastFrame.Time);
            }

            var nextKeyframeIndex = lastFrameIndex + 1;

            if (nextKeyframeIndex >= keyframes.Count) return;

            var nextKeyframe = keyframes[nextKeyframeIndex];

            var t = (time - lastFrame.Time) / (nextKeyframe.Time - lastFrame.Time);

            // Update head transform iff the cursor is unlcocked
            if (currentUIMode != UIModeType.Playing)
            {
                LerpIntoTransform(headTransform, in lastFrame.Head, in nextKeyframe.Head, in t);
            }

            LerpIntoTransform(leftHandTransform, in lastFrame.LeftHand, in nextKeyframe.LeftHand, in t);
            LerpIntoTransform(rightHandTransform, in lastFrame.RightHand, in nextKeyframe.RightHand, in t);
        }

        private void OnUIModeSwitched(UIModeType newUIMode)
        {
            currentUIMode = newUIMode;
            headGameObject.SetActive(currentUIMode != UIModeType.Playing);
        }

        private void OnDestroy()
        {
            UIMode.UIModeSwitched -= OnUIModeSwitched;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LerpIntoTransform(Transform transform, in Pose lastPose, in Pose nextPose, in float t)
        {
            var lerped = LerpPose(in lastPose, in nextPose, in t);

            // Unfortunately, SetPositionAndRotation only applies to world space!
            //   ...That, and SetLocalPositionAndRotation was added in Unity 2022.1 :(
            transform.localPosition = lerped.position;
            transform.localRotation = lerped.rotation;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Pose LerpPose(in Pose a, in Pose b, in float t)
            => new(FastLerpVector3(in a.position, in b.position, in t),
                Quaternion.LerpUnclamped(a.rotation, b.rotation, t));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector3 FastLerpVector3(in Vector3 a, in Vector3 b, in float t)
            => new(FastLerp(in a.x, in b.x, in t),
                FastLerp(in a.y, in b.y, in t),
                FastLerp(in a.z, in b.z, in t));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float FastLerp(in float a, in float b, in float t) => a + (b - a) * t;
    }
}
