using System.Collections.ObjectModel;
using System.Reflection;
using BonesClassLibrary.FileFinders;
using ResourcesGetter;


ResourcesReplacer.Run();


namespace ResourcesGetter
{

    public static class ResourcesReplacer
    {
        static readonly string GameResourcesFolder = Path.Combine(VietnamWarSource.Path, "Vietnam War_Data");
        static readonly string BackupResourcesFolder = Path.Combine(VietnamWarModLab.Path, @"ResourcesGetter\BackupResources");
        static readonly string ModResourcesFolder = Path.Combine(VietnamWarModLab.Path, @"ResourcesGetter\ModResources");
        static readonly Dictionary<string, string> ModResources = Directory.GetFiles(ModResourcesFolder).Select(file => (Name: Path.GetFileName(file), Path: file)).ToDictionary();
        static readonly Dictionary<string, string> GameResources = GetResources(GameResourcesFolder);
        public static void Run()
        {
            if (!Directory.EnumerateFiles(BackupResourcesFolder).Any())
                Copy(GameResources, key => Path.Combine(BackupResourcesFolder, key));
            Copy(ModResources, key => GameResources[key]);
        }

        static void Copy(Dictionary<string, string> obj, Func<string, string> expr)
        {
            obj.ToList().ForEach(x =>
            {
                string copyTo = expr(x.Key);
                Console.WriteLine($"Copying {x.Key} from {x.Value} to {copyTo}!");
                File.Copy(x.Value, copyTo, true);
            });
        }
        static Dictionary<string, string> GetResources(string path)
        {
            string[] names = [.. ModResources.Keys];
            Dictionary<string, string> resources = new(names.Length);
            foreach (var file in Directory.EnumerateFiles(path, "*.assets", SearchOption.TopDirectoryOnly))
            {
                string name = Path.GetFileName(file);
                if (names.Contains(name))
                {
                    resources[name] = file;
                }
            }
            return resources;
        }
    }
}