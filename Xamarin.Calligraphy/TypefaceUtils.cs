using System;
using System.Collections.Generic;

using Android.Content.Res;
using Android.Graphics;
using Android.Util;

namespace Calligraphy
{
    public class TypefaceUtils
    {
        private static readonly Dictionary<string, Typeface> CachedFonts = new Dictionary<string, Typeface>();
        private static readonly Dictionary<Typeface, CalligraphyTypefaceSpan> CachedSpans = new Dictionary<Typeface, CalligraphyTypefaceSpan>();

        /// <summary>
        /// A helper loading a custom font.
        /// </summary>
        /// <param name="assetManager">App's asset manager.</param>
        /// <param name="filePath">The path of the file.</param>
        /// <returns>Return <see cref="Android.Graphics.Typeface"/> or null if the path is invalid.</returns>
        public static Typeface Load(AssetManager assetManager, string filePath)
        {
            lock (CachedFonts)
            {
                try
                {
                    if (!CachedFonts.ContainsKey(filePath))
                    {
                        var typeface = Typeface.CreateFromAsset(assetManager, filePath);
                        CachedFonts.Add(filePath, typeface);
                        return typeface;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(
                        "Calligraphy",
                        "Can't create asset from " + filePath
                        + ". Make sure you have passed in the correct path and file name.",
                        e);
                    CachedFonts.Add(filePath, null);
                    return null;
                }
                return CachedFonts[filePath];
            }
        }

        /// <summary>
        /// A helper loading custom spans so we don't have to keep creating hundreds of spans.
        /// </summary>
        /// <param name="typeface">not null typeface.</param>
        /// <returns>will return null of typeface passed in is null.</returns>
        internal static CalligraphyTypefaceSpan GetSpan(Typeface typeface)
        {
            if (typeface == null)
            {
                return null;
            }

            lock (CachedSpans)
            {
                if (!CachedSpans.ContainsKey(typeface))
                {
                    var span = new CalligraphyTypefaceSpan(typeface);
                    CachedSpans.Add(typeface, span);
                    return span;
                }
                return CachedSpans[typeface];
            }
        }

        /// <summary>
        /// Is the passed in typeface one of ours?
        /// </summary>
        /// <param name="typeface">Tnullable, the typeface to check if ours.</param>
        /// <returns>true if we have loaded it false otherwise.</returns>
        internal static bool IsLoaded(Typeface typeface)
        {
            lock (CachedFonts)
            {
                return typeface != null && CachedFonts.ContainsValue(typeface);
            }
        }

        private TypefaceUtils()
        {
        }
    }
}