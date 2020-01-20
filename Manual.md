# How to use All-Seeing Eye

## Overall structure
The struture of ASE is very different from any other pixel editor, namely Booster's Lab and Cave Editor.
On the contrary, ASE works off of a "project file", an XML file with the extension `.gux`, that holds all the info about your exe.

Because of this, you will need to save your project file in a safe place if you're going to patch your exe.
If you aren't going to patch your exe, you *can* survive without saving, but it's still not recommended.

## Window overview
There are three main windows in ASE:
1. Main window - The one that lets you choose what stage to edit, determine how your exe is set up, etc.
2. Stage Editor - Pretty much what it sounds like, edit tiles and entities for each stage.
3. Attribute Editor - Changes how each tile in tileset behaves

The Stage Editor and Attribute Editor function exactly the same, except that the Attribute Editor has all the entity functionality removed.

# Main Window

## Buttons
- `File`
  - `New...` - Creates a new Guxt project from the given Guxt executable
  - `Open...` - Opens an existing 
  - `Save Project` - Saves the current project to the disk
  - `Save Project As...` - Saves the current project
- `Edit`
  - `Scramble Image...` - Scrambles an image
  - `Unscramble Image...` - Unscrambles an image

## Panels

### Stages
Click on a stage to open the Stage Editor

### Images
Right click on any number of images to get quick acces to scrambling/unscrambling

### Attributes
Click on an attribute file to open the Attribute Editor

### Projects
This panel doesn't actually do anything...
The .stgrpj files seem to be leftovers from pixel's own guxt editor.
They are simply listed here for complete-ness

### Properties
This panel lets you change all the properties about your mod.
Should be pretty obivous what things do


# Stage/Attribute Editor

## Buttons
- `File`
  - `Save` - Saves the currently loaded tile and entity data, or the attribute data for that editor
- `Edit`
  - `Delete All Entities` - Deletes all entities (this is a one way process after you save).
- `View` - Each button is a toggle for that graphical item
  - `Tile Types`
  - `Entity Sprites`
  - `Entity Boxes`

The attribute editor has all these buttons and less

## Keyboard Controls
Please note that these are the *default* controls for the stage editor (and the attribute editor, but it can only use Save).

You can change them by editing the .config file

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
You can also use Pick Tile to quickly select whatever tile you're hovering over on the map

## Entity Tab
The top panel lets you edit the currently selected npc. If you have multiple npcs selected, it will edit the first one.

The lower panel lets you select what npc you would like to place. It will change to reflect the type of the first selected npc.

# Advanced features

## Custom entities
As a hack developer, you can get ASE to recognise your custom NPCs.

Simply make a .net framework class library, that has a class that inherits from `GuxtModdingFramework.Entities.EntityShell`.

Compile it to a dll, and put it in the same folder as your project file.

Then, in the project file, find the `<EntityTypes>` section, and put:
```XML
<Type key="ENTITYNUMBER" dll="DLLNAME">NAMESPACE.CLASSNAME</Type>
```
(Replacing the values to be relevant to your dll, of course)

You'll probably also want to add an entry to the `<EntityNames>` list while you're at it.
```XML
<Name key="ENTITYNUMBER">ENTITYNAME</Name>
```