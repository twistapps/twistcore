public class VersionComparison
{
    private const string ZERO = "0.0.0";
    private readonly Version _current, _next;

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

    public bool hasUpdate => HasPatchUpdate();
    
    public VersionComparison(string currentVersion, string nextVersion)
    {
        _current = new Version(currentVersion);
        _next = new Version(nextVersion);
    }

    public VersionComparison()
    {
        _current = new Version(ZERO);
        _next = new Version(ZERO);
    }
    
    public override string ToString()
    {
        return _current.ToString();
    }
}