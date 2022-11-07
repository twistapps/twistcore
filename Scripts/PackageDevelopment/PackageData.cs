// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnassignedField.Global

using System;
using TwistCore.PackageRegistry.Versioning;
using UnityEditor.PackageManager;

namespace TwistCore.PackageDevelopment
{
    [Serializable]
    public class PackageData
    {
        public string assetPath;
        public string description;
        public string displayName;

        public string[] keywords;

        public string name;

        //public string unity;
        public string version;
        private VersionInfo _versionInfoInfo;

        public Author author;

        public Repository repository;
        public VersionInfo VersionInfoInfo => _versionInfoInfo ??= new VersionInfo(version);

        public static explicit operator PackageData(PackageInfo info)
        {
            if (info == null) return null;
            var data = new PackageData
            {
                author = new Author
                {
                    email = info.author.email,
                    name = info.author.name,
                    url = info.author.url
                },
                description = info.description,
                displayName = info.displayName,
                keywords = info.keywords,
                name = info.name,
                repository = new Repository
                {
                    type = info.repository.type,
                    url = info.repository.url
                },
                version = info.version,
                assetPath = info.assetPath
            };

            return data;
        }

        public class Repository
        {
            public string type;
            public string url;
        }

        public class Author
        {
            public string email;
            public string name;
            public string url;
        }
    }
}