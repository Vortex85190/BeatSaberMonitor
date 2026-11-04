# HardwareHud

## Présentation / Overview

### Français
HardwareHud est un mod pour **Beat Saber 1.40.8** qui affiche dans le jeu les informations matérielles du PC dans un HUD flottant : utilisation et température du CPU et du GPU, GPU Hot Spot, VRAM utilisée et totale, ainsi que RAM utilisée et totale. Chaque mesure peut afficher sa valeur actuelle et un graphique d'historique. Les températures changent de couleur selon les seuils configurés. Le panneau peut être déplacé, tourné et redimensionné en VR ou au clavier, avec des positions séparées pour le menu et les parties.

### English
HardwareHud is a mod for **Beat Saber 1.40.8** that displays the computer's hardware information in an in-game floating HUD: CPU and GPU usage and temperature, GPU Hot Spot, used and total VRAM, and used and total RAM. Each metric can show its current value and a history graph. Temperatures change color according to the configured thresholds. The panel can be moved, rotated and resized in VR or with the keyboard, with separate positions for the menu and songs.

### 日本語
HardwareHud は **Beat Saber 1.40.8** 用の mod です。ゲーム内のフローティング HUD に、CPU と GPU の使用率・温度、GPU Hot Spot、使用中 / 合計 VRAM、使用中 / 合計 RAM など、PC のハードウェア情報を表示します。各項目には現在値と履歴グラフを表示できます。温度は設定したしきい値に応じて色が変わります。パネルは VR またはキーボードで移動、回転、サイズ変更でき、メニューとプレイ中で別々の位置を設定できます。

### 한국어
HardwareHud는 **Beat Saber 1.40.8**용 모드입니다. 게임 내 플로팅 HUD에 CPU와 GPU 사용률 및 온도, GPU Hot Spot, 사용 중 / 전체 VRAM, 사용 중 / 전체 RAM 등 PC 하드웨어 정보를 표시합니다. 각 항목에는 현재 값과 기록 그래프를 표시할 수 있습니다. 온도는 설정한 임계값에 따라 색상이 바뀝니다. 패널은 VR 또는 키보드로 이동, 회전, 크기 조절할 수 있으며 메뉴와 플레이 중 위치를 따로 설정할 수 있습니다.

> Made with **Claude AI** (Anthropic). / Réalisé avec **Claude AI** (Anthropic).

---

## English

In-game hardware monitoring HUD for **Beat Saber 1.40.8** (BSIPA plugin). A floating panel displays the current value and a history graph for:

- CPU temperature and usage
- GPU temperature, GPU Hot Spot and usage
- VRAM used / total and temperature
- System RAM used / total

Temperatures change from green to orange to red according to the thresholds you choose. The panel can be moved, rotated and resized in VR or with the keyboard (FPFC), with separate positions for the menu and songs.

### Requirements

