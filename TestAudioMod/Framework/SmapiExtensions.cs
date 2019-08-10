using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Ogg2XNA;
using StardewModdingAPI;
using StardewValley;

namespace Pathoschild.Stardew.TestAudioMod.Framework
{
    /// <summary>Provides extensions on the SMAPI API.</summary>
    internal static class SmapiExtensions
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Load content from SMAPI's content API with added support for audio files.</summary>
        /// <typeparam name="T">The expected data type.</typeparam>
        /// <param name="content">The content manager to extend.</param>
        /// <param name="key">The asset key to fetch (if the <paramref name="source"/> is <see cref="ContentSource.GameContent"/>), or the local path to a content file relative to the mod folder.</param>
        /// <param name="source">Where to search for a matching content asset.</param>
        /// <remarks>Since this is just a quick prototype, this doesn't handle caching and such for audio files.</remarks>
        public static T ExtendedLoad<T>(this IContentHelper content, string key, ContentSource source = ContentSource.ModFolder)
        {
            // ignore non-audio files
            if (source != ContentSource.ModFolder || key?.Trim().EndsWith(".ogg", StringComparison.InvariantCultureIgnoreCase) != true)
                return content.Load<T>(key, source);

            // get mod file
            key = content.NormalizeAssetName(key);
            FileInfo file = content.GetPrivateField<object>("ModContentManager").InvokePrivateMethod<FileInfo>("GetModFile", key);
            if (!file.Exists)
                return content.Load<T>(key, source);

            // load unpacked audio file
            if (typeof(T) != typeof(IModCue) && typeof(T) != typeof(ICue))
                throw new ContentLoadException($"Failed loading asset '{key}' from content manager: can't read file with extension '{file.Extension}' as type '{typeof(T)}'; must be type '{typeof(ICue)}' or '{typeof(IModCue)}'.");
            SoundEffect effect = OggLoader.Load(file.FullName);
            return (T)(object)new SoundEffectCue(key, effect);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Use reflection to get the value of a private field on an instance.</summary>
        /// <typeparam name="T">The expected field value.</typeparam>
        /// <param name="parent">The object to extend.</param>
        /// <param name="name">The field name.</param>
        private static T GetPrivateField<T>(this object parent, string name)
        {
            Type type = parent.GetType();

            FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
                throw new InvalidOperationException($"Can't find field {type.FullName}.{name}.");

            object result = field.GetValue(parent);
            try
            {
                return (T)result;
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException($"Can't cast value of field {type.FullName}.{name} from {result?.GetType().FullName ?? "null"} to {typeof(T).FullName}.");
            }
        }

        /// <summary>Use reflection to invoke a private method on an instance.</summary>
        /// <typeparam name="T">The expected return value.</typeparam>
        /// <param name="parent">The object to extend.</param>
        /// <param name="name">The method name.</param>
        /// <param name="arguments">The method arguments.</param>
        private static T InvokePrivateMethod<T>(this object parent, string name, params object[] arguments)
        {
            Type type = parent.GetType();
            MethodInfo method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if (method == null)
                throw new InvalidOperationException($"Can't find method {type.FullName}.{name}.");

            object result = method.Invoke(parent, arguments);
            try
            {
                return (T)result;
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException($"Can't cast result of method {type.FullName}.{name} from {result?.GetType().FullName ?? "null"} to {typeof(T).FullName}.");
            }
        }
    }
}
