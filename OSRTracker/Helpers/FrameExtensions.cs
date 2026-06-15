using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Controls;

namespace OSRTracker.Helpers;

public static class FrameExtensions
{
    extension(Frame frame)
    {
        public object? GetPageViewModel() => frame?.Content?.GetType().GetProperty("ViewModel")?.GetValue(frame.Content, null);
    }
}
