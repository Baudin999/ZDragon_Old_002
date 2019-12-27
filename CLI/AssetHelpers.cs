using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace CLI
{
    internal static class AssetHelpers
    {
        internal static string ReadAsset(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "CLI.Assets." + name;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (!(stream is null))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var result = reader.ReadToEnd();
                        reader.Close();
                        reader.Dispose();
                        return result;
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        internal static void WriteAsset(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        internal static void ReadAndWriteAsset(string assetName, string outPath)
        {
            var outName = System.IO.Path.GetFullPath(assetName, outPath);
            AssetHelpers.WriteAsset(outName, AssetHelpers.ReadAsset(assetName));
        }
    }
}
