using System;
using System.Collections.Generic;
using System.IO;
using LogExpert.Classes.Log;

namespace LogExpert.Tests
{
    internal class RolloverHandlerTestBase
    {
        #region Fields

        protected const string TEST_DIR_NAME = "test";

        #endregion


        protected LinkedList<string> CreateTestFilesWithDate()
        {
            LinkedList<string> createdFiles = new();
            DirectoryInfo dInfo = Directory.CreateDirectory(TEST_DIR_NAME);
            createdFiles.AddLast(CreateFile(dInfo, "engine_2010-06-08_1.log"));
            createdFiles.AddLast(CreateFile(dInfo, "engine_2010-06-08_0.log"));
            createdFiles.AddLast(CreateFile(dInfo, "engine_2010-06-10_0.log"));
            createdFiles.AddLast(CreateFile(dInfo, "engine_2010-06-11_1.log"));
            createdFiles.AddLast(CreateFile(dInfo, "engine_2010-06-11_0.log"));
            createdFiles.AddLast(CreateFile(dInfo, "engine_2010-06-12_2.log"));
            createdFiles.AddLast(CreateFile(dInfo, "engine_2010-06-12_1.log"));
            createdFiles.AddLast(CreateFile(dInfo, "engine_2010-06-12_0.log"));
            return createdFiles;
        }

        protected LinkedList<string> CreateTestFilesWithoutDate()
        {
            LinkedList<string> createdFiles = new();
            DirectoryInfo dInfo = Directory.CreateDirectory(TEST_DIR_NAME);
            createdFiles.AddLast(CreateFile(dInfo, "engine.log.6"));
            createdFiles.AddLast(CreateFile(dInfo, "engine.log.5"));
            createdFiles.AddLast(CreateFile(dInfo, "engine.log.4"));
            createdFiles.AddLast(CreateFile(dInfo, "engine.log.3"));
            createdFiles.AddLast(CreateFile(dInfo, "engine.log.2"));
            createdFiles.AddLast(CreateFile(dInfo, "engine.log.1"));
            createdFiles.AddLast(CreateFile(dInfo, "engine.log"));
            return createdFiles;
        }

        protected LinkedList<string> RolloverSimulation(LinkedList<string> files, string formatPattern,
            bool deleteLatestFile)
        {
            LinkedList<string> fileList = files;
            RolloverFilenameBuilder fnb = new(formatPattern);
            fnb.SetFileName(fileList.Last.Value);
            fnb.Index += fileList.Count;
            string newFileName = fnb.BuildFileName();
            fileList.AddFirst(newFileName);
            LinkedList<string>.Enumerator enumerator = fileList.GetEnumerator();
            LinkedList<string>.Enumerator nextEnumerator = fileList.GetEnumerator();
            nextEnumerator.MoveNext(); // move on 2nd entry
            enumerator.MoveNext();
            while (nextEnumerator.MoveNext())
            {
                File.Move(nextEnumerator.Current, enumerator.Current);
                enumerator.MoveNext();
            }
            CreateFile(null, nextEnumerator.Current);

            if (deleteLatestFile)
            {
                File.Delete(fileList.First.Value);
                fileList.RemoveFirst();
            }
            return fileList;
        }


        protected void Cleanup()
        {
            try
            {
                Directory.Delete(TEST_DIR_NAME, true);
            }
            catch (Exception)
            {
            }
        }

        protected string CreateFile(DirectoryInfo dInfo, string fileName)
        {
            int lineCount = 10;
            string fullName = dInfo == null ? fileName : dInfo.FullName + Path.DirectorySeparatorChar + fileName;

            using (StreamWriter writer = new(File.Create(fullName)))
            {
                for (int i = 1; i <= lineCount; ++i)
                {
                    writer.WriteLine("Line number " + i.ToString("D3") + " of File " + fullName);
                }

                writer.Flush();
            }

            return fullName;
        }
    }
}