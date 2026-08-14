# Kinogoblin Editor

A collection of Unity Editor productivity tools bundled as a single UPM package
(`com.kinogoblin.editor`). Most of it lives in one consolidated **Kinogoblin tools**
window (`Tools > Kinogoblin tools`), plus a couple of dedicated windows/overlays
described below.

## Favorite Assets

A dockable window (`Tools > Kinogoblin tools > FavoriteAssets > ShowFavoritesWindow`,
hotkey `Shift+T`) for bookmarking things you come back to often:

- **Personal Favorites** – favorite any project asset (`Ctrl+Shift+T`), grouped by
  type, or switch to grouping by your own custom lists and drag favorites into them.
- **History** – recently used assets, sorted by recency or frequency.
- **Scene Favorites** – bookmark GameObjects per open scene (drag from the
  Hierarchy, or the same `Ctrl+Shift+T` hotkey when this tab is active); click a
  bookmark to select/ping it in the scene. Favorited objects get a star marker in
  both the Project window and the Hierarchy.
- **Camera Bookmarks** – a Scene View overlay (next to Unity's own Tools overlay)
  for saving and jumping back to camera positions, stored per scene.
- **Scene Asset Usage** – which project assets are actually used in the current scene.
- Multi-user profiles (per Windows user, with override), so favorites/history don't
  get mixed between teammates on a shared machine.

## Hierarchy & Project

- **Advanced Hierarchy View** – per-component icons next to GameObjects (colliders,
  renderers, Rigidbody, Camera, UI elements, etc.), configurable per-type in
  `Project Settings > KinogoblinEditor`.
- Prefix-based colored hierarchy rows, `Folder` marker GameObjects, and predefined
  scene/project folder catalogs for quickly scaffolding a project structure.
- Find/Cleanup Missing Scripts across all open scenes, one-click assembly recompile.

## GameObject & Mesh tools

- Pivot tools (center pivot on bounds, match a child's transform), object grouping,
  and scale reset.
- Export selected mesh(es) to `.asset`/`.obj`, with optional auto-generated collider
  or NavMesh obstacle child objects.
- Batch material tools: apply a material to every renderer under a selection, or
  extract shared materials to disk.
- Copy/paste MeshRenderer settings and arbitrary components across a selection.

## Project & build workflow

- Scene setup snapshots (`Save Scene Setup As...`) to save/restore which scenes are
  open, loaded, and active.
- Custom Android build panel: build path, keystore/alias, Addressables
  integration, and a post-build sound/open-output-folder option.
- One-click installers for common third-party UPM packages (NaughtyAttributes,
  AssetUsageDetector, UniTask, UniRx, Graphy, MRTK GraphicsTools, ...), plus
  self-update via the OpenUPM scoped registry.
- Timeline duplication tool: clone a `PlayableAsset` + `PlayableDirector`, rebinding
  the copy's tracks automatically.
- Editor window layout presets.
