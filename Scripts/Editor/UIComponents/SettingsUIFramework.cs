using System;
using System.Collections.Generic;
using System.Reflection;

namespace TwistCore.Editor.UIComponents
{
    public static class SettingsUIFramework
    {
        private static readonly Dictionary<IPackageSettingsWindow<SettingsAsset>,
                Dictionary<string, ISettingsUIComponent<SettingsAsset>>>
            Cache =
                new Dictionary<IPackageSettingsWindow<SettingsAsset>,
                    Dictionary<string, ISettingsUIComponent<SettingsAsset>>>();

        private static Type GetTypeByName(string name)
        {
            return Type.GetType($"TwistCore.Editor.UIComponents.{name}, {Assembly.GetCallingAssembly().GetName()}");
            //return Type.GetType($"TwistCore.Editor.UIComponents.{name}, TwistCore");
        }

        private static void SetupCachedComponent<T>(IPackageSettingsWindow<T> boundWindow,
            ISettingsUIComponent<T> instance, string componentName)
            where T : SettingsAsset
        {
            instance.BindWindow(boundWindow);
            instance.SetComponentName(componentName);
            Cache[boundWindow].Add(componentName, instance);
        }

        public static void DrawCachedComponent<T>(this PackageSettingsWindow<T> bind, string component)
            where T : SettingsAsset
        {
            if (!Cache.ContainsKey(bind))
                Cache.Add(bind, new Dictionary<string, ISettingsUIComponent<SettingsAsset>>());

            if (!Cache[bind].ContainsKey(component))
            {
                var type = GetTypeByName(component) ?? GetTypeByName(component + "`1");
                var resultingType = type;

                if (type.IsGenericTypeDefinition)
                    resultingType = type.MakeGenericType(typeof(T));

                if (Activator.CreateInstance(resultingType) is ISettingsUIComponent<T> instance)
                    SetupCachedComponent(bind, instance, component);
                else
                    SetupCachedComponent(bind, Activator.CreateInstance<UnsupportedNotification>(), component);

                return;
            }

            Cache[bind][component].Draw();
        }
    }
}