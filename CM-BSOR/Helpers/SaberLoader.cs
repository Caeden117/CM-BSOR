using System.Reflection;
using UnityEngine;

namespace CM_BSOR.Helpers
{
    public class SaberLoader
    {
        private static GameObject saberPrefab = null!;
        private static GameObject headPrefab = null!;

        public static (GameObject left, GameObject right) LoadSabers()
        {
            if (saberPrefab == null)
            {
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CM_BSOR.Assets.Sabers.saber");
                
                var len = (int)stream.Length;
                var bytes = new byte[len];
                stream.Read(bytes, 0, len);

                var bundle = AssetBundle.LoadFromMemory(bytes);

                saberPrefab = CleanupGameObject(bundle.LoadAsset<GameObject>("_CustomSaber"));
            }

            var saberDuo = Object.Instantiate(saberPrefab);

            var left = saberDuo.transform.Find("LeftSaber").gameObject;
            var right = saberDuo.transform.Find("RightSaber").gameObject;

            return (left, right);
        }

        public static GameObject LoadHead()
        {
            if (headPrefab == null)
            {
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CM_BSOR.Assets.reaxt");
                
                var len = (int)stream.Length;
                var bytes = new byte[len];
                stream.Read(bytes, 0, len);

                var bundle = AssetBundle.LoadFromMemory(bytes);

                headPrefab = CleanupGameObject(bundle.LoadAsset<GameObject>("Head"));
            }

            return Object.Instantiate(headPrefab);
        }

        private static GameObject CleanupGameObject(GameObject obj)
        {
            var components = obj.GetComponents<Component>();

            for (var i = 0; i < components.Length; i++)
            {
                if (components[i] == null) Object.Destroy(components[i]);
            }

            return obj;
        }
    }
}