- Beat Saber 1.40.8 with BSIPA and BSML, installed with ModAssistant or BSManager
- Windows and the [.NET SDK](https://dotnet.microsoft.com/download), or Visual Studio 2022+, to build
- An NVIDIA, AMD or Intel GPU; sensors are read with [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor)

### Build and install

1. Create `HardwareHud.csproj.user` next to `HardwareHud.csproj` and set the Beat Saber directory:

   ```xml
   <Project>
     <PropertyGroup>
       <BeatSaberDir>D:\steam\steamapps\common\Beat Saber</BeatSaberDir>
     </PropertyGroup>
   </Project>
   ```

Alternatively, run `dotnet build -c Release -p:BeatSaberDir="C:\...\Beat Saber"`.
2. Build with `dotnet build -c Release`, or use **Build → Rebuild Solution** in Visual Studio.
3. The build copies `HardwareHud.dll` to `Plugins\` and dependencies to `Libs\`. Copy them manually from `bin\...\net472\` if necessary.
4. Start the game. `UserData\HardwareHud.json` is created on first launch.

### CPU permissions

CPU usage is read without administrator rights. CPU temperature depends on the processor and LibreHardwareMonitor. If its sensor requires a driver that cannot be accessed by a standard user, only CPU temperature displays `N/A`; the other metrics continue to work. Beat Saber should not need to be started as administrator.

### In-game menu

Open **MODS → HardwareHud**. Changes apply immediately and are saved.

| Tab | Function |
|---|---|
| Tracked info | Enable or disable CPU, GPU, Hot Spot, VRAM, VRAM temperature and RAM lines |
| Temp. thresholds | Orange and red thresholds in °C for CPU, GPU and VRAM |
| Other thresholds | GPU Hot Spot thresholds, optional usage coloring and percentage thresholds |
| Display | Visibility in the menu or songs, background opacity, refresh rate and language |

Supported language values are `auto`, `en`, `fr`, `ja` and `ko`. The menu must be reopened after changing the language.

### Moving the panel

Press both thumbsticks at once in VR, or press `F8`, to enter edit mode. Grip the panel to move or rotate it. Move the thumbstick up or down while gripping to resize it. Arrow keys and `PageUp` / `PageDown` move the panel; `1` and `2` rotate it; `+` and `-` resize it. Repeat the gesture or press `F8` to save. Menu and song positions are separate.

### Configuration and troubleshooting

Settings are stored in `UserData\HardwareHud.json`, including displayed lines, thresholds, opacity, `updateIntervalMs`, `language`, `editKey` and positions. If the MODS button is missing, check `Logs\` for `HardwareHud` or `BSML`. If a value is `N/A`, the sensor may be unavailable on the hardware; CPU temperature may also require a compatible LibreHardwareMonitor sensor. Build errors about missing assemblies usually mean that `BeatSaberDir` is incorrect or `Plugins\BSML.dll` is missing.

### Credits and license

Hardware sensors use [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) (MPL-2.0). Beat Saber is a trademark of Beat Games; this is an unofficial community mod.

---

## Français

HUD de monitoring matériel en jeu pour **Beat Saber 1.40.8** (plugin BSIPA). Un panneau flottant affiche la valeur actuelle et un graphique d'historique pour :

- la température et l'utilisation du CPU ;
- la température, le Hot Spot et l'utilisation du GPU ;
- la VRAM utilisée / totale et sa température ;
- la RAM système utilisée / totale.

Les températures passent du vert à l'orange puis au rouge selon les seuils choisis. Le panneau peut être déplacé, tourné et redimensionné en VR ou au clavier (FPFC), avec des positions séparées pour le menu et les parties.

### Prérequis

- Beat Saber 1.40.8 avec BSIPA et BSML, installés avec ModAssistant ou BSManager ;
- Windows et le [SDK .NET](https://dotnet.microsoft.com/download), ou Visual Studio 2022+, pour compiler ;
- un GPU NVIDIA, AMD ou Intel ; les capteurs sont lus avec [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor).

### Compilation et installation

1. Crée `HardwareHud.csproj.user` à côté de `HardwareHud.csproj` et indique le dossier de Beat Saber :

   ```xml
   <Project>
     <PropertyGroup>
       <BeatSaberDir>D:\steam\steamapps\common\Beat Saber</BeatSaberDir>
     </PropertyGroup>
   </Project>
   ```

Tu peux aussi exécuter `dotnet build -c Release -p:BeatSaberDir="C:\...\Beat Saber"`.
2. Compile avec `dotnet build -c Release`, ou utilise **Build → Rebuild Solution** dans Visual Studio.
3. Le build copie `HardwareHud.dll` dans `Plugins\` et les dépendances dans `Libs\`. Si nécessaire, copie-les manuellement depuis `bin\...\net472\`.
4. Lance le jeu. `UserData\HardwareHud.json` est créé au premier démarrage.

### Droits CPU

L'utilisation CPU est lue sans droits administrateur. La température CPU dépend du processeur et de LibreHardwareMonitor. Si son capteur nécessite un pilote inaccessible à un utilisateur standard, seule la température CPU affiche `N/A` ; les autres mesures continuent de fonctionner. Il ne devrait donc pas être nécessaire de lancer Beat Saber en administrateur.

### Menu en jeu

Ouvre **MODS → HardwareHud**. Les changements s'appliquent immédiatement et sont sauvegardés.

| Onglet | Fonction |
|---|---|
| Infos suivies | Active ou désactive les lignes CPU, GPU, Hot Spot, VRAM, température VRAM et RAM |
| Seuils temp. | Seuils orange et rouge en °C pour le CPU, le GPU et la VRAM |
| Autres seuils | Seuils du Hot Spot GPU, coloration facultative des utilisations et seuils en pourcentage |
| Affichage | Visibilité dans le menu ou les parties, opacité du fond, fréquence de rafraîchissement et langue |

Les valeurs de langue disponibles sont `auto`, `en`, `fr`, `ja` et `ko`. Il faut rouvrir le menu après un changement de langue.

### Déplacer le panneau

Appuie simultanément sur les deux sticks en VR, ou sur `F8`, pour entrer en mode édition. Utilise le grip pour déplacer ou tourner le panneau. Le stick haut/bas pendant le grip change sa taille. Les flèches et `PageUp` / `PageDown` déplacent le panneau ; `1` et `2` le font tourner ; `+` et `-` changent sa taille. Refais le même geste ou appuie sur `F8` pour sauvegarder. Les positions du menu et des parties sont séparées.

### Configuration et dépannage

Les réglages sont enregistrés dans `UserData\HardwareHud.json` : lignes affichées, seuils, opacité, rafraîchissement `updateIntervalMs`, `language`, `editKey`, positions... Si le bouton MODS est absent, vérifie `Logs\` et recherche `HardwareHud` ou `BSML`. Si une valeur affiche `N/A`, le capteur peut être indisponible sur le matériel ; la température CPU peut aussi nécessiter un capteur LibreHardwareMonitor compatible. Les erreurs de build sur des assemblies manquants indiquent généralement que `BeatSaberDir` est incorrect ou que `Plugins\BSML.dll` est absent.

### Crédits et licence

Les capteurs matériels utilisent [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) (MPL-2.0). Beat Saber est une marque de Beat Games ; ce mod est communautaire et non officiel.

---

## 日本語

**Beat Saber 1.40.8** 用のゲーム内ハードウェア監視 HUD（BSIPA プラグイン）です。フローティングパネルに次の現在値と履歴グラフを表示します。

- CPU の温度と使用率
- GPU の温度、Hot Spot、使用率
- 使用中 / 合計 VRAM と温度
- 使用中 / 合計システム RAM

温度は設定したしきい値に応じて緑、オレンジ、赤に変わります。パネルは VR またはキーボード（FPFC）で移動、回転、サイズ変更できます。メニューとプレイ中で別々の位置を保存できます。

### 必要条件

- ModAssistant または BSManager で BSIPA と BSML を導入した Beat Saber 1.40.8
- ビルド用の Windows と [.NET SDK](https://dotnet.microsoft.com/download)、または Visual Studio 2022 以降
- NVIDIA、AMD、Intel のいずれかの GPU。[LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) でセンサーを読み取ります

### ビルドとインストール

1. `HardwareHud.csproj` の隣に `HardwareHud.csproj.user` を作成し、Beat Saber のフォルダーを指定します。

   ```xml
   <Project>
     <PropertyGroup>
       <BeatSaberDir>D:\steam\steamapps\common\Beat Saber</BeatSaberDir>
     </PropertyGroup>
   </Project>
   ```

または `dotnet build -c Release -p:BeatSaberDir="C:\...\Beat Saber"` を実行します。
2. `dotnet build -c Release` でビルドします。Visual Studio では **Build → Rebuild Solution** も使用できます。
3. ビルドにより `HardwareHud.dll` が `Plugins\` に、依存ファイルが `Libs\` にコピーされます。必要に応じて `bin\...\net472\` から手動でコピーしてください。
4. ゲームを起動します。初回起動時に `UserData\HardwareHud.json` が作成されます。

### CPU の権限

CPU 使用率は管理者権限なしで読み取ります。CPU 温度はプロセッサーと LibreHardwareMonitor に依存します。標準ユーザーがアクセスできないドライバーをセンサーが必要とする場合、`N/A` になるのは CPU 温度だけで、他の測定値は引き続き動作します。そのため、Beat Saber を管理者として起動する必要はありません。

### ゲーム内メニュー

**MODS → HardwareHud** を開きます。変更はすぐに適用され、保存されます。

| タブ | 機能 |
|---|---|
| Tracked info | CPU、GPU、Hot Spot、VRAM、VRAM 温度、RAM の各行を表示 / 非表示にします |
| Temp. thresholds | CPU、GPU、VRAM のオレンジ / 赤のしきい値（°C）を設定します |
| Other thresholds | GPU Hot Spot のしきい値、使用率の色付け、バックグラウンドの不透明度しきい値を設定します |
| Display | メニューまたは曲中での表示、背景の不透明度、更新頻度、言語を設定します |

サポートされている言語は `auto`、`en`、`fr`、`ja`、`ko` です。言語を変更した後はメニューを再度開く必要があります。

### パネルの移動

VR では両方のスティックを同時に押すか、`F8` を押して編集モードに入ります。グリップでパネルを移動または回転します。グリップ中にスティックを上下に動かすとサイズを変更できます。矢印キーと `PageUp` / `PageDown` で移動し、`1` と `2` で回転し、`+` と `-` でサイズを変更します。同じ動作をもう一度行うか `F8` を押すと保存されます。メニューとプレイ中の位置は別々です。

### 設定とトラブルシューティング

設定は `UserData\HardwareHud.json` に保存されます。表示行、しきい値、不透明度、`updateIntervalMs`、`language`、`editKey`、位置などが含まれます。MODS ボタンが absent、`Logs\` で `HardwareHud` または `BSML` を検索します。値が `N/A` の場合、そのセンサーがハードウェアにない可能性があります。CPU 温度には互換性のある LibreHardwareMonitor センサーが必要な場合もあります。アセンブリ不足のビルドエラーは、一般に `BeatSaberDir` が正しくないか、`Plugins\BSML.dll` がないことを示します。

### クレジットとライセンス

ハードウェアセンサーには [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor)(MPL-2.0)を使用しています。Beat Saber は Beat Games の商標です。本 mod は非公式のコミュニティ mod です。

---

## 한국어

**Beat Saber 1.40.8**용 게임 내 하드웨어 모니터링 HUD(BSIPA 플러그인)입니다. 플로팅 패널에 다음 항목의 현재 값과 기록 그래프를 표시합니다.

- CPU 온도와 사용률
- GPU 온도, Hot Spot, 사용률
- 사용 중 / 전체 VRAM 및 온도
- 사용 중 / 전체 시스템 RAM

온도는 설정한 임계값에 따라 녹색, 주황색, 빨간색으로 바뀝니다. 패널은 VR 또는 키보드(FPFC)로 이동, 회전, 크기 조절할 수 있으며 메뉴와 플레이 중 위치를 따로 저장합니다.

### 요구 사항

- ModAssistant 또는 BSManager로 BSIPA와 BSML을 설치한 Beat Saber 1.40.8
- 빌드용 Windows 및 [.NET SDK](https://dotnet.microsoft.com/download), 또는 Visual Studio 2022 이상
- NVIDIA, AMD 또는 Intel GPU. 센서는 [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor)로 읽습니다

### 빌드 및 설치

1. `HardwareHud.csproj` 옆에 `HardwareHud.csproj.user`를 만들고 Beat Saber 폴더를 지정합니다。

   ```xml
   <Project>
     <PropertyGroup>
       <BeatSaberDir>D:\steam\steamapps\common\Beat Saber</BeatSaberDir>
     </PropertyGroup>
   </Project>
   ```

또는 `dotnet build -c Release -p:BeatSaberDir="C:\...\Beat Saber"`를 실행합니다.
2. `dotnet build -c Release`로 빌드합니다. Visual Studio에서는 **Build → Rebuild Solution**도 사용할 수 있습니다.
3. 빌드하면 `HardwareHud.dll`은 `Plugins\`에, 종속 파일은 `Libs\`에 복사됩니다. 필요에 따라 `bin\...\net472\`에서 수동으로 복사하십시오.
4. 게임을 시작합니다. 처음 실행할 때 `UserData\HardwareHud.json`이 생성됩니다.

### CPU 권한

CPU 사용률은 관리자 권한 없이 읽습니다. CPU 온도는 프로세서와 LibreHardwareMonitor에 따라 달라집니다. 표준 사용자가 접근할 수 없는 드라이버가 센서에 필요한 경우 `N/A`가 되는 것은 CPU 온도만이며 다른 측정값은 계속 작동합니다. 따라서 Beat Saber를 관리자 권한으로 실행할 필요가 없습니다.

### 게임 내 메뉴

**MODS → HardwareHud**를 엽니다. 변경 사항은 즉시 적용되고 저장됩니다.

| 탭 | 기능 |
|---|---|
| Tracked info | CPU, GPU, Hot Spot, VRAM, VRAM 온도, RAM 항목을 표시하거나 숨깁니다 |
| Temp. thresholds | CPU, GPU, VRAM의 주황색 / 빨간색 임계값(°C)을 설정합니다 |
| Other thresholds | GPU Hot Spot 임계값, 사용률의 색상 표시, 배경의 불투명도 임계값을 설정합니다 |
| Display | 메뉴/플레이 중 표시, 배경 불투명도, 새로 고침 간격, 언어를 설정합니다 |

언어에는 `auto`、`en`、`fr`、`ja`、`ko`를 지정할 수 있습니다. 언어를 변경한 후에는 메뉴를 다시 열어야 합니다.

### 패널 이동

VR에서는 양쪽 스틱을 동시에 누르거나 `F8`을 눌러 편집 모드로 들어갑니다. 그립으로 패널을 이동하거나 회전합니다. 그립 중 스틱을 위아래로 움직이면 크기를 변경할 수 있습니다. 방향키와 `PageUp` / `PageDown`으로 이동하고, `1`과 `2`로 회전하며, `+`와 `-`로 크기를 변경합니다. 같은 동작을 다시 하거나 `F8`을 누르면 저장됩니다. 메뉴와 플레이 중 위치는 서로 다릅니다.

### 설정 및 문제 해결

설정은 `UserData\HardwareHud.json`에 저장됩니다. 표시 항목, 임계값, 불투명도, `updateIntervalMs`, `language`, `editKey`, 위치 등이 포함됩니다. MODS 버튼이 없으면 `Logs`에서 `HardwareHud` 또는 `BSML`을 검색합니다. 값이 `N/A`인 경우 해당 센서가 하드웨어에 없을 수 있습니다. CPU 온도에는 호환되는 LibreHardwareMonitor 센서가 필요할 수도 있습니다. 누락된 어셈블리 빌드 오류는 일반적으로 `BeatSaberDir`이 잘못되었거나 `Plugins\BSML.dll`이 없다는 의미입니다.

### 크레딧 및 라이선스

하드웨어 센서는 [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor)(MPL-2.0)를 사용합니다. Beat Saber는 Beat Games의 상표이며, 이 mod는 비공식 커뮤니티 mod입니다.
