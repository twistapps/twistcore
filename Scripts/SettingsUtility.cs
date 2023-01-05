using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TwistCore
{
    public static class SettingsUtility
    {
        private static readonly Dictionary<Type, SettingsAsset> Settings = new Dictionary<Type, SettingsAsset>();

        public static T Load<T>() where T : SettingsAsset
        {
            var type = typeof(T);
            T asset;
            if (Settings.ContainsKey(type))
            {
                asset = (T)Settings[type];
                if (asset != null)
                    return asset;

                Settings.Remove(type);
            }

            asset = Resources.Load<T>(Path.Combine("Settings", type.Name));
            if (asset != null)
            {
                Settings.Add(type, asset);
                return asset;
            }

#if UNITY_EDITOR

            //if settings file not found at desired location
            asset = ScriptableObject.CreateInstance<T>();
            var assetPath = Path.Combine(TwistCore.SettingsFolder, type.Name) + ".asset";
            Directory.CreateDirectory(TwistCore.SettingsFolder);
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();

            Settings.Add(type, asset);
            return asset;

#else
            Debug.LogError($"Settings file {typeof(T).Name} not found in Resources!");
            return null;
#endif
        }
    }
}