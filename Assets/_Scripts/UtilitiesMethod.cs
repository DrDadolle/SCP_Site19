﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilitiesMethod {

    //Utilities method to change the material of all the children
    public static void ChangeMaterialOfRecChildGameObject(GameObject obj, Material material)
    {
        Renderer[] children;
        children = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in children)
        {
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++)
            {
                mats[j] = material;
            }
            rend.materials = mats;
        }
    }

    //Return true if at least one material of the game object is from the specified Material
    public static bool IsChildGameObjectOfSpecificMaterial(GameObject obj, Material material)
    {
        Renderer[] children;
        children = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in children)
        {
            if (rend.sharedMaterial.Equals(material))
            {
                return true;
            }
        }
        return false;
    }

    /*
	 * Snap the position at the center of the grid
	 * The y axis will be always of 0
	*/
    public static Vector3 snapCenterPosition(Vector3 original)
    {
        Vector3 snapped;
        snapped.x = Mathf.Floor(original.x) + 0.5f;
        snapped.y = Mathf.Floor(0f);
        snapped.z = Mathf.Floor(original.z) + 0.5f;
        return snapped;
    }

    /*
     * Snap the position at the edge of the grid
     * The y axis will be always of 0
    */
    public static Vector3 snapPosition(Vector3 original)
    {
        Vector3 snapped;
        snapped.x = Mathf.Floor(original.x + 0.5f);
        snapped.y = Mathf.Floor(0f);
        snapped.z = Mathf.Floor(original.z + 0.5f);
        return snapped;
    }

    /**
     *  Generic method to return list of keys from dic
    */
    public static List<T> GetListOfModelFromDictionnary<T>(Dictionary<T, GameObject> dic)
    {
        List<T> ret = new List<T>();
        foreach (var key in dic.Keys)
        {
            ret.Add(key);
        }
        return ret;
    }

    /**
  *  Replace all keys of a dict by the list
  */
    public static Dictionary<T, GameObject> ReplaceKeysOfDictByList<T>(Dictionary<T, GameObject> dic, List<T> list)
    {
        // Avoid out of sync issue for dictionnary
        Dictionary<T, GameObject> result = new Dictionary<T, GameObject>();
        T foundElement;

        bool isFound = false;
        foreach (var key in dic.Keys)
        {
            //Reset
            isFound = false;
            foundElement = default(T);

            foreach (var l in list)
            {
                if (l.Equals(key))
                {
                    // Mark as found
                    isFound = true;
                    foundElement = l;
                    //Add the new keys and the old value
                    result.Add(l, dic[key]);

                    // exit the loop
                    break;
                }
            }

            // if found, remove the l element from the list
            if (isFound)
            {
                list.Remove(foundElement);
            }
            //if not found, it means that the save action had an issue
            else
            {
                Debug.LogError("We didn't found the key " + key + " in the list " + list);
            }
        }

        // At the end we replace the dictionnary by the result one
        return result;
    }

}
