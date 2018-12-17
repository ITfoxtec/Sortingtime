using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace Sortingtime.Infrastructure
{
    public static class IHtmlHelperExtensions
    {
        public static string Fingerprint<T>(this IHtmlHelper<T> htmlHelper, string rootRelativePath)
        {
            //var lastModified = Startup.Environment.WebRootFileProvider.GetFileInfo(rootRelativePath).LastModified;

            //var index = rootRelativePath.LastIndexOf("/");
            //return rootRelativePath.Insert(index + 1, $"v-{lastModified.ToString("yyMMddHHmm")}-");
            return rootRelativePath;
        }
    }
}
