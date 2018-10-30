using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nebula.SDK.Util
{
    public class FileUtil
    {
        /// <summary>
        /// Copy one directory to another
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="targetDirectory"></param>
        public static void Copy(string sourceDirectory, string targetDirectory)
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
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="allFiles"></param>
        /// <param name="ext"></param>
        /// <param name="replaceFunc"></param>
        public static void GenerateFileList(string folder, List<string> allFiles, string ext = ".neb", Func<string, string> replaceFunc = null)
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
    }
}