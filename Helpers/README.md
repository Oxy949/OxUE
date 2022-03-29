# CursorHandler
An advanced cursor behavior using state precedence.

## Scripts:
- [Helpers](Scripts/Helpers.cs) - Static library with a bunch of useful functions.
- [IInitableSystem](Scripts/IInitableSystem.cs) - Use it for initialization from GameManager script, when order do matter.
- [ObjectCopier](Scripts/ObjectCopier.cs) - Provides a method for performing a deep copy of an object.
- [Singleton](Scripts/Singleton.cs) - Singleton class. Inherit by passing the inherited type (e.g. class GameManager : Singleton&lt;GameManager&gt;.
