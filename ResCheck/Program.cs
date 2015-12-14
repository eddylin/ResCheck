using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace ResCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"E:\x1\editor\mobile_reseditor\common_image";
            DirectoryInfo dir = new DirectoryInfo(path);

            var files = dir.GetFiles("*.png", SearchOption.AllDirectories);


            Dictionary<string, List<string>> list = new Dictionary<string, List<string>>();
            
            using (var md5 = MD5.Create())
            {
                foreach(var file in files)
                {
                    Console.WriteLine(file.FullName);
                    using (var stream = File.OpenRead(file.FullName))
                    {
                        var buffer = md5.ComputeHash(stream);
                        var sb = new StringBuilder();
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            sb.Append(buffer[i].ToString("x2"));
                        }
                        string key = sb.ToString();

                        if(list.Keys.Contains(key))
                        {
                            list[key].Add(file.FullName);
                        }
                        else
                        {
                            list[key] = new List<string>();
                            list[key].Add(file.FullName);
                        }
                    }
                }
            }

            Dictionary<string, List<string>> sameList = new Dictionary<string, List<string>>();

            foreach(var item in list)
            {
                if(item.Value.Count > 1)
                {
                    sameList.Add(item.Key, item.Value);
                }
            }

            string output_file = @"E:\x1\editor\mobile_reseditor\common_image\res_check.txt";

            FileInfo file_info = new FileInfo(output_file);
            if(file_info.Exists)
            {
                file_info.Delete();
            }
            file_info.Create().Close();

            using (Stream s = file_info.OpenWrite())
            {
                using (StreamWriter sb = new StreamWriter(s))
                {
                    foreach (var item in sameList)
                    {
                        sb.WriteLine(item.Key);
                        foreach (var file in item.Value)
                        {
                            sb.WriteLine("    " + file);
                        }
                    }
                }
            }
        }
    }
}
