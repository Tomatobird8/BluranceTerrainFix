using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine.AI;

namespace BluranceTerrainFix
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class BluranceTerrainFix : BaseUnityPlugin
    {
        public static BluranceTerrainFix Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;

        public static ConfigEntry<bool> drawInstanced;


        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            drawInstanced = Config.Bind<bool>("General","DrawInstanced",true,"Whether the terrain should be drawn instanced. False allows the game's post processing to affect terrain rendering but makes quicksand barely visible. True keeps the terrain's old look.");

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");

            SceneManager.sceneLoaded += OnSceneLoaded;

        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "blurance")
            {
                return;
            }
            Terrain terrain = scene.GetRootGameObjects()[1].GetComponentInChildren<Terrain>();
            terrain.gameObject.tag = "Gravel";
            terrain.gameObject.layer = LayerMask.NameToLayer("Room");
            terrain.drawInstanced = drawInstanced.Value;
            GameObject[] ladderLinks = CreateArray<GameObject>(3);
            foreach (GameObject ladderLink in ladderLinks)
            {
                ladderLink.layer = LayerMask.NameToLayer("NavigationSurface");
                GameObject a = new();
                a.name = "A";
                a.transform.parent = ladderLink.transform;
                GameObject b = new();
                b.name = "B";
                b.transform.parent = ladderLink.transform;
                OffMeshLink link = ladderLink.AddComponent<OffMeshLink>();
                link.costOverride = -1; link.area = 4;
                link.startTransform = b.transform;
                link.endTransform = a.transform;
            }
            ladderLinks[0].name = "ShipLadderRight";
            ladderLinks[1].name = "ShipLadderFront";
            ladderLinks[2].name = "ShipLadderBack";
            ladderLinks[0].transform.position = new Vector3(-6.2389f, 1.0254f, -9.4513f);
            ladderLinks[0].transform.Find("A").position = new Vector3(-6.2389f, -0.5f, -7.0943f);
            ladderLinks[0].transform.Find("B").position = new Vector3(-6.2389f, 0.2614f, -9.6813f);
            ladderLinks[1].transform.position = new Vector3(-8.5289f, 1.0254f, -13.6413f);
            ladderLinks[1].transform.Find("A").position = new Vector3(-10.6689f, -0.2f, -14.2013f);
            ladderLinks[1].transform.Find("B").position = new Vector3(-8.0479f, 0.2614f, -13.8713f);
            ladderLinks[2].transform.position = new Vector3(7.4088f, 0.4519f, -8.1851f);
            ladderLinks[2].transform.Find("A").position = new Vector3(7.4364f, -1.2f, -6.7258f);
            ladderLinks[2].transform.Find("B").position = new Vector3(7.428f, 0.2614f, -9.1096f);
            foreach (GameObject ladderLink in ladderLinks)
            {
                Instantiate(ladderLink);
            }
        }

        public static T[] CreateArray<T>(int count) where T : new()
        {
            var array = new T[count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new T();
            }
            return array;
        }

    }
}
