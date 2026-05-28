// Compiler shim required by C# 9+ records / init-only setters on
// netstandard2.1 (the BCL doesn't ship this type). Defining it here
// lets the compiler emit references to it. internal so it doesn't leak
// into the public API of Game.Core consumers.
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
