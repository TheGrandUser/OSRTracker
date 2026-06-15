using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Windows.ApplicationModel.Resources;

namespace OSRTracker.Helpers;

public static class ResourceExtensions
{
    private static readonly ResourceLoader _resourceLoader = new();

    extension(string resourceKey)
    {
        public string GetLocalized() => _resourceLoader.GetString(resourceKey);
    }
}
