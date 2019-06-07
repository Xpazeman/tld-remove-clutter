﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ModComponentMapper;

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

        internal static void SetLayer(GameObject gameObject, int layer = vp_Layer.InteractivePropNoCollideGear)
        {
            ChangeLayer changeLayer = gameObject.AddComponent<ChangeLayer>();
            changeLayer.Layer = layer;
            changeLayer.Recursively = false;
        }

        internal static void TimeUpdate()
        {
            if (Input.GetKeyUp(KeyCode.F3))
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
    }
}
