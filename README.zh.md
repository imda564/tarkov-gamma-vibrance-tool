# Tarkov Settings (维护中的分支)
![screenshot](./1.png)

**Language:** [English](README.md) | [한국어](README.ko.md) | [Русский](README.ru.md) | 中文 | [日本語](README.ja.md)

本仓库是 [incheon-kim/tarkov-settings](https://github.com/incheon-kim/tarkov-settings) 的维护分支。原项目自 2023 年 10 月起未再更新,且存在多个尚未修复的崩溃问题。此分支修复了以下内容:
- 当 NVIDIA 显示句柄失效时(远程桌面会话切换、关闭 NVIDIA 控制面板、显示器休眠/热插拔)导致的崩溃 —— 参见原仓库 issue [#3](https://github.com/incheon-kim/tarkov-settings/issues/3) 和 [#17](https://github.com/incheon-kim/tarkov-settings/issues/17)
- 使用标题栏的 X 按钮关闭窗口时设置未保存的问题(此前只有通过托盘图标 → Exit 才会保存)
- `settings.json` 损坏或格式错误时应用崩溃的问题(现在会弹出警告并重置为默认值)
- 在默认监控进程列表中加入 `EscapeFromTarkovArena` —— 参见原仓库 issue [#23](https://github.com/incheon-kim/tarkov-settings/issues/23)

## [->**下载最新版本**<-](https://github.com/imda564/tarkov-settings-fixed/releases/latest)

自动为 [Escape from Tarkov](https://escapefromtarkov.com) 调整显示颜色设置。

## 工作原理
- 通过 [NvAPIWrapper](https://github.com/falahati/NvAPIWrapper) 调整 NVIDIA 设置中的数字饱和度(Digital Vibrance)数值
- 通过部分 [Win32 API 调用](https://docs.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-setdevicegammaramp) 调整伽马值

只有当 Escape from Tarkov 窗口处于焦点状态时才会更改显示颜色。
最小化/还原窗口时会平滑过渡。

## 支持的显卡
- NVIDIA 显卡 **完全支持**(亮度/对比度/伽马/饱和度)
- AMD 显卡 **部分支持**(饱和度除外)
- **Intel/其他显卡暂不支持。**

## 功能说明
可以调整以下颜色设置:
1. 亮度(Brightness)
2. 对比度(Contrast)
3. 伽马(Gamma)
4. 数字饱和度控制(Digital Vibrance,即饱和度)
5. 仅在 EFT 窗口处于焦点时生效(同时可防止 **切换窗口时屏幕突然闪烁**)

## 使用方法
1. 打开应用程序(由于未进行签名,SmartScreen 可能会发出警告)
2. 设置任意颜色数值
2.1. 双击滑块标签可重置该数值。
3. 最小化程序并游玩 EFT
4. 如需停用效果,关闭程序即可

**关闭窗口或从托盘图标退出时,设置会保存到 `settings.json`。**

## 配置文件(Profiles)
在左下角的输入框中输入名称,使用 **Save** / **Load** / **Del** 按钮保存和读取多组亮度/对比度/伽马/饱和度数值(例如为不同游戏或不同时间段分别设置)。配置文件会和其他设置一起保存在 `settings.json` 中。

## 注意事项
1. 激活 EFT 窗口时屏幕可能会闪烁几次,这是正常现象,无需担心。
2. **免责声明:无法保证使用本程序不会被 BSG 封禁。**
3. AMD 仅支持亮度/对比度/伽马控制
4. 不支持 Intel 显卡
5. 仅在 **无边框(Borderless)模式** 下工作
6. 未测试 NVIDIA Optimus 环境(多为笔记本电脑)

## TODO / 功能计划
- [x] 进程焦点检测
- [x] 数字饱和度数值调整
- [x] 伽马值调整
- [x] 亮度、对比度、伽马值修改
- [x] 图形界面
- [x] ini 或 json 配置
- [x] 可更改监控的进程(不局限于 EscapeFromTarkov)
- [x] 更改目标显示器
- [x] 最小化到托盘
- [x] 配置文件(Profiles)
- [ ] 快捷键
- [ ] 修改 EFT 游戏内设置(帧数限制或画质设置)

感谢您的支持!
