
namespace TestApplication
{


    public static class SimpleFileSystemHelper
    {

        public static void DirectoryCopy(this System.IO.Abstractions.IFileSystem fileSystem, string sourceDirName, string destDirName)
        {
            DirectoryCopy(fileSystem, sourceDirName, destDirName, true);
        }


        public static void DirectorySetAttribute(this System.IO.Abstractions.IFileSystem fileSystem, string relativeDirectoryPath, System.IO.FileAttributes attrib)
        {
            System.IO.Abstractions.IDirectoryInfo di = fileSystem.DirectoryInfo
                .FromDirectoryName(relativeDirectoryPath);

            di.Attributes |= attrib;
        }


        public static void DirectoryCopy(this System.IO.Abstractions.IFileSystem fileSystem, string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            System.IO.Abstractions.IDirectoryInfo dir = fileSystem.DirectoryInfo.FromDirectoryName(sourceDirName);


            if (!dir.Exists)
            {
                throw new System.IO.DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            System.IO.Abstractions.IDirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            if (!fileSystem.Directory.Exists(destDirName))
            {
                fileSystem.Directory.CreateDirectory(destDirName);
            }




            // Get the files in the directory and copy them to the new location.
            System.IO.Abstractions.IFileInfo[] files = dir.GetFiles();
            foreach (System.IO.Abstractions.IFileInfo file in files)
            {
                string temppath = fileSystem.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (System.IO.Abstractions.IDirectoryInfo subdir in dirs)
                {
                    string temppath = fileSystem.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(fileSystem, subdir.FullName, temppath, copySubDirs);
                }
            }
        }


    }


    class SimpleFileHelper
    {

        public static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            DirectoryCopy(sourceDirName, destDirName, true);
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new System.IO.DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            System.IO.DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!System.IO.Directory.Exists(destDirName))
            {
                System.IO.Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            System.IO.FileInfo[] files = dir.GetFiles();
            foreach (System.IO.FileInfo file in files)
            {
                string temppath = System.IO.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (System.IO.DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }


    }
}
