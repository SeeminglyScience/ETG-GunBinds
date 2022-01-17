using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace GunBinds;

internal static class GameUIRootExtensions
{
    private static readonly Lazy<Func<GameUIRoot, List<dfSprite>>> s_getAdditionalGunBoxes = new(
        () =>
        {
            FieldInfo field = typeof(GameUIRoot)
                .GetField(
                    "additionalGunBoxes",
                    BindingFlags.NonPublic | BindingFlags.Instance);

            ParameterExpression self = Expression.Parameter(typeof(GameUIRoot), nameof(self));
            return Expression.Lambda<Func<GameUIRoot, List<dfSprite>>>(
                Expression.Field(self, field),
                self)
                .Compile();
        });

    private static readonly Lazy<Func<dfLanguageManager, Dictionary<string, string>>> s_getStrings = new(
        () =>
        {
            FieldInfo field  = typeof(dfLanguageManager)
                .GetField(
                    "strings",
                    BindingFlags.NonPublic | BindingFlags.Instance);

            ParameterExpression self = Expression.Parameter(typeof(dfLanguageManager), nameof(self));
            return Expression.Lambda<Func<dfLanguageManager, Dictionary<string, string>>>(
                Expression.Field(self, field),
                self)
                .Compile();
        });

    public static List<dfSprite> GetAdditionalGunBoxes(this GameUIRoot self)
    {
        return s_getAdditionalGunBoxes.Value(self);
    }

    public static Dictionary<string, string> GetStrings(this dfLanguageManager self)
    {
        return s_getStrings.Value(self);
    }
}
