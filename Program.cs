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
        static readonly (string, string)[] ModResources = [.. Directory.GetFiles(ModResourcesFolder).Select(file => (Name: Path.GetFileName(file), Path: file))];
        static readonly string[] ResourceNames = [.. ModResources.Select(file => file.Item1)];
        static readonly Dictionary<string, string> GameResources = GetResources(GameResourcesFolder);

        public static void Run()
        {
            GameResources.ToList().ForEach(file =>
            {
                string copyTo = Path.Combine(BackupResourcesFolder, file.Key);
                Console.WriteLine($"Backuping up {file.Key} to {copyTo}!");
                File.Copy(file.Value, copyTo, true);
            });

            ModResources.ToList().ForEach(file =>
            {
                string copyTo = GameResources[file.Item1];
                Console.WriteLine($"Copying {file.Item1} to {copyTo}!");
                File.Copy(file.Item2, copyTo, true);
            });
        }
        static Dictionary<string, string> GetResources(string path)
        {
            Dictionary<string, string> resources = new(ResourceNames.Length);
            foreach (var file in Directory.EnumerateFiles(path, "*.assets", SearchOption.TopDirectoryOnly))
            {
                string name = Path.GetFileName(file);
                if (ResourceNames.Contains(name))
                {
                    resources[name] = file;
                }
            }
            return resources;
        }
    }
}