using System.Collections.ObjectModel;
using System.Reflection;
using BonesClassLibrary.IO;
using BonesClassLibrary;
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
        static readonly string[] Options = ["BACKUP", "RESTORE FROM BACKUP", "INSTALL"];
        public static void Run()
        {
            int option = ConsoleHelper.Choices(Options, "This program is for backing up Vietnam War assets and installing mod assets in their place.");
            if (option == 0)
            {
                if (Backup() == false)
                    Console.WriteLine("Already backed up!");
            }
            else if (option == 1)
            {
                Console.WriteLine("Restoring from backup...");
                Restore();
            }
            else if (option == 2)
            {
                Backup();
                Console.WriteLine("Installing...");
                Copy(ModResources, key => GameResources[key]);
            }
        }

        public static void Restore()
        {
            if (!Directory.EnumerateFiles(BackupResourcesFolder).Any())
                throw new FileNotFoundException("Backup resources folder is empty.");
            var backups = GetResources(BackupResourcesFolder);
            Copy(backups, key => Path.Combine(GameResourcesFolder, key));
        }

        static bool Backup()
        {
            if (!Directory.EnumerateFiles(BackupResourcesFolder).Any())
            {
                Console.WriteLine("Backing up...");
                Copy(GameResources, key => Path.Combine(BackupResourcesFolder, key));
                return true;
            }
            return false;

        }

        //here, dictionary obj is the resources you are copying from
        //expr is a func that creates the copy target
        //typically it is going to be a Path.Combine with your target folder path + the key of the current object selected in the dictionary
        //(which will be a filename key w/o a path)
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