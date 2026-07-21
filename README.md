# Tarkov Gamma & Vibrance Tool (formerly tarkov-settings)
![screenshot](./1.png)

**Language:** English | [한국어](README.ko.md) | [Русский](README.ru.md) | [中文](README.zh.md) | [日本語](README.ja.md)

This is a maintained fork of [incheon-kim/tarkov-settings](https://github.com/incheon-kim/tarkov-settings), which has had no updates since October 2023 and several unfixed crash reports. This fork fixes:
- Crashes when the NVIDIA display handle becomes invalid (RDP session change, closing NVIDIA Control Panel, monitor sleep/hotplug) — see upstream issues [#3](https://github.com/incheon-kim/tarkov-settings/issues/3) and [#17](https://github.com/incheon-kim/tarkov-settings/issues/17)
- Settings not saving when closing the window with the title bar's X button (previously only saved via tray icon → Exit)
- App crashing on a corrupted/invalid `settings.json` (now resets to defaults with a warning instead)
- Added `EscapeFromTarkovArena` to the default monitored process list — see upstream issue [#23](https://github.com/incheon-kim/tarkov-settings/issues/23)

## [->**DOWNLOAD Latest**<-](https://github.com/imda564/tarkov-gamma-vibrance-tool/releases/latest)

Automatically change color settings for [Escape from Tarkov](https://escapefromtarkov.com).

## How it works?
- Changes Digital Vibrance value from Nvidia Settings using [NvAPIWrapper](https://github.com/falahati/NvAPIWrapper)
- Changes Gamma using some [Win32 API calls](https://docs.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-setdevicegammaramp)

It only changes your display's colors when Escape from Tarkov's window is in focus.
This leaves a smooth transition when minimizing/maximizing.

## Supported Graphic Cards
- Nvidia GPU **fully supported.** (Brightness/Contrast/Gamma/Saturation)
- AMD GPU **partially supported.** (Except Saturation)
- **Intel/Etc is not supported yet.**

## What does it do?
You can change any of the following color settings:
1. Brightness
2. Contrast
3. Gamma
4. Digital Vibrance Control (aka. Saturation)
5. Only affects display while EFT window is focussed (It also prevents **sudden flash during Alt-tabbing**)

## How to Use
1. Open application (SmartGuard might prevent opening as it's not signed)
2. Set any color value
2.1. Double-click any slider labels to reset their values.
3. Minimize and play EFT
4. Close application if you want to deactivate

**Settings are saved to `settings.json` whenever you close the window or exit from the tray icon.**

## Profiles
Type a name in the box at the bottom-left and use **Save** / **Load** / **Del** to store and recall multiple sets of Brightness/Contrast/Gamma/Saturation (e.g. one profile per game or time of day). Profiles are stored in `settings.json` alongside your other settings.

## Hotkeys
Save a profile first, then click **Set** next to "Hotkey:" and press a key combination (e.g. `Ctrl+F9`) to bind it to that profile. The hotkey works globally — pressing it while EFT (or any other window) has focus instantly applies that profile's Brightness/Contrast/Gamma/Saturation. Use **Clear** to remove a profile's hotkey. If a combination is already used by another application, you'll get a warning and need to pick a different one.

## Warning
1. It might blink couple times when you active EFT window but it works. Don't worry.
2. **Disclaimer: I don't know BSG will ban for using this.**
3. AMD only supports Brightness/Contrast/Gamma Controls
4. Intel Graphic Cards are not supported
5. Only works in **Borderless mode.**
6. Nvidia Optimus Environment (mostly laptops) is not tested.

## TODO / Feature
- [x] Process Focusing Awareness
- [x] Digital Vibrance Value Change
- [x] Gamma Value Change
- [x] Brightness, Contrast, Gamma Value modify
- [x] GUI
- [x] ini or json configuration
- [x] Process Changeability (Not only for EscapeFromTarkov)
- [x] change display(monitor) target
- [x] Minimize to tray
- [x] Profiles
- [x] Hot Keys
- [ ] EFT setting modify (Framelimit or Graphic Settings)

Thanks for your support!
