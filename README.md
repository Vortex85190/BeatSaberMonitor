# HardwareHud — monitoring CPU / GPU / VRAM en jeu (Beat Saber 1.40.8)

Plugin BSIPA qui affiche un panneau flottant avec, pour chaque métrique, la valeur actuelle et un graphique d'historique :

- Température CPU + utilisation CPU
- Température GPU + utilisation GPU
- VRAM utilisée / totale

Les couleurs des températures passent de vert à orange (75 °C) puis rouge (85 °C). Seuils réglables dans la config.

## Prérequis

- Beat Saber 1.40.8 avec **BSIPA** installé (via ModAssistant par exemple)
- Windows, SDK .NET installé (`dotnet --version`) pour compiler
- Un GPU NVIDIA, AMD ou Intel (lecture via LibreHardwareMonitorLib)

## Compilation et installation

1. Ouvre un terminal dans ce dossier.
2. Compile en indiquant le chemin de ton jeu (adapte-le) :

   ```
   dotnet build -c Release -p:BeatSaberDir="C:\Program Files (x86)\Steam\steamapps\common\Beat Saber"
   ```

3. Le build copie automatiquement :
   - `HardwareHud.dll` → `Beat Saber\Plugins\`
   - les DLL de dépendances (LibreHardwareMonitorLib, HidSharp…) → `Beat Saber\Libs\`

   Si le copie auto ne se fait pas, fais-le à la main depuis `bin\Release\net472\`.
4. Lance le jeu. Au premier démarrage, `UserData\HardwareHud.json` est créé.

## Positionner le panneau

Le mode édition s'active avec **les deux sticks cliqués en même temps** (VR) ou la touche **F8** (clavier).

| Entrée | Action |
|---|---|
| Grip (une main) | attraper et déplacer / tourner le panneau |
| Stick haut / bas pendant le grip | agrandir / réduire |
| Flèches, PageUp/PageDown | déplacer (X/Y, profondeur) |
| 1 / 2 | tourner |
| + / - | taille |

Relance le même geste pour valider : la position est sauvegardée. Il y a **deux profils** : un pour le menu, un pour en jeu (à faire une fois dans chaque contexte).

## Configuration (`UserData\HardwareHud.json`)

`showCpuTemp`, `showCpuLoad`, `showGpuTemp`, `showGpuLoad`, `showVram` (lignes affichées), `opacity`, `updateIntervalMs` (250–10000, le graphique garde 120 échantillons), `warnTemp` / `hotTemp`, `editKey`, `showInMenu` / `showInSong`, et les positions / rotations / tailles.

## Points d'attention

- **Température CPU** : LibreHardwareMonitorLib charge un pilote pour lire les capteurs CPU, ce qui demande en général de lancer **Beat Saber (ou Steam) en administrateur**. Sans ça, la ligne CPU Temp affichera `N/A` ; le GPU, lui, marche sans droits admin.
- Le positionnement en VR utilise les coordonnées de suivi des manettes ; si tu as un décalage de zone de jeu (room offset) dans Beat Saber, le panneau peut apparaître légèrement décalé pendant l'édition.
- Si le symbole `°` ne s'affiche pas avec la police du jeu, remplace `" °C"` par `" C"` dans `HudController.cs`.
- Le code n'a pas été compilé ni testé contre les DLL du jeu au moment de l'écriture : si une référence manque au build, adapte la liste dans `HardwareHud.csproj`.
