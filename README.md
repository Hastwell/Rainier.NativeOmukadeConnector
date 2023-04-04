# Rainier Native Omukade Connector
![logo](noc-logo.png)

**If you're looking to play with others on an Omukade server, this is all you need.**

This is a BepInEx plugin for the Pokemon TCG Live software (codename "Rainier") that patches the game
to connect to a server running an Omukade-family server (eg, Omukade Cheyenne) instead of offical servers.

## Requirements
* Pokemon TCG Live for Windows
    * _Mobile is not supported, and probably never will._
    * It might work for macOS, but I don't have a Mac.
    * It might work for Linux via Wine, but I don't test this.
* BepInEx 5 (included with the installer release)
* The address of an Omukade server you want to join (eg. `wss://server.example.com:10851`)

## Usage
Start the game using the TCGL Omukade shortcut (or using the command line `--enable-omukade`).
Without either of these, the plugin will assume you want to connect to the offical game servers and disable itself without doing anything.

### Configuration
In the game install directory with the Pokemon TCG Live executable, locate or create the `config-noc.json` file. An example minimum functioning config file:
```json
{"OmukadeEndpoint":"ws://omukade.example.com:10850"}
```
All currently supported configurations are:
* OmukadeEndpoint - **REQUIRED**. Configures the Omukade server to use.
* EnableAllCosmetics (true/false - default false) - _Avoid using._ Very crudely enables the use of all deck cosmetics. Unowned cosmetics may render oddly in selection menus. May or may not be a bannable offense if used on offical servers.
* ForceAllLegalityChecksToSucceed (true/false - default false) - _Debug setting, avoid using._ Forces all decks to appear legal even if they contain out-of-format cards. The server can still reject your invalid deck (official servers do reject, so don't bother using this to cheat.)
* ForceFriendsToBeOnline (true/false - default false) - _Debug setting, avoid using._ Forces all friends to appear online, even if they aren't.
* DumpManifestFileUrl (true/false - default false) - _Debug setting, avoid using._ Dumps the URL containing the current game version's information manifest.

### Compatable Servers
Rainier NOC is compatable with the following server implementations:
* Omukade Cheyenne
* Offical TCGL Servers (when not opting-in to Omukade use via command-line)

### Gameplay
While Omukade is enabled, the following game changes are in effect:
* Deck legality ignores if you own enough/any copies of a card. Decks must still be otherwise legal for your desired format.
* Any legal deck can be played on ladder or in friend matches, even if you don't have all of the cards in that deck.
* Ladder matches will match you against other users on your chosen Omukade server.
* "Online Friends" will only reflect your friends that are connected to your chosen Omukade server.
* You will appear offline to friends on different Omukade servers, or offical servers.
* Rewards of any kind cannot be earned.
* All telemetry is disabled.

All other functions are unchanged, notably:
* Products purchased, packs opened, etc, are not affected.
* Your list of friends, and managing friends. Only their online status is affected by this plugin.
* Decks you build and edits you make will appear in your TCGL account regardless of whether using this plugin or not.
  You may need to change your active deck when switching to offical servers if you don't own the neccisary cards to use that deck.

## Information Disclosure
If using a non-secure server (one starting with `ws://` instead of `wss://`), others may be able to spy on your games with packet capture tools such as Wireshark. Use WSS wherever possible.
Note that Omukade Cheyenne doesn't currently support WSS.

To facilitate gameplay, some account details are sent to your chosen Omukade server.
* The current region you're connected to (eg, West US, East US, Europe, Japan). This is used to trick the game into not
  disconnecting and reconnecting to the Omukade server, which has caused problems in the past.

* The decklist of the current deck you're using is sent every time you start a new game. _(The server does not get to see all of your decks.)_
* Your IGN (in-game name, eg Trainer1234)

Some details are specifically **not** sent:
* The username (not the IGN) and password you use to sign into the game and Pokemon Trainer Club.
* Session or other security tokens that authorize you (or whoever has those tokens) to access your Pokemon Trainer Club account.
* PII from information the game retreives from the Pokemon Organized Play API. (This can include your real name and information about child accounts attached to yours.)

## Game Updates
Game updates do not appear to affect Rainier NOC, other BepInEx plugins, or BepInEx itself. However,
game updates can and have introduced incompatibilities with the patches this tool makes.

## Compiling
* Use Visual Studio 2022 or later, build the project.
* With the .NET 6 SDK, `dotnet build Omukade.NativeOmukadeConnector.sln`

## License
This software is licensed under the terms of the [GNU AGPL v3.0](https://www.gnu.org/licenses/agpl-3.0.en.html)