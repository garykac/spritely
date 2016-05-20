# Release Notes

Summary Summary of changes for each release

For a complete list of release notes, see [CHANGES.txt](http://garykac.github.io/spritely/CHANGES.txt)

## 0.14.22

Updated UI and tutorials plus changes to work with devkitARM release 24. Various consistency checks to ensure that tutorials work
properly with each release. Support for background images. Undo for map editing.

* UI changes:
  * MDI tabbed interface
  * Background tiles and maps are in same window now
  * Display options moved into toolbox (from Options menu)
  * Floodfill tool for maps
  * Application icon
  * Transparent pixels now displayed with pattern (instead of red 'x')
* Generated code:
  * Added CheckKeyPress, CheckKeyHeld.
  * Broke SetupStage and Update into separate routines for each stage.
  * backgrounds.cpp|h renamed to background_maps.cpp|h
  * Added background_images.cpp|h
  * Added game_utils.cpp
  * Added code to manage game stages
* Tutorials
  * Automated tutorial verification
* Improved undo support
  * Added for background map editing
  * Basic unittests
* Various bug fixes
  * Undo bugs (when duplicating sprites, or when loading from file, can't redo the first undo, restore palette)
  * Map tile id bug (wrote internal id instead of export id)

## 0.13.17

Primarily a backend update - very little change to the UI, but significant changes to the way things are stored internally. Also, this introduces a new file format which is needed to support upcoming features.

* Update generated code:
  * Add palettes.h to store palette types and definitions.
    * Select "Update project files" when exporting.
  * In sprite.h:
    * Add constant for number of palettes (currently 1) {{{'kNumPalettes'}}}
    * Add constant for number of spritesets (currently 1) {{{'kNumSpritesets'}}}
    * Add constant for each spriteset (currently only 1) {{{'kSpriteset_' + <spriteset-name>}}}
    * Make sprite constants begin with spriteset name: {{{'k' + <spriteset-name> + '_' + <sprite-name>}}}. Previously, is was {{{'kSprite_' + <sprite-name>}}}
  * In backgrounds.h:
    * Add constant for number of bgpalettes (currently 1) {{{'kNumBgPalettes'}}}
    * Add constant for number of bgtilesets (currently 1) {{{'kNumBgTilesets'}}}
    * Add constant for number of bgmaps (currently 1) {{{'kNumBgMaps'}}}
    * Add constant for each bgtileset (currently only 1) {{{'kBgTileset_' + <tileset-name>}}}
    * Add constant for each bgmap (currently only 1) {{{'kBgMap_' + <map-name>}}}
    * Add constants for each background tile: 1x1: {{{'k' + <tileset-name> + '_' + <bgsprite-name>}}}. if multiple tiles, add {{{'_' + <tile>}}} at end
* Fix exception when clicking on edit-sprite pane when no sprite is selected.
* Change save file format to support/prepare for future enhancements:
  * multiple spritesets, palettes, bgtilesets, bgmaps
  * 256-color palettes, bgimages, sounds
  * (note: none of these features are implemented yet, but are planned for future releases)
* Create new classes to manage multiple palettes, spritesets and maps
* Add new project interface prototype (devel version only)
* Add unit tests for file load (old/new version)

## 0.8.7

* Auto-generate collision bitmasks for each sprite.
* Add pixel-perfect collision routines to collision.cpp
* Add (debug only) CollisionTest dialog.
* Change generated code to use a class for GameState instead of a struct.
* Fix bug where undo after editing a duplicated sprite would result in an empty sprite. Repro: duplicate sprite with some pixels set, edit the new sprite, undo edit results in empty sprite.
* Fix bug where deleting multiple sprites in a row would result in no sprite becoming the current selection after the delete.
* Fix bug where collision.(cpp|h) were not included in the auto-generated Programmer's Notepad project.

## 0.7.6

* Fix bug where DC_FlushRange wasn't being called for NDS projects. NDS projects worked in emulation, but not on real hardware.
* Fix compilation problem with NDS projects referring to GBA headers (in collision.cpp).
* Enable backgrounds in NDS.
* Add GBA/NDS tools to dispay screen outline on background map
* Fix bug where files in the "Recent Files" menu that no longer existed would still try to be opened.
* Keep track of current platform so that the export dialog doesn't need to be updated each time for NDS.
* Inherit platform from current state of GBA/NDS tools on BackgroundMap tab.
* Add option to enable/disable screen boundary and grid in Map edit.
* Add option to enable/disable sprite pixel and sprite tile grid

## 0.7.1

* Merge VerticalFlipObject and HorizontalFlipObject into FlipObject
* Merge ShowObject and HideObject into ShowObject
* Automatically generate Programmer's Notepad project (pnproj) file when exporting
* Add Rotate (90 clockwise, 90 counterclockwise, 180)
* Add Flip (horizontal, vertical, both)
* Add File::Recent Files menu
* Fix bug where sprites didn't get moved into correct spritelist group when resized larger
* Fix bug where undoing a sprite resize (or rotate) would render the sprite in the old group in the spritelist.

## 0.6.8

* Change InitObject() to no longer require starting x,y coords. Add MoveObjectTo() after InitObject() in generated code. This allows us to call GetObjectWidth(), et al, when calculating the initial position of the object.
* Add option in Export dialog to update the project. This updates all project files except for game_state.(cpp|h) and makes it easier for previously generated projects to be updated to the latest project code (without overwriting the project-specific code in game_state.(cpp|h).
* Update About dialog to include reference to http://code.google.com/p/spritely location.
