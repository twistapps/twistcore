// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnassignedField.Global

using System;
using UnityEditor.PackageManager;

namespace TwistCore
{
    [Serializable]
    public class PackageData
    {
        private Version _versionInfo;

        public string assetPath;

        public Author author;
        public string description;
        public string displayName;

        public string[] keywords;
        public string name;

        public Repository repository;
        //public string unity;
        public string version;
        public Version versionInfo => _versionInfo ??= new Version(version);

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

        public static explicit operator PackageData(PackageInfo info) => new PackageData
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
    }
}