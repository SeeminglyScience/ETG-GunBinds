using System.Collections.Generic;

namespace GunBinds;

public readonly struct GunventoryState
{
    public static readonly GunventoryState Folded;

    public readonly int SelectedSpriteId;

    public readonly bool IsExpanded;

    public readonly object FirstGunBox;

    private GunventoryState(int selectedSpriteId, bool isExpanded, object firstGunBox)
    {
        SelectedSpriteId = selectedSpriteId;
        IsExpanded = isExpanded;
        FirstGunBox = firstGunBox;
    }

    public static GunventoryState Get(List<dfSprite> gunBoxes)
    {
        GunventoryState state = default;
        Get(ref state, gunBoxes);
        return state;
    }

    public static void Get(ref GunventoryState state, List<dfSprite> gunBoxes)
    {
        if (gunBoxes is null or { Count: 0 })
        {
            state = Folded;
            return;
        }

        object weaponBox0 = null;
        foreach (dfSprite gunBox in gunBoxes)
        {
            if (gunBox.name == "AdditionalWeaponBox0")
            {
                weaponBox0 = gunBox;
            }
        }

        state = new GunventoryState(
            gunBoxes[0].GetComponentsInChildren<tk2dBaseSprite>()[0].spriteId, true, weaponBox0);
    }

    public readonly bool IsNewUnfolding(in GunventoryState current)
    {
        return !object.ReferenceEquals(FirstGunBox, current.FirstGunBox);
    }

    public readonly bool HasSelectionChanged(in GunventoryState current)
    {
        return SelectedSpriteId != current.SelectedSpriteId;
    }
}
