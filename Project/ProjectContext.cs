using System;
namespace Project
{
    public class ProjectContext
    {
        public static IProject? Instance { get; private set; }

        public static IProject Init(string dir)
        {
            Instance = new FileProject(dir);
            return Instance;
        }

        public static void Destruct()
        {
            Instance = null;
        }
    }
}
