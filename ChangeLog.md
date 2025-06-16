># Update 3.2.1
>
>>## General
>>> - Added new custom weapon: **AdminAbuse ("Regert")**. Fires a frag grenade on shot and prevents attachment changes.
>>> - Introduced new debug/admin commands for C4 detonation and plugin debugging.
>>> - Improved error handling and logging, including more detailed error messages with timestamps and time zone info.
>
>>## SCP-1162
>>> - Removed the `SCP-1162` item from the game, as it was moved to a dedicated plugin.
>
>>## Sniper
>>> - Prevented attachment changes on the Sniper (SR-118) and provided user feedback.
>>> - Ensured the Sniper's magazine and barrel are emptied after each shot.
>
>>## Utils
>>> - Added and improved utility methods for item handling, color conversion, and candy logic.
>>> - Updated item name mappings and added new item types.
>>> - Improved documentation and code comments for maintainability.
>
>>## Internal
>>> - Refactored and organized command registration and permission checks for better maintainability.
>>> - Improved event subscription management to prevent duplicate event handler calls.