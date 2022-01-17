using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using BindingAPI;
using HarmonyLib;
using InControl;
using MonoMod.RuntimeDetour;
using UnityEngine;

using OpCodes = System.Reflection.Emit.OpCodes;

namespace GunBinds;

public class GunBindsBehaviours : MonoBehaviour
{
    private static GunBindsBehaviours s_instance;

    private ConfigManager<GunBindsConfig> _config;

    private PlayerAction _switchToStarter;

    private PlayerAction _primaryToggle;

    private readonly Gun[] _favorites = new Gun[9];

    private Gun _primary;

    private Gun _secondary;

    private bool _primaryWasSet;

    private PlayerAction _switchToFavorite1;

    private PlayerAction _switchToFavorite2;

    private PlayerAction _switchToFavorite3;

    private PlayerAction _switchToFavorite4;

    private PlayerAction _switchToFavorite5;

    private PlayerAction _switchToFavorite6;

    private PlayerAction _switchToFavorite7;

    private PlayerAction _switchToFavorite8;

    private PlayerAction _switchToFavorite9;

    private GunventoryState _lastGunventoryState;

    public void Start()
    {
        s_instance = this;
        _config = ConfigManager.Create<GunBindsConfig>(
            "GunBinds",
            "gunBinds",
            () => default);

        if (!_config.Value.reenableQuickGunKeys)
        {
            GameManager.Options.DisableQuickGunKeys = true;
        }

        CreateHooks();
        CreateBinds();
    }

    public void Update()
    {
        GameUIRoot ui = GameUIRoot.Instance;
        if (ui is null)
        {
            return;
        }

        GungeonActions primaryPlayerActions = BraveInput.PrimaryPlayerInstance?.ActiveActions;
        if (primaryPlayerActions is null)
        {
            return;
        }

        bool isInGunSelect = ui.MetalGearActive && primaryPlayerActions.GunQuickEquipAction.IsPressed;

        if (GunBinds.Text?.text is not null and not "")
        {
            GunventoryState last = _lastGunventoryState;
            GunventoryState current = GunventoryState.Get(GameUIRoot.Instance?.GetAdditionalGunBoxes());
            if (!current.IsExpanded || last.HasSelectionChanged(current) || last.IsNewUnfolding(current))
            {
                GunBinds.Text.text = "";
            }
        }

        if (_primaryToggle.WasPressed)
        {
            void UpdatePrimaryToggle()
            {
                var current = GunventoryState.Get(GameUIRoot.Instance?.GetAdditionalGunBoxes());
                if (!isInGunSelect)
                {
                    if (_primary is null)
                    {
                        DebugLog("PrimaryToggle: primary == null, do quick equip");
                        GameManager.Instance.PrimaryPlayer.DoQuickEquip();
                        return;
                    }

                    if (_secondary is null)
                    {
                        DebugLog("PrimaryToggle: secondary == null, do quick equip");
                        GameManager.Instance.PrimaryPlayer.DoQuickEquip();
                        return;
                    }

                    if (_primary == _secondary)
                    {
                        DebugLog("PrimaryToggle: primary == secondary, do quick equip");
                        GameManager.Instance.PrimaryPlayer.DoQuickEquip();
                        return;
                    }

                    if (GameManager.Instance.PrimaryPlayer.CurrentGun == _primary)
                    {
                        DebugLog("PrimaryToggle: switching to secondary");
                        SwitchToGun(_secondary, 1);
                        return;
                    }

                    DebugLog("PrimaryToggle: switching to primary");
                    SwitchToGun(_primary, 0);
                    return;
                }

                if (_lastGunventoryState.IsNewUnfolding(current))
                {
                    _lastGunventoryState = current;
                    UpdateGun(ref _primary, Strings.Primary);
                    _primaryWasSet = true;
                    return;
                }

                _lastGunventoryState = current;
                if (_primaryWasSet)
                {
                    UpdateGun(ref _secondary, Strings.Secondary);
                    _primaryWasSet = false;
                    return;
                }

                UpdateGun(ref _primary, Strings.Primary);
                _primaryWasSet = true;
            }

            UpdatePrimaryToggle();
        }

        int favorite = true switch
        {
            _ when _switchToStarter.WasPressed => 0,
            _ when _switchToFavorite1.WasPressed => 1,
            _ when _switchToFavorite2.WasPressed => 2,
            _ when _switchToFavorite3.WasPressed => 3,
            _ when _switchToFavorite4.WasPressed => 4,
            _ when _switchToFavorite5.WasPressed => 5,
            _ when _switchToFavorite6.WasPressed => 6,
            _ when _switchToFavorite7.WasPressed => 7,
            _ when _switchToFavorite8.WasPressed => 8,
            _ when _switchToFavorite9.WasPressed => 9,
            _ => -1,
        };

        if (favorite is -1)
        {
            return;
        }

        if (favorite is 0)
        {
            if (isInGunSelect)
            {
                return;
            }

            GameManager.Instance.PrimaryPlayer.ChangeToGunSlot(0);
            return;
        }

        GunventoryState.Get(ref _lastGunventoryState, GameUIRoot.Instance?.GetAdditionalGunBoxes());
        if (isInGunSelect)
        {
            SetFavorite(favorite);
            return;
        }

        SwitchToFavorite(favorite);
    }

