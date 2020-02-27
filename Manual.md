# How to use All-Seeing Eye

## Overall structure
The struture of ASE is very different from any other pixel editor, namely Booster's Lab and Cave Editor.
Whereas those editors regularly make changes to the exe, ASE does not.
Instead, it works off of a "project file", an XML file with the extension `.gux`, that holds all the info about your exe.

Because of this, you will need to save your project file in a safe place if you're going to patch your exe.
If you aren't going to patch your exe, you *can* survive without saving, but it's still not recommended.

## Window overview
There are three main windows in ASE:
1. Main window - The one that lets you choose what stage to edit, determine how your exe is set up, etc.
2. Stage Editor - Pretty much what it sounds like, edit tiles and entities for each stage.
3. Attribute Editor - Changes how each tile in tileset behaves.

The Stage Editor and Attribute Editor function exactly the same, except that the Attribute Editor has all the entity functionality removed.


# Main Window

## Buttons
- `File`
  - `New...` - Creates a new Guxt project from the given Guxt executable
  - `Open...` - Opens an existing Guxt project
  - `Save Project` - Saves the current project to the disk
  - `Save Project As...` - Saves the current project
- `Edit`
  - `Scramble Image...` - Scrambles an image
  - `Unscramble Image...` - Unscrambles an image

## Panels

### Stages
Click on a stage to open the Stage Editor.

### Images
Right click on any number of images to get quick access to scrambling/unscrambling.

### Attributes
Click on an attribute file to open the Attribute Editor.

### Projects
This panel doesn't actually do anything...
The `*.stgrpj` files seem to be leftovers from Pixel's Guxt editor.
They are simply listed here for complete-ness.

### Properties
This panel lets you change all the properties about your mod.

As stated earlier, ASE does not make any changes to your exe. If you see something that looks like it would affect the exe, it doesn't, it's just for display.


# Stage/Attribute Editor

## Buttons
- `File`
  - `Save` - Saves the currently loaded tile and entity data, or the attribute data for that editor.
- `Edit`
  - `Delete All Entities` - Deletes all entities (this is a one way process after you save).
- `View`
  - `Tile Types` - The tile type for each tile on the map is displayed.
  - `Entity Sprites` - Entity sprites/icons appear where entities are placed.
  - `Entity Boxes` - Boxes appear where entities are placed.
  - `Screen Preview` - Draws a box as big as the in-game screen size. Useful for lining up enemies in just the right spot. Use the extra scrollbars in the Stage Editor to move it around.

The attribute editor has all these buttons and less™.

## Keyboard Controls
Please note that these are the *default* controls for the stage editor (and the attribute editor, but it can only use Save and Zoom).
See the section on [Controls] lower down for more info.

|Button|Action|
|---|---|
|+|Zoom In|
|-|Zoom out|
|P|Pick Tile|
|I, Insert|Insert Entity|
|Delete|Delete Entity|
|Ctrl + C|Copy Entities|
|Ctrl + V|Paste Entities|
|Ctrl + S|Save|

## Mouse Controls
Unlike the keyboard, these controls are NOT rebindable. Sorry.

|Button|Action|
|---|---|
|Left|Place Tiles, Select/Move Entities|
|Middle|Pick Tile|
|Right|Context Menu|

## Map Tab
The top panel lets you change things about the map, such as the size.
If you really want, you could edit tiles from here, but I can't recommend it.

The lower panel is the tileset. Click on a tile to select it, then drag around on the map to draw.
You can also use Pick Tile to quickly select whatever tile you're hovering over on the map.

## Entity Tab
The top panel lets you edit the currently selected npc. If you have multiple npcs selected, it *will* edit all of them.

The lower panel lets you select what entity you would like to place. It will change to reflect the type of the first selected NPC.
You can also type while the lower panel is selected to quickly select an entity.


# The .config file

If you're looking to edit the UI or the controls of ASE, you've come to the right place!
It's all stored in the .config file included with your download (or created on first run).

## UI
Under the `<GuxtEditor.UI>` section you can changed the color of various elements.
This covers the selected tile box, the cursor, the entity boxes (selected and not selected), and the screen preview.

Simply set the `<value>` to any [HTML color](https://en.wikipedia.org/wiki/Web_colors).

## Controls
Under the `<GuxtEditor.Keybinds>` section you'll find the keybinds for the stage/attribute editors.

Each `<Keybind>` has two parts: the `<Input>` and the `<Action>`.

The `<KeyValue>` section of each input can be any key from the [Keys Enum](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.keys).
Listing multiple keys is untested. Attempt at your own risk.

The other options, `<Shift>`, `<Control>`, and `<Alt>`, are pretty self explanitory.
Do you want to have to press this key? `true` or `false`.

The `<Action>` should be one of these values, unless you like filling your config file with useless keybinds! 🙃
- `ZoomIn`
- `ZoomOut`
- `PickTile`
- `InsertEntity`
- `DeleteEntities`
- `Copy`
- `Paste`
- `Save`


# The .gux project files

As stated in the beginning, .gux files are just renamed XML files, and can be opened in any your favorite text editor.
Inside is all the info regarding how your exe is set up. This info is also shown in the Properties panel on the right side of the main window.

The only things that isn't shown in that panel is the `<EntityNames>` and `<EntityTypes>` section. These should never have to be edited, unless you're a hack developer who's replacing an entity.
If you *are* a hack developer, you should read the section on [Custom entities].

## Custom entities
As a hack developer, you can get ASE to recognize your custom NPCs.

Simply make a .net framework class library with a class that inherits from `GuxtModdingFramework.Entities.EntityShell`.
Make sure that your class also has a public constructer that takes *only* a `GuxtModdingFramework.Entities.Entity`.

Now compile it to a dll, and put it in the same folder as your project file.

Then, in your mod's project file, find the `<EntityTypes>` section, and put:
```XML
<Type key="ENTITYNUMBER" dll="DLLNAME">NAMESPACE.CLASSNAME</Type>
```
(Replacing the values to be relevant to your dll, of course)

You'll probably also want to add an entry to the `<EntityNames>` list while you're at it.
```XML
<Name key="ENTITYNUMBER">ENTITYNAME</Name>
```