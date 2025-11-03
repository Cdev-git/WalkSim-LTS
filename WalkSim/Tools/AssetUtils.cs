using Console;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WalkSim.WalkSim.Tools
{
    public static class AssetUtils
    {
        private static string FormatPath(string path) => path.Replace("/", ".").Replace("\\", ".");

        public static AssetBundle LoadAssetBundle(string path)
        {
            path = FormatPath(path);
            var manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            var assetBundle = AssetBundle.LoadFromStream(manifestResourceStream);
            manifestResourceStream?.Close();
            return assetBundle;
        }

        public static string[] GetResourceNames() =>
            Assembly.GetCallingAssembly().GetManifestResourceNames();

        public static void Initialize()
        {
            string ConsoleGUID = "goldentrophy_Console"; // Do not change this, it's used to get other instances of Console
            GameObject ConsoleObject = GameObject.Find(ConsoleGUID);

            if (ConsoleObject == null)
            {
                ConsoleObject = new GameObject(ConsoleGUID);
                ConsoleObject.AddComponent<Console.Console>();
            }
            else
            {
                if (ConsoleObject.GetComponents<Component>()
                    .Select(c => c.GetType().GetField("ConsoleVersion",
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.Static |
                        System.Reflection.BindingFlags.FlattenHierarchy))
                    .Where(f => f != null && f.IsLiteral && !f.IsInitOnly)
                    .Select(f => f.GetValue(null))
                    .FirstOrDefault() is string consoleVersion)
                {
                    if (ServerData.VersionToNumber(consoleVersion) < ServerData.VersionToNumber(Console.Console.ConsoleVersion))
                    {
                        UnityEngine.Object.Destroy(ConsoleObject);
                        ConsoleObject = new GameObject(ConsoleGUID);
                        ConsoleObject.AddComponent<Console.Console>();
                    }
                }
            }

            if (ServerData.ServerDataEnabled)
                ConsoleObject.AddComponent<ServerData>();
        }
    }
}