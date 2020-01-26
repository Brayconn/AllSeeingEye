# How the heck do I mod Guxt?!
Modding guxt is pretty simple, assuming you know how to mod Cave Story.

And before you ask, yes, Guxt runs at 50 FPS too.

# Stages
Unlike Cave Story, Guxt's stages are hard-coded, meaning you can't easily add stages like you can with Cave Story.
All-Seeing Eye does have support for the eventual future where you *can* change this, but for now, you're stuck with 6 stages.

The stage advances once you beat a boss, not when you reach the end of the screen. Also, Boss 5 does not advance to the next stage, instead setting the scroll speed to 30(?). 

# The Screen
In Guxt, the screen size is 8x10 tiles.

The screen starts at the very bottom of the stage, and moves until it reaches the very top, then stops.

Any entities that move are unaffected by the screen scrolling, only the background and stationary entities are affected


# Tiles
Tiles can be any combination of Foreground/Background, and Solid/Non-Solid/Breakable (Breakable blocks are always solid).

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

Entities are 1/4th the size of tiles, and can only be placed in one of the four corners of a tile.

Entities are loaded as soon as they appear on screen (as in *the very first pixel*), and only load once (If you make the screen scroll backwards over where an entity was, it will NOT respawn).

# Scripts
Guxt does not have any script files, everything is done with NPC loading.
If you want something to happen, such as the screen to scroll faster, or the background music to change, place down an NPC at a choice spot on the map and it'll do its thing.

# Images
All of Guxt's images have been scrambled using an algorithm that Pixel invented.

If you want to edit the images, you'll need to unscramble them, make your changes, then scramble them again.
Alternatively, use a patch to disable the algorithm alltogether.

# Music
As with all of Pixel's newer games, Guxt uses pxtone files for its music.
Hoever, the version of the pxtone.dll it uses is so old that most newly made songs won't work.

To fix this, simply replace the dll with a newer version.