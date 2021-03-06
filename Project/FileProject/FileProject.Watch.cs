﻿using Project.FileProject;
using System;
using System.Linq;

namespace Project.File
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

            ProjectWatcher.ModuleStream.Subscribe("OnChange", MessageType.ModuleChanged, async msm =>
            {
                var module = this.Modules.FirstOrDefault(m => m.Name == msm.ModuleName);
                if (module is null)
                {
                    module = new FileModule(msm.FileFullPath, this.BasePath, this);
                    Modules.Add(module);
                }
                
                module.Parse();
                await module.SaveModuleOutput(false);
            });

            //ProjectWatcher.ModuleStream.Subscribe("OnCreate", MessageType.ModuleCreated, msm =>
            //{
            //    var module = Modules.FirstOrDefault(m => m.Name == msm.ModuleName);
            //    if (module is null)
            //    {
            //        module = new Module(msm.FileFullPath, this.BasePath, this);
            //        Modules.Add(module);
            //    }
            //});

            ProjectWatcher.ModuleStream.Subscribe("OnDelete", MessageType.ModuleDeleted, async msm =>
            {
                var module = Modules.FirstOrDefault(m => m.Name == msm.ModuleName);
                if (!(module is null))
                {
                    Modules.Remove(module);
                    await module.Clean();
                }
            });

            ProjectWatcher.ModuleStream.Subscribe("OnRename", MessageType.ModuleRenamed, async msm =>
            {
                var moduleOld = Modules.FirstOrDefault(m => m.Name == msm.ModuleName);
                if (!(moduleOld is null))
                {
                    await moduleOld.Clean();
                    Modules.Remove(moduleOld);
                }
                var module = new FileModule(msm.FileFullPath, this.BasePath, this);
                Modules.Add(module);
                module.Parse();
                await module.SaveModuleOutput();
            });
        }
    }
}
