using System;
using System.Linq;
using CLI.Signals;

namespace CLI
{
    public partial class Project
    {
        public void Watch()
        {
            var projectWatcher = new ProjectFilesWatcher(this.BasePath);
            projectWatcher.Start();
            SignalSingleton.ExitSignal.Subscribe(() =>
            {
                projectWatcher.Dispose();
            });

            projectWatcher.ModuleStream.Subscribe("Logger", msm =>
            {
                Console.WriteLine($"{msm}");
            });

            projectWatcher.ModuleStream.Subscribe("OnChange", MessageType.ModuleChanged, msm =>
            {
                var module = this.Modules.FirstOrDefault(m => m.Name == msm.ModuleName);
                if (module != null)
                {
                    module.Parse();
                    module.SaveModuleOutput(false);
                }
            });

            projectWatcher.ModuleStream.Subscribe("OnCreate", MessageType.ModuleCreated, msm =>
            {
                var module = Modules.FirstOrDefault(m => m.Name == msm.ModuleName);
                if (module is null)
                {
                    module = new Module(msm.FileFullPath, this.BasePath, this);
                    Modules.Add(module);
                }
            });

            projectWatcher.ModuleStream.Subscribe("OnDelete", MessageType.ModuleDeleted, msm =>
            {
                var module = Modules.FirstOrDefault(m => m.Name == msm.ModuleName);
                if (!(module is null))
                {
                    module.Clean();
                    Modules.Remove(module);
                }
            });

            projectWatcher.ModuleStream.Subscribe("OnMove", MessageType.ModuleMoved, msm =>
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
