using System;
namespace CLI
{
    public class ModuleStreamMessage
    {
        public string ModuleName { get; }
        public string FileFullPath { get; }
        public MessageType MessageType { get; }
        public string? OldModuleName { get; }
        public string? OldFileFullPath { get; }

        public ModuleStreamMessage(string moduleName, string fileFullpath, MessageType messageType)
        {
            this.ModuleName = moduleName;
            this.FileFullPath = fileFullpath;
            this.MessageType = messageType;
        }
        public ModuleStreamMessage(string moduleName, string fileFullpath, string oldModuleName, string oldFileFullPath, MessageType messageType)
        {
            this.ModuleName = moduleName;
            this.FileFullPath = fileFullpath;
            this.OldModuleName = oldModuleName;
            this.OldFileFullPath = oldFileFullPath;
            this.MessageType = messageType;
        }


        public override string ToString()
        {
            if (OldModuleName is null)
            {
                return $"{MessageType.ToString("g")}: {ModuleName}";
            }
            else
            {
                return $"{MessageType.ToString("g")}: from {OldModuleName}, to {ModuleName}";
            }
        }
    }

    public enum MessageType
    {
        ModuleCreated,
        ModuleChanged,
        ModuleDeleted,
        ModuleMoved
    }
}
