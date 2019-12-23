using System;
using System.Linq;

namespace Project
{
    public partial class FileProject
    {
        internal ProjectFilesWatcher? ProjectWatcher { get; private set; }
        public void Watch()
        {
            ProjectWatcher = new ProjectFilesWatcher(this.BasePath);
            ProjectWatcher.Start();
            
            ProjectWatcher.ModuleStream.Subscribe("Logger", msm =>
            {
                //Console.WriteLine($"{msm}");
            });

            ProjectWatcher.ModuleStream.Subscribe("OnChange", MessageType.ModuleChanged, msm =>
            {
                var module = this.Modules.FirstOrDefault(m => m.Name == msm.ModuleName);
                if (module != null)
                {
                    module.Parse();
                    module.SaveModuleOutput(false);
                }
            });

            ProjectWatcher.ModuleStream.Subscribe("OnCreate", MessageType.ModuleCreated, msm =>
            {
                var module = Modules.FirstOrDefault(m => m.Name == msm.ModuleName);
                if (module is null)
                {
                    module = new Module(msm.FileFullPath, this.BasePath, this);
                    Modules.Add(module);
                }
            });

            ProjectWatcher.ModuleStream.Subscribe("OnDelete", MessageType.ModuleDeleted, msm =>
            {
                var module = Modules.FirstOrDefault(m => m.Name == msm.ModuleName);
                if (!(module is null))
                {
                    module.Clean();
                    Modules.Remove(module);
                }
            });

            ProjectWatcher.ModuleStream.Subscribe("OnRename", MessageType.ModuleRenamed, msm =>
            {
                var moduleOld = Modules.FirstOrDefault(m => m.Name == msm.ModuleName);
                if (!(moduleOld is null))
                {
                    moduleOld.Clean();
                    Modules.Remove(moduleOld);
                }
                var module = new Module(msm.FileFullPath, this.BasePath, this);
                Modules.Add(module);
                module.Parse();
                module.SaveModuleOutput();
            });
        }
    }
}
