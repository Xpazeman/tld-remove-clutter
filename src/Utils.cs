using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RemoveClutter
{
    internal class RCUtils
    {
        internal static List<GameObject> GetRootObjects()
        {
            List<GameObject> rootObj = new List<GameObject>();

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                GameObject[] sceneObj = scene.GetRootGameObjects();

                foreach (GameObject obj in sceneObj)
                {
                    rootObj.Add(obj);
                }
            }

            return rootObj;
        }

        internal static void GetChildrenWithName(GameObject obj, string name, List<GameObject> result)
        {
            if (obj.transform.childCount > 0)
            {

                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    GameObject child = obj.transform.GetChild(i).gameObject;

                    if (child.name.ToLower().Contains(name.ToLower()))
                        //&& child.name.ToLower().Contains("prefab"))
                    {
                        if (child.GetComponent<Container>() == null &&
                            child.GetComponentInChildren<Container>() == null &&
                            child.GetComponentInParent<Container>() == null &&
                            child.GetComponent<BreakDown>() == null &&
                            child.GetComponent<Bed>() == null &&
                            child.GetComponentInChildren<Bed>() == null &&
                            child.GetComponent<AuroraScreenDisplay>() == null &&
                            child.GetComponentInChildren<AuroraScreenDisplay>() == null)
                        {
                            result.Add(child);
                        }

                        continue;
                    }

                    GetChildrenWithName(child, name, result);
                }
            }
        }

        internal static void SetLayer(GameObject gameObject, int layer)
        {
            ChangeLayer changeLayer = gameObject.AddComponent<ChangeLayer>();
            changeLayer.Layer = layer;
            changeLayer.Recursively = false;
        }

        internal static void TimeUpdate()
        {
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.F3))
            {
                GetObjectNameUnderPoint();
            }
        }

        internal static void GetObjectNameUnderPoint()
        {
            vp_FPSCamera cam = GameManager.GetVpFPSPlayer().FPSCamera;
            RaycastHit raycastHit = DoRayCast(cam.transform.position, cam.transform.forward);

            GameObject go = raycastHit.collider.gameObject;

            HUDMessage.AddMessage(go.name);
            Debug.Log("[remove-clutter] Object under crosshair:>>>>> "+go.name);
        }

        internal static RaycastHit DoRayCast(Vector3 start, Vector3 direction)
        {
            Physics.Raycast(start, direction, out RaycastHit result, float.PositiveInfinity);

            return result;
        }

        internal static GameObject GetFurnitureRoot(GameObject gameObject)
        {
            if (gameObject.GetComponent<LODGroup>() != null)
            {
                return gameObject;
            }

            return GetFurnitureRoot(gameObject.transform.parent.gameObject);
        }
    }

    public class ChangeLayer : MonoBehaviour
    {
        public int Layer;
        public bool Recursively;

        public ChangeLayer(IntPtr intPtr) : base(intPtr) { }

        public void Start()
        {
            this.Invoke("SetLayer", 1);
        }

        internal void SetLayer()
        {
            vp_Layer.Set(this.gameObject, Layer, Recursively);
            Destroy(this);
        }
    }
}
