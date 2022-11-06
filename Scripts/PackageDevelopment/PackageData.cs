

// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global

namespace TwistCore
{
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
        public string unity;
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
    }
}