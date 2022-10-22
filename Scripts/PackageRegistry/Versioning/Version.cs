using System.Linq;

public class Version
{
    private readonly uint[] versionRaw;
            
    public uint Major => versionRaw[0];
    public uint Minor => versionRaw[1];
    public uint Patch => versionRaw[2];

    public string version => string.Join(".", versionRaw.Select(v => v.ToString()));

    public Version(string version)
    {
        versionRaw = version.Split('.').Select(uint.Parse).ToArray();
    }

    public override string ToString()
    {
        return version;
    }
}