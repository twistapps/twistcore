using System;

namespace TwistCore.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PackageNameAttribute : Attribute
    {
        public readonly string PackageName;

        public PackageNameAttribute(string packageName)
        {
            PackageName = packageName;
        }
    }
}