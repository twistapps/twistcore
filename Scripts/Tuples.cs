using System;
using UnityEngine;

namespace TwistCore
{
    public static class Tuples
    {
        public static Tuple<T1, T2>[] Make<T1, T2>(T1[] arr1, T2[] arr2, bool suppressWarnings = false)
        {
            var count = Mathf.Min(arr1.Length, arr2.Length);
            if (!suppressWarnings && arr1.Length != arr2.Length)
                Debug.LogWarning("List length mismatch. Returned tuples are truncated.");

            var result = new Tuple<T1, T2>[count];
            for (var i = 0; i < count; i++) result[i] = Tuple.Create(arr1[i], arr2[i]);

            return result;
        }
    }
}