using Project.File;
using Project.FileSystems;

namespace Project
{
    public class ProjectContext
    {
        public static IProject? Instance { get; private set; }
        public static IFileSystem? FileSystem { get; private set; }

        public static IProject Init(string dir)
        {
            FileSystem = new FileSystem();
            Instance = new Project.File.FileProject(dir);
            return Instance;
        }

        public static IProject InitInMemory()
        {
            FileSystem = new MemorySystem();
            Instance = new Project.MemoryProject.MemoryProject();
            return Instance;
        }

        public static void Destruct()
        {
            Instance = null;
        }
    }
}