    private void CreateHooks()
    {
        if (!_config.Value.disableQuickEquipHook)
        {
            new Hook(
                typeof(PlayerController).GetMethod("DoQuickEquip"),
                typeof(GunBindsBehaviours).GetMethod(nameof(DoQuickEquipHook)));
        }

        const BindingFlags flags = BindingFlags.NonPublic
            | BindingFlags.Public
            | BindingFlags.Instance
            | BindingFlags.Static;

        MethodInfo target = typeof(GameUIRoot)
            .GetNestedType("<HandleMetalGearGunSelect>c__Iterator6", flags)
            .GetMethod(nameof(IEnumerator.MoveNext), flags);

        Harmony patch = new("seeminglyscience.GunBinds");
        patch.Patch(
            target,
            transpiler: new HarmonyMethod(
                typeof(GunBindsBehaviours)
                    .GetMethod(
                        nameof(HandleMetalGearGunSelectTranspiler),
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)));
    }

    private void CreateBinds()
    {
        _switchToStarter = BindingBuilder.CreateBinding(
            "#GUNBINDS_STARTER",
            defaultKey: Key.X);

        _primaryToggle = BindingBuilder.CreateBinding(
            "#GUNBINDS_PRIMARY_TOGGLE",
            defaultKey: Key.C);

        _switchToFavorite1 = BindingBuilder.CreateBinding(
            "#GUNBINDS_FAVORITE1",
            defaultKey: Key.Key1);

        _switchToFavorite2 = BindingBuilder.CreateBinding(
            "#GUNBINDS_FAVORITE2",
            defaultKey: Key.Key2);

        _switchToFavorite3 = BindingBuilder.CreateBinding(
            "#GUNBINDS_FAVORITE3",
            defaultKey: Key.Key3);

        _switchToFavorite4 = BindingBuilder.CreateBinding(
            "#GUNBINDS_FAVORITE4",
            defaultKey: Key.Key4);

        _switchToFavorite5 = BindingBuilder.CreateBinding(
            "#GUNBINDS_FAVORITE5",
            defaultKey: Key.Key5);

        _switchToFavorite6 = BindingBuilder.CreateBinding(
            "#GUNBINDS_FAVORITE6",
            defaultKey: Key.Key6);

        _switchToFavorite7 = BindingBuilder.CreateBinding(
            "#GUNBINDS_FAVORITE7",
            defaultKey: Key.Key7);

        _switchToFavorite8 = BindingBuilder.CreateBinding(
            "#GUNBINDS_FAVORITE8",
            defaultKey: Key.Key8);

        _switchToFavorite9 = BindingBuilder.CreateBinding(
            "#GUNBINDS_FAVORITE9",
            defaultKey: Key.Key9);
    }

