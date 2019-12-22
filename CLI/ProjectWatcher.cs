using System;
using System.IO;

namespace CLI
{
    public class ProjectFilesWatcher : IDisposable
    {
        public string BasePath { get; }
        private FileSystemWatcher? watcher;
        public ModuleStream ModuleStream { get; } = new ModuleStream();

        public ProjectFilesWatcher(string basePath)
        {
            this.BasePath = basePath;
        }

        public void Start()
        {
            // Create a new FileSystemWatcher and set its properties.
            this.watcher = new FileSystemWatcher
            {
                IncludeSubdirectories = true,
                Path = this.BasePath,

                // Watch for changes in LastAccess and LastWrite times, and
                // the renaming of files or directories.
                NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName,

                // Only watch text files.
                Filter = "*.car"
            };

            // Add event handlers.
            watcher.Changed += OnChanged;
            watcher.Created += OnCreate;
            watcher.Deleted += OnDelete;
            watcher.Renamed += OnRenamed;

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                ModuleStream.Publish(new ModuleStreamMessage(
                    Module.FromPathToName(e.FullPath, this.BasePath),
                    e.FullPath,
                    MessageType.ModuleChanged
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnCreate(object source, FileSystemEventArgs e)
        {
            try
            {
                ModuleStream.Publish(new ModuleStreamMessage(
                    Module.FromPathToName(e.FullPath, this.BasePath),
                    e.FullPath,
                    MessageType.ModuleCreated
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnDelete(object source, FileSystemEventArgs e)
        {
            try
            {
                ModuleStream.Publish(new ModuleStreamMessage(
                    Module.FromPathToName(e.FullPath, this.BasePath),
                    e.FullPath,
                    MessageType.ModuleDeleted
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            try
            {
                ModuleStream.Publish(new ModuleStreamMessage(
                    Module.FromPathToName(e.FullPath, this.BasePath),
                    e.FullPath,
                    Module.FromPathToName(e.OldFullPath, this.BasePath),
                    e.OldFullPath,
                    MessageType.ModuleMoved
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Dispose() {
            Console.WriteLine("Disposing of the Project Watcher");
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
        }
    }

}
