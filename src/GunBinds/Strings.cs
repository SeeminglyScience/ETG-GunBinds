namespace GunBinds;

internal static class Strings
{
    public static string[] AllKeys { get; } =
    {
        "#GUNBINDS_STARTER",
        "#GUNBINDS_PRIMARY_TOGGLE",
        "#GUNBINDS_PRIMARY",
        "#GUNBINDS_SECONDARY",
        "#GUNBINDS_FAVORITE",
        "#GUNBINDS_FAVORITE1",
        "#GUNBINDS_FAVORITE2",
        "#GUNBINDS_FAVORITE3",
        "#GUNBINDS_FAVORITE4",
        "#GUNBINDS_FAVORITE5",
        "#GUNBINDS_FAVORITE6",
        "#GUNBINDS_FAVORITE7",
        "#GUNBINDS_FAVORITE8",
        "#GUNBINDS_FAVORITE9",
        "#GUNBINDS_SET",
    };

    public static string Starter => GetString("#GUNBINDS_STARTER");

    public static string Primary => GetString("#GUNBINDS_PRIMARY");

    public static string Secondary => GetString("#GUNBINDS_SECONDARY");

    public static string Favorite => GetString("#GUNBINDS_FAVORITE");

    public static string Set => GetString("#GUNBINDS_SET");

    private static string GetString(string value) => StringTableManager.GetString(value);
}
