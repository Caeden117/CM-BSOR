using System;
using System.Reflection;
using UnityEngine;

namespace CM_BSOR.Helpers
{
    public static class ImageLoader
    {
        public static Sprite IconSprite = null!;

        public static void LoadImages()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CM_BSOR.Images.Icon.png");
            
            var len = (int)stream.Length;
            var bytes = new byte[len];
            stream.Read(bytes, 0, len);

            var texture2D = new Texture2D(307, 307);
            texture2D.LoadImage(bytes);

            var rect = new Rect(0, 0, texture2D.width, texture2D.height);

            IconSprite = Sprite.Create(texture2D, rect, Vector2.zero, 100.0f, 0, SpriteMeshType.Tight);
        }
    }
}
