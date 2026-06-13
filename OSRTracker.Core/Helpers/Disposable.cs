using System;
using System.Collections.Generic;
using System.Text;

namespace OSRTracker.Helpers;

public static class Disposable
{
   public static IDisposable Empty { get; } = new EmptyDisposable();
}

internal class EmptyDisposable : IDisposable
{
   public void Dispose() { }
}