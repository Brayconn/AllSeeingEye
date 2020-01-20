# How the heck do I mod Guxt?!
Modding guxt is pretty simple, assuming you know how to mod Cave Story.

And before you ask, yes, Guxt runs at 50 FPS too.

# Stages
Unlike Cave Story, Guxt's stages are hard-coded, meaning you can't easily add stages like you can with Cave Story.
All-Seeing Eye does have support for the eventual future where you *can* change this, but for now, you're stuck with 6 stages.

# The Screen
In Guxt, the screen size is 8x10 tiles.

The screen starts at the very bottom of the stage, and moves until it reaches the very top, then stops.

Any entities that move are unaffected by the screen scrolling, only the background and stationary entities are affected


# Tiles
Tiles are the same as CS, but with far less types.

Tile can be any combination of Foreground/Background, and Solid/Non-Solid/Breakable (Breakable blocks are always solid).

This is reflected in `tiletypes.png` using the following code:

|Symbol|Meaning|
|---|---|
|Hollow Square|Background|
|Solid Square|Foreground|
|Green Triangle|Solid|
|Grey Color|Breakable|

# Entities
Every entity has the following data:
```C
struct Entity
{
    int Unused; //Unused variable
    int X, Y; //Position of the entity
    int ID; //What type of entity this is
    int ExtraInfo; //The entity's properties. Contents/interpretation depends on the entity
}
```
(Note: this code does not reflect the Entity class exactly as written. See [`GuxtModdingFramework/Entities.cs`](GuxtModdingFramework/Entities.cs) for the exact spec.)

Entity's load as soon as they appear on screen (as in, the very first pixel).

# Scripts
Guxt does not have any script files.
If you want something to happen, place down an NPC at a choice spot on the map, and it should do its thing once it appears on screen.

# Images
All of Guxt's images have been scrambled using an algorithm that Pixel invented.

If you want to edit the images, you'll need to unscramble them, make your changes, then scramble them again.
Alternatively, use a patch to disable the algorithm alltogether.

# Music
As with all of Pixel's newer games, Guxt uses pxtone file for its music.
Hoever, the version of the pxtone.dll it uses is so old that most newly made songs won't work.

To fix this, simply replace the dll with a newer version.