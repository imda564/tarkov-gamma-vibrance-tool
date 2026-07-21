# EFT ガンマ・彩度自動調整ツール(旧称:tarkov-settings)
![screenshot](./1.png)

**Language:** [English](README.md) | [한국어](README.ko.md) | [Русский](README.ru.md) | [中文](README.zh.md) | 日本語

このリポジトリは [incheon-kim/tarkov-settings](https://github.com/incheon-kim/tarkov-settings) のメンテナンス継続フォークです。本家は2023年10月以降更新が止まっており、未解決のクラッシュ報告も複数残っています。このフォークで修正・追加した内容は以下の通りです:
- NVIDIAのディスプレイハンドルが無効になった際(リモートデスクトップのセッション切り替え、NVIDIAコントロールパネルを閉じる、モニターのスリープ/ホットプラグ)に発生していたクラッシュ — 本家issue [#3](https://github.com/incheon-kim/tarkov-settings/issues/3)、[#17](https://github.com/incheon-kim/tarkov-settings/issues/17) 参照
- タイトルバーのXボタンでウィンドウを閉じた際に設定が保存されない問題(従来はトレイアイコン→Exitからのみ保存)
- `settings.json` が壊れている/不正な場合にアプリがクラッシュしていた問題(現在は警告を表示してデフォルト設定にリセット)
- デフォルトの監視対象プロセスに `EscapeFromTarkovArena` を追加 — 本家issue [#23](https://github.com/incheon-kim/tarkov-settings/issues/23) 参照
- プロファイル機能: 明るさ・コントラスト・ガンマ・彩度の組み合わせを複数名前を付けて保存・呼び出し — 下記[プロファイル](#プロファイル)参照
- プロファイル切り替え用のグローバルホットキー: 他のウィンドウがフォーカスされていても即座に切り替え — 下記[ホットキー](#ホットキー)参照 — 本家issue [#1](https://github.com/incheon-kim/tarkov-settings/issues/1)、[#12](https://github.com/incheon-kim/tarkov-settings/issues/12) で要望されていた機能

## [->**最新版をダウンロード**<-](https://github.com/imda564/tarkov-gamma-vibrance-tool/releases/latest)

[Escape from Tarkov](https://escapefromtarkov.com) の画面色設定を自動的に変更します。

## 仕組み
- [NvAPIWrapper](https://github.com/falahati/NvAPIWrapper) を使用してNVIDIA設定のDigital Vibrance値を変更
- [Win32 APIの呼び出し](https://docs.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-setdevicegammaramp) によりガンマ値を変更

Escape from Tarkovのウィンドウがフォーカスされている時のみ、画面の色設定を変更します。
最小化・復元時はスムーズに切り替わります。

## 対応グラフィックカード
- NVIDIA GPU: **完全対応**(明るさ/コントラスト/ガンマ/彩度)
- AMD GPU: **部分対応**(彩度を除く)
- **Intel/その他は現時点で未対応です。**

## できること
以下の色設定を変更できます:
1. 明るさ(Brightness)
2. コントラスト(Contrast)
3. ガンマ(Gamma)
4. Digital Vibrance Control(彩度)
5. EFTウィンドウがフォーカスされている時のみ適用(Alt+Tab時の画面の急な明滅も防止)

## 使い方
1. アプリを起動(署名されていないため、SmartScreenが警告を出す場合があります)
2. 好みの色設定値を設定
2.1. スライダーのラベルをダブルクリックすると値がリセットされます。
3. アプリを最小化してEFTをプレイ
4. 無効化したい場合はアプリを終了してください

**ウィンドウを閉じる、またはトレイアイコンから終了する際に `settings.json` へ設定が保存されます。**

## プロファイル
左下の入力欄に名前を入力し、**Save** / **Load** / **Del** ボタンで明るさ・コントラスト・ガンマ・彩度の組み合わせを複数保存・呼び出しできます(ゲームごと、時間帯ごとのプロファイルなど)。プロファイルは他の設定と共に `settings.json` に保存されます。

## ホットキー
まずプロファイルを保存し、「Hotkey:」の横にある **Set** ボタンを押してから、割り当てたいキーの組み合わせ(例:`Ctrl+F9`)を押すと、そのプロファイルにホットキーが登録されます。このホットキーはグローバルに動作するため、EFT(または他のどのウィンドウ)がフォーカスされている状態でも、押すだけで即座にそのプロファイルの明るさ・コントラスト・ガンマ・彩度が適用されます。**Clear** でホットキーの割り当てを解除できます。すでに他のアプリケーションが使用している組み合わせの場合は警告が表示されるので、別の組み合わせを選んでください。

## 注意事項
1. EFTウィンドウをアクティブにした際に画面が数回点滅することがありますが、正常動作です。心配ありません。
2. **免責事項: 本ツールの使用によりBSGからBANされるかどうかは保証できません。**
3. AMDは明るさ/コントラスト/ガンマのみ対応
4. Intelグラフィックカードは非対応
5. **ボーダーレスモード** でのみ動作します
6. NVIDIA Optimus環境(主にノートPC)は未検証です

## TODO / 今後の機能
- [x] プロセスのフォーカス検知
- [x] Digital Vibrance値の変更
- [x] ガンマ値の変更
- [x] 明るさ・コントラスト・ガンマ値の変更
- [x] GUI
- [x] iniまたはjsonでの設定
- [x] 監視対象プロセスの変更(EscapeFromTarkov以外にも対応)
- [x] 対象ディスプレイ(モニター)の変更
- [x] トレイへの最小化
- [x] プロファイル機能
- [x] ホットキー
- [ ] EFTのゲーム内設定変更(フレームレート制限やグラフィック設定)

応援ありがとうございます!
