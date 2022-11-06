namespace TwistCore
{
    public static class Github
    {
        public static string GetPackageJsonURL(string packageName)
        {
            var parts = packageName.Split('.');
            var author = parts[1];
            var name = parts[2];

            var url = $"https://raw.githubusercontent.com/{author}/{name}/main/package.json";
            return url;
        }

        public static string GetPackageRootURL(string author, string name)
        {
            var url = $"https://raw.githubusercontent.com/{author}/{name}/main/";
            return url;
        }
        
        public static string GetPackageRootURL(string packageFullName)
        {
            var parts = packageFullName.Split('.');
            var author = parts[1];
            var name = parts[2];
            var url = $"https://raw.githubusercontent.com/{author}/{name}/main/";
            return url;
        }
    }
}