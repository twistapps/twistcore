using System;
using System.Collections.Generic;
using System.Reflection;

namespace TwistCore.Editor.GuiWidgets
{
    public static class GuiWidgetFramework
    {
        private static readonly Dictionary<IPackageSettingsWindow<SettingsAsset>,
                Dictionary<string, IGuiWidget<SettingsAsset>>>
            Cache =
                new Dictionary<IPackageSettingsWindow<SettingsAsset>,
                    Dictionary<string, IGuiWidget<SettingsAsset>>>();

        private static Type GetTypeByName(string name)
        {
            return Type.GetType($"TwistCore.Editor.GuiWidgets.{name}, {Assembly.GetCallingAssembly().GetName()}");
        }

        private static void SetupCachedComponent<T>(IPackageSettingsWindow<T> boundWindow,
            IGuiWidget<T> instance, string componentName)
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
                Cache.Add(bind, new Dictionary<string, IGuiWidget<SettingsAsset>>());

            if (!Cache[bind].ContainsKey(component))
            {
                var type = GetTypeByName(component) ?? GetTypeByName(component + "`1");
                var resultingType = type;

                if (type.IsGenericTypeDefinition)
                    resultingType = type.MakeGenericType(typeof(T));

                if (bind.CurrentSection == null)
                    if (type.GetCustomAttribute<WidgetIncludesSectionAttribute>()
                            ?.SectionIsIncluded != true)
                    {
                        SetupCachedComponent(bind, Activator.CreateInstance<NotInsideSectionNotification>(), component);
                        return;
                    }

                if (Activator.CreateInstance(resultingType) is IGuiWidget<T> instance)
                    SetupCachedComponent(bind, instance, component);
                else
                    SetupCachedComponent(bind, Activator.CreateInstance<UnsupportedNotification>(), component);

                return;
            }

            /*bug: NullReferenceException, reproduction:
             widget is present in cache,
             WidgetIncludesSectionAttribute.SectionIsIncluded != true 
             and current section is null.
             
             to fix, we should check for current section every time we try to draw component
             but it's expensive.
             */
            Cache[bind][component].Draw();
        }
    }
}