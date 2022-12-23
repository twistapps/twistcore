using System;

namespace TwistCore.PackageRegistry.Versioning
{
    [Serializable]
    public class VersionComparer
    {
        private const string Zero = "0.0.0";
        private readonly VersionInfo _current, _next;

        public string NextVersion => _next.ToString();

        public VersionComparer(string currentVersion, string nextVersion)
        {
            _current = new VersionInfo(currentVersion);
            _next = new VersionInfo(nextVersion);
        }

        public VersionComparer()
        {
            _current = new VersionInfo(Zero);
            _next = new VersionInfo(Zero);
        }

        public bool HasUpdate => HasPatchUpdate();

        public bool HasMajorUpdate()
        {
            return _current.Major < _next.Major;
        }

        public bool HasMinorUpdate()
        {
            return HasMajorUpdate() || _current.Minor < _next.Minor;
        }

        public bool HasPatchUpdate()
        {
            return HasMajorUpdate() || HasMinorUpdate() || _current.Patch < _next.Patch;
        }

        public override string ToString()
        {
            return _current.ToString();
        }
    }
}