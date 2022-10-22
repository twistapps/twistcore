using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global

namespace TwistCore
{
    public class PackageData
    {
        public string name;
        public string displayName;
        public string description;
        public string version;
        public string unity;

        public class Repository
        {
            public string type;
            public string url;
        }

        public Repository repository;

        public class Author
        {
            public string name;
            public string email;
            public string url;
        }

        public Author author;

        public string[] keywords;

        private Version _versionInfo;
        public Version versionInfo => _versionInfo ??= new Version(version);
        
        
        public string assetPath;
    }
}