    private void UpdateGun(ref Gun gun, string name)
    {
        gun = DetermineCurrentMetalGearSelection();
        GunBinds.Text.text = string.Concat(name, " ", Strings.Set);
    }

    private void SetFavorite(int number)
    {
        UpdateGun(ref _favorites[number - 1], $"{Strings.Favorite} {number}");
    }

    private void SwitchToGun(Gun gun, int backupSlot)
    {
        PlayerController player = GameManager.Instance.PrimaryPlayer;
        List<Gun> allGuns = player.inventory.AllGuns;
        if (gun is null)
        {
            DebugLog("SwitchToGun: null favorite, backup slot: {0}", backupSlot);
            if (allGuns.Count > backupSlot)
            {
                ChangeToGunSlotImpl(player, backupSlot);
            }

            return;
        }

        int slot = allGuns.IndexOf(gun);
        if (slot is -1)
        {
            DebugLog("SwitchToGun: ERROR! gun can't be found, backup slot: {0}", backupSlot);
            ChangeToGunSlotImpl(player, backupSlot);
            return;
        }

        DebugLog("SwitchToGun: Gun found, slot {0}", slot);
        ChangeToGunSlotImpl(player, slot);
    }

    private static void ChangeToGunSlotImpl(PlayerController player, int slot)
    {
        player.ChangeToGunSlot(slot);
    }

    private void SwitchToFavorite(int number)
    {
        Gun favorite = _favorites[number - 1];
        SwitchToGun(favorite, number);
    }

    private static Gun DetermineCurrentMetalGearSelection()
    {
        return GameManager.Instance.PrimaryPlayer.inventory.GetTargetGunWithChange(s_totalGunShift);
    }

    public static void DoQuickEquipHook(Action<PlayerController> original, PlayerController self)
    {
        if (s_instance._primary is null && s_instance._secondary is null)
        {
            original(self);
            return;
        }

        if (s_instance._primary == s_instance._secondary)
        {
            original(self);
            return;
        }

        if (self.CurrentGun == s_instance._primary)
        {
            s_instance.SwitchToGun(s_instance._secondary, 1);
            return;
        }

        s_instance.SwitchToGun(s_instance._primary, 0);
    }

    // Can't be read only, it's set by a transpiled method.
#pragma warning disable RCS1169, IDE0044
    private static int s_totalGunShift;
#pragma warning restore RCS1169, IDE0044

    private static IEnumerable<CodeInstruction> HandleMetalGearGunSelectTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        const string coroutineTypeName = "<HandleMetalGearGunSelect>c__Iterator6";
        const string totalGunShiftFieldName = "<totalGunShift>__0";

        FieldInfo sourceField = typeof(GameUIRoot).GetNestedType(
            coroutineTypeName,
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .GetField(
                totalGunShiftFieldName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        FieldInfo resultField = typeof(GunBindsBehaviours)
            .GetField(
                nameof(s_totalGunShift),
                BindingFlags.NonPublic | BindingFlags.Static);

        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.StoresField(sourceField))
            {
                yield return new CodeInstruction(OpCodes.Stsfld, resultField);
                yield return new CodeInstruction(OpCodes.Pop);
                continue;
            }

            if (instruction.LoadsField(sourceField))
            {
                yield return new CodeInstruction(OpCodes.Pop);
                yield return new CodeInstruction(OpCodes.Ldsfld, resultField);
                continue;
            }

            if (instruction.LoadsField(sourceField, byAddress: true))
            {
                yield return new CodeInstruction(OpCodes.Pop);
                yield return new CodeInstruction(OpCodes.Ldsflda, resultField);
                continue;
            }

            yield return instruction;
        }
    }

    private void DebugLog<T>(string format, T arg)
    {
        if (!_config.Value.enableDebugLogging) return;
        DebugImpl(string.Format(CultureInfo.InvariantCulture, format, arg));
    }

    private void DebugLog(string message)
    {
        if (!_config.Value.enableDebugLogging) return;
        DebugImpl(message);
    }

    private void DebugImpl(string message) => Log.Info(message);
}
