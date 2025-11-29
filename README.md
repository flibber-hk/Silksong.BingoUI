# BingoUI

A Hollow Knight: Silksong mod to show counts of various in-game statistics
on the screen. This is primarily intended as an tool for Bingo races.

## Installation

The latest version of the mod can be downloaded from the actions tab (requires
a github account). You will also need dependencies downloaded from Thunderstore,
the list of dependencies is available in this repo at thunderstore/thunderstore.toml
(version numbers don't matter).

Make sure that the en.json file is placed in a languages dir next to the mod dll.

When this mod is ready for release, I'll remove this section (with the expectation
that people install it with a Thunderstore-aware mod installer that can automatically
handle dependencies).

## Notes

For the consumable collectible trackers (e.g. rosary necklaces), the text appears as X(Y), where
X is the number currently in the inventory and Y is the total number ever picked up. Y excludes
from reusable sources such as rosary machines and shops.

## WIP

This mod is currently a work in progress. Icons can and should be created from the
template in the assets folder.

Anything that requires save data does not behave properly at the moment. It will
be reset when you close the game, but will *not* be reset when entering a different
save file.

Things like config options, more counters (obviously), and ItemChanger integration,
are planned.

## Requested counters

* Craftmetal, relics, cogheart pieces
* Shakra in different areas
