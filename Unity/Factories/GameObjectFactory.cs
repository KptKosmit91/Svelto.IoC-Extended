using System;
using System.Collections.Generic;
using UnityEngine;

namespace Svelto.Context
{
    public class GameObjectFactory : Factories.IGameObjectFactory
    {
        public GameObjectFactory()
        {
            _prefabs = new Dictionary<string, GameObject[]>();
        }

        public GameObject Build(string prefabName, Action<GameObject> preInit = null)
        {
            DesignByContract.Check.Require(_prefabs.ContainsKey(prefabName), "Svelto.Factories.IGameObjectFactory -prefab was not found: " + prefabName);

            var go = Build(_prefabs[prefabName][0], preInit);
            GameObject parent = _prefabs[prefabName][1];

            if (parent != null)
            {
                Transform transform = go.transform;

                var scale = transform.localScale;
                var rotation = transform.localRotation;
                var position = transform.localPosition;

                parent.SetActive(true);

                transform.parent = parent.transform;

                transform.localPosition = position;
                transform.localRotation = rotation;
                transform.localScale = scale;
            }

            return go;
        }
        
        virtual public GameObject Build(GameObject prefab, Action<GameObject> preInit = null)
        {
            DesignByContract.Check.Require(prefab != null, "Svelto.Factories.IGameObjectFactory -null prefab");

            var copy = GameObject.Instantiate(prefab) as GameObject;
            preInit?.Invoke(copy);

            return copy;
        }

        /// <summary>
        /// Register a prefab to be built later using a string ID.
        /// </summary>
        /// <param name="prefab">original prefab</param>
        public void RegisterPrefab(GameObject prefab, string prefabName, GameObject parent = null)
        {
            var objects = new GameObject[2];

            objects[0] = prefab; objects[1] = parent;

            _prefabs.Add(prefabName, objects);
        }

        readonly Dictionary<string, GameObject[]>                        _prefabs;
    }
}
