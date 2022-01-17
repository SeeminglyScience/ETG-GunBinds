# GunBinds

A mod for Enter the Gungeon that allows you to assign a gun to a favorite and then use that key to swap to it. [See on ModWorkshop](https://modworkshop.net/mod/35899).

## Why

Ever wish you could quickly swap to a beam weapon to destroy beholster missles without messing up your quick equip combo? Or swap to some garbage gun to check for secrets? Or get more use out of support weapons like plunger or molotov launcher? Or just make more efficient use of weapons with long reloads?

You can use this to do all of that without spending half of your play time in the gun select menu.

## Favorites

- Press while in gun select menu to `Favorite`
- Press outside of gun select menu to switch to `Favorite`

## Primary Toggle

- Press while in gun select menu to set `Primary`
- Press again in gun select menu to set `Secondary`
- Press outside of gun select menu to swap between `Primary` and `Secondary`
- Performing `Quick Equip` will also toggle between `Primary` and `Secondary` if they are set (configurable)

## Key binds

All key binds are in the normal controls menu.

## Settings

Configuration file is located at `<Enter the Gungeon>\Resources\GunBinds\gunBinds.json`.

- `reenableQuickGunKeys`: The <kbd>1</kbd>-<kbd>9</kbd> keys can be used without this mod to switch to specific slots. They cannot be rebound. This mod disables that functionality unless this key is set to `true`. Only set this to true if you plan on rebinding `Favorite` keys to something other than <kbd>1</kbd>-<kbd>9</kbd>

- `disableQuickEquipHook`: This mod hooks into the standard `Quick Equip` to allow it to swap between `Primary` and `Secondary` favorites. Set this to `true` to disable this hook and revert `Quick Equip` behavior.

## Special Thanks

- [TranslationAPI](https://modworkshop.net/mod/35150)
- [BindingAPI](https://modworkshop.net/mod/34260)
- [Simple Stats](https://modworkshop.net/mod/23701) (that I ripped the UI code from)
