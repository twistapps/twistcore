using System.Linq;

namespace TwistCore.PackageRegistry.Versioning
{
    public class VersionInfo
    {
        private readonly uint[] _parts;
        private readonly string _raw;

        public VersionInfo(string version)
        {
            _raw = version;
            _parts = version.Split('.').Select(uint.Parse).ToArray();
        }

        public uint Major => _parts[0];
        public uint Minor => _parts[1];
        public uint Patch => _parts[2];

        public override string ToString()
        {
            return _raw;
        }
    }
}