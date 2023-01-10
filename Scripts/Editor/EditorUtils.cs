using System;
using System.Collections.Generic;
using System.Linq;

namespace TwistCore.Editor
{
    public static class EditorUtils
    {
        //cache results of GetDerivedFrom() because it's a pretty expensive method
        private static readonly Dictionary<Type, Type[]> DerivativesCache = new Dictionary<Type, Type[]>();
        private static TwistCoreSettings Settings => SettingsUtility.Load<TwistCoreSettings>();

        public static Type[] GetDerivedFrom<T>(params Type[] ignored)
        {
#if TWISTCORE_DEBUG
            var stopwatch = new Stopwatch();
            stopwatch.Start();
#endif

            Type[] foundArr;
            if (DerivativesCache.ContainsKey(typeof(T)))
            {
                // return cached result if GetDerivedFrom() has already been invoked before
                foundArr = DerivativesCache[typeof(T)];
            }
            else
            {
                var found = from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                    from assemblyType in domainAssembly.GetTypes()
                    where typeof(T).IsAssignableFrom(assemblyType)
                    select assemblyType;

                foundArr = found as Type[] ?? found.ToArray();
                DerivativesCache.Add(typeof(T), foundArr);
            }

            if (ignored != null)
                foundArr = foundArr.Where(t => !ignored.Contains(t)).ToArray();

#if TWISTCORE_DEBUG
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > 0 && Settings.debug)
            {
                Debug.Log($"GetDerivedFrom<{typeof(T).Name}>() took {stopwatch.ElapsedMilliseconds}ms to execute");
                stopwatch.Reset();
            }
#endif

            return foundArr;
        }

        public static Type[] GetDerivedTypesExcludingSelf<T>(params Type[] ignored)
        {
            return GetDerivedFrom<T>(ignored.Append(typeof(T)).ToArray());
        }
    }
}