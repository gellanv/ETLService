using System.Text;

namespace TaskOne
{
    public class DirectoryWorkClass
    {
        public string SourceFolder { get; set; }
        public string ResultFolder { get; set; }

        public IEnumerable<List<string>> ReadFile(string path)
        {
            List<string> Lines = new List<string>();

            using (StreamReader stream = new StreamReader(path))
            {
                int count = 0;
                while (stream.EndOfStream != true)
                {
                    string line = stream.ReadLine();
                    if (line != null && line.Contains("first_name:"))
                        continue;
                    else
                    {
                        Lines.Add(line);
                        count++;
                    }
                    if (count == 100)
                    {
                        count = 0;
                        yield return Lines;
                        Lines.Clear();
                    }
                }
                if (count > 0 && count <= 100)
                {
                    yield return Lines;
                }
            }
        }

        public string CreateResultFileName()
        {
            string path = CreateFolderIfNotExist();
            int countExistFiles = Directory.GetFiles(path).Length;
            path = $"{path}\\output{(++countExistFiles).ToString()}.json";

            return path;
        }

        public void WriteDataToFile(string pathOldFile, string pathNewFile, string jsonString)
        {
            try
            {
                using (FileStream fs = File.Create(pathNewFile))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(jsonString);
                    fs.Write(info, 0, info.Length);
                }
                FileInfo fileInfo = new FileInfo(pathOldFile);
                fileInfo.Delete();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void WriteMetaDataToFile(MetaData metaData)
        {
            string path = CreateFolderIfNotExist();
            string pathMetaFile = path + "\\meta.log";
            try
            {
                using (FileStream fs = File.Create(pathMetaFile))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(metaData.ToString());
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private string CreateFolderIfNotExist()
        {
            DateTime dateTime = DateTime.Now;
            string nameFolder = $"{dateTime.Month}-{dateTime.Day}-{dateTime.Year}";
            string path = this.ResultFolder + "\\" + nameFolder;

            var directory = new DirectoryInfo(path);
            if (!directory.Exists)
            {
                directory.Create();
            }
            return path;
        }
    }
}
