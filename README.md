# HardwareHud

🇬🇧 [English](#english) · 🇫🇷 [Français](#français)

> Made with **Claude AI** (Anthropic). / Réalisé avec **Claude AI** (Anthropic).

---

## English

In-game hardware monitoring HUD for **Beat Saber 1.40.8** (BSIPA plugin). A floating panel shows the current value **and a history graph** for each metric:

- CPU temperature and usage
- GPU temperature, **GPU Hot Spot** and usage
- VRAM used / total and VRAM temperature
- System RAM used / total

Temperatures turn green → orange → red according to thresholds you choose. The panel can be moved, rotated and resized in VR or with the keyboard (FPFC), with separate positions for the menu and for songs.

### Requirements

- Beat Saber 1.40.8 with **BSIPA** and **BSML** (install them with ModAssistant or BSManager)
- Windows and the [.NET SDK](https://dotnet.microsoft.com/download) (or Visual Studio 2022+) to build
- An NVIDIA, AMD or Intel GPU (sensors are read with [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor))

### Build and install

1. Tell the project where your game is. Create a file named `HardwareHud.csproj.user` next to `HardwareHud.csproj`:

   ```xml
   <Project>
     <PropertyGroup>
       <BeatSaberDir>D:\steam\steamapps\common\Beat Saber</BeatSaberDir>
     </PropertyGroup>
   </Project>
   ```

   (Or pass it on the command line: `dotnet build -c Release -p:BeatSaberDir="C:\...\Beat Saber"`.)

2. Build: `dotnet build -c Release` (or *Build → Rebuild Solution* in Visual Studio).
3. The build copies `HardwareHud.dll` to `Plugins\` and the dependencies to `Libs\`. Do it by hand from `bin\...\net472\` if needed.
4. Start the game. `UserData\HardwareHud.json` is created on first launch.

> **CPU temperature needs administrator rights.** The sensor library loads a driver that Windows only allows for elevated processes. Run Beat Saber (or your launcher, e.g. BSManager) **as administrator**, otherwise the CPU temperature shows `N/A`. GPU and RAM work without it.

### In-game menu

Open **MODS → HardwareHud**. Changes apply instantly and are saved.

| Tab | What it does |
|---|---|
| Tracked info | Turn each line on/off (CPU, GPU, Hot Spot, VRAM, VRAM temp, RAM) |
| Temp. thresholds | Orange / red thresholds (°C) for CPU, GPU and VRAM |
| Other thresholds | GPU Hot Spot thresholds, optional coloring of usage lines and their % thresholds |
| Display | Show in menu / in song, background opacity, refresh rate, language (auto / en / fr) |

### Moving the panel

Click **both thumbsticks** at once (VR) or press **F8** to enter edit mode, then:

| Input | Action |
|---|---|
| Grip (one hand) | Grab, move and rotate the panel |
| Thumbstick up/down while gripping | Resize |
| Arrow keys, PageUp / PageDown | Move (X/Y, depth) |
| `1` / `2` | Rotate left / right |
| `+` / `-` | Resize |

Repeat the gesture (or F8) to confirm and save. There are two profiles: one for the menu and one in song — set each once.

### Configuration

Everything is also stored in `UserData\HardwareHud.json` (lines shown, thresholds, opacity, refresh `updateIntervalMs`, `language`, `editKey`, positions...).

### Troubleshooting

- **No HardwareHud button in MODS**: check the game log in `Logs\` for lines containing `HardwareHud` or `BSML`.
- **A line shows `N/A`**: the sensor is not reported by your hardware (VRAM temp, Hot Spot) or needs admin rights (CPU temp).
- **Build errors about missing assemblies**: check `BeatSaberDir`; BSML must be installed (`Plugins\BSML.dll`).

### Credits and license

Made with Claude AI (Anthropic). Hardware sensors via [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) (MPL-2.0). Beat Saber is a trademark of Beat Games; this is an unofficial community mod.

---

## Français

HUD de monitoring matériel en jeu pour **Beat Saber 1.40.8** (plugin BSIPA). Un panneau flottant affiche la valeur actuelle **et un graphique d'historique** pour chaque mesure :

- Température et utilisation du CPU
- Température, **GPU Hot Spot** et utilisation du GPU
- VRAM utilisée / totale et température de la VRAM
- RAM système utilisée / totale

Les températures passent du vert à l'orange puis au rouge selon les seuils que tu choisis. Le panneau se déplace, se tourne et se redimensionne en VR ou au clavier (FPFC), avec des positions séparées pour le menu et pour les parties.

### Prérequis

- Beat Saber 1.40.8 avec **BSIPA** et **BSML** (à installer avec ModAssistant ou BSManager)
- Windows et le [SDK .NET](https://dotnet.microsoft.com/download) (ou Visual Studio 2022+) pour compiler
- Un GPU NVIDIA, AMD ou Intel (capteurs lus avec [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor))

### Compilation et installation

1. Indique où se trouve ton jeu. Crée un fichier `HardwareHud.csproj.user` à côté de `HardwareHud.csproj` :

   ```xml
   <Project>
     <PropertyGroup>
       <BeatSaberDir>D:\steam\steamapps\common\Beat Saber</BeatSaberDir>
     </PropertyGroup>
   </Project>
   ```

   (Ou passe-le en ligne de commande : `dotnet build -c Release -p:BeatSaberDir="C:\...\Beat Saber"`.)

2. Compile : `dotnet build -c Release` (ou *Build → Rebuild Solution* dans Visual Studio).
3. Le build copie `HardwareHud.dll` dans `Plugins\` et les dépendances dans `Libs\`. Fais-le à la main depuis `bin\...\net472\` si besoin.
4. Lance le jeu. `UserData\HardwareHud.json` est créé au premier démarrage.

> **La température CPU demande les droits administrateur.** La bibliothèque de lecture charge un pilote que Windows n'autorise que pour un processus élevé. Lance Beat Saber (ou ton lanceur, par exemple BSManager) **en administrateur**, sinon la température CPU affiche `N/A`. Le GPU et la RAM marchent sans.

### Menu en jeu

Ouvre **MODS → HardwareHud**. Les changements s'appliquent tout de suite et sont sauvegardés.

| Onglet | Rôle |
|---|---|
| Infos suivies | Active/désactive chaque ligne (CPU, GPU, Hot Spot, VRAM, temp VRAM, RAM) |
| Seuils temp. | Seuils orange / rouge (°C) du CPU, du GPU et de la VRAM |
| Autres seuils | Seuils du Hot Spot GPU, coloration optionnelle des utilisations et leurs seuils en % |
| Affichage | Visible dans le menu / en partie, opacité du fond, rafraîchissement, langue (auto / en / fr) |

### Déplacer le panneau

Clique **les deux sticks** en même temps (VR) ou appuie sur **F8** pour passer en mode édition, puis :

| Entrée | Action |
|---|---|
| Grip (une main) | Attraper, déplacer et tourner le panneau |
| Stick haut/bas pendant le grip | Changer la taille |
| Flèches, PageUp / PageDown | Déplacer (X/Y, profondeur) |
| `1` / `2` | Tourner à gauche / à droite |
| `+` / `-` | Taille |

Refais le même geste (ou F8) pour valider et sauvegarder. Il y a deux profils : un pour le menu et un en partie, à régler une fois chacun.

### Configuration

Tout est aussi dans `UserData\HardwareHud.json` (lignes affichées, seuils, opacité, rafraîchissement `updateIntervalMs`, `language`, `editKey`, positions...).

### Dépannage

- **Pas de bouton HardwareHud dans MODS** : regarde le log du jeu dans `Logs\` et cherche les lignes avec `HardwareHud` ou `BSML`.
- **Une ligne affiche `N/A`** : le capteur n'est pas fourni par ton matériel (temp VRAM, Hot Spot) ou demande les droits admin (temp CPU).
- **Erreurs de build sur des assemblies manquants** : vérifie `BeatSaberDir` ; BSML doit être installé (`Plugins\BSML.dll`).

### Crédits et licence

Réalisé avec Claude AI (Anthropic). Capteurs matériels via [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) (MPL-2.0). Beat Saber est une marque de Beat Games ; ce mod est communautaire et non officiel.
