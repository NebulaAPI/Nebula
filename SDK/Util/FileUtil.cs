using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nebula.SDK.Util
{
    public interface IFileUtil
    {
        void Copy(string sourceDirectory, string targetDirectory);
        void CopyAll(DirectoryInfo source, DirectoryInfo target);
        void GenerateFileList(string folder, List<string> allFiles, string ext = ".neb", Func<string, string> replaceFunc = null);
        bool DirectoryExists(string directory);
        void DirectoryDelete(string directory, bool recursive = false);
        string[] DirectoryGetFiles(string directory);
        IEnumerable<string> FileReadLines(string filePath);
        string FileReadAllText(string filePath);
        void FileWriteAllLines(string filePath, IEnumerable<string> lines);
        bool FileExists(string filePath);
        void FileWriteAllText(string filePath, string content);
        void FileMove(string source, string dest);
        FileStream FileCreate(string filePath);
        string[] GetDirectories(string directory);
    }
    
    public class FileUtil : IFileUtil
    {
        /// <summary>
        /// Copy one directory to another
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="targetDirectory"></param>
        public void Copy(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        
        /// <summary>
        /// Copy all files and sub-directories from source to target
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (var fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public void DirectoryDelete(string directory, bool recursive = false)
        {
            Directory.Delete(directory, recursive);
        }

        public bool DirectoryExists(string directory)
        {
            return Directory.Exists(directory);
        }

        public string[] DirectoryGetFiles(string directory)
        {
            return Directory.GetFiles(directory);
        }

        public FileStream FileCreate(string filePath)
        {
            return File.Create(filePath);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public void FileMove(string source, string dest)
        {
            File.Move(source, dest);
        }

        public string FileReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public IEnumerable<string> FileReadLines(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        public void FileWriteAllLines(string filePath, IEnumerable<string> lines)
        {
            File.WriteAllLines(filePath, lines);
        }

        public void FileWriteAllText(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="allFiles"></param>
        /// <param name="ext"></param>
        /// <param name="replaceFunc"></param>
        public void GenerateFileList(string folder, List<string> allFiles, string ext = ".neb", Func<string, string> replaceFunc = null)
        {
            var filesInThisFolder = Directory
                .GetFiles(folder)
                .Select(f => replaceFunc(f))//f.Replace(CurrentProject.SourceDirectory + Path.DirectorySeparatorChar, ""))
                .Where(f => f.EndsWith(ext));
            allFiles.AddRange(filesInThisFolder);
            var folders = Directory.GetDirectories(folder);
            foreach (var f in folders)
            {
                GenerateFileList(f, allFiles, ext, replaceFunc);
            }
        }

        public string[] GetDirectories(string directory)
        {
            return Directory.GetDirectories(directory);
        }
    }
}