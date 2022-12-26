using TwistCore;

public class $NAME$Settings : SettingsAsset
{
    public override string GetEditorWindowTitle()
    {
        return "$DISPLAYNAME$";
    }

    public override string GetPackageName()
    {
        return "com.%ORGANIZATION%.%NAME_LOWERCASE%";
    }
}