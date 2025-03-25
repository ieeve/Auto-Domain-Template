using System;
using System.Text;

namespace Infrastructure.Common.Utility
{
    public class FileHelper : IDisposable
    {
        private bool _alreadyDispose = false;

        #region 构造函数
        public FileHelper()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        ~FileHelper()
        {
            Dispose(); ;
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_alreadyDispose) return;
            _alreadyDispose = true;
        }
        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region 取得文件后缀名
        /****************************************
          * 函数名称：GetPostfixStr
          * 功能说明：取得文件后缀名
          * 参     数：filename:文件名称
          * 调用示列：
          *            string filename = "aaa.aspx";        
          *            string s = EC.FileObj.GetPostfixStr(filename);         
         *****************************************/
        /// <summary>
        /// 取后缀名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>.gif|.html格式</returns>
        public static string GetPostfixStr(string filename)
        {
            int start = filename.LastIndexOf(".", StringComparison.Ordinal);
            int length = filename.Length;
            string postfix = filename.Substring(start, length - start);
            return postfix;
        }
        #endregion

        #region 写文件
        /****************************************
          * 函数名称：WriteFile
          * 功能说明：写文件,会覆盖掉以前的内容
          * 参     数：Path:文件路径,Strings:文本内容
          * 调用示列：
          *            string Path = Server.MapPath("Default2.aspx");       
          *            string Strings = "这是我写的内容啊";
          *            EC.FileObj.WriteFile(Path,Strings);
         *****************************************/
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="Strings">文件内容</param>
        public static void WriteFile(string Path, string Strings)
        {
            if (!System.IO.File.Exists(Path))
            {
                System.IO.FileStream f = System.IO.File.Create(Path);
                f.Close();
            }
            //System.IO.StreamWriter f2 = new System.IO.StreamWriter(Path, false, System.Text.Encoding.GetEncoding("gb2312"));
            //Encoding.Default 为默认编码。如果编码混乱，使用 GetEncoding(filePath)，获取当前文件编码。
            System.IO.StreamWriter f2 = new System.IO.StreamWriter(Path, false, Encoding.Default);
            f2.Write(Strings);
            f2.Close();
            f2.Dispose();
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="Strings">文件内容</param>
        /// <param name="encode">编码格式</param>
        public static void WriteFile(string Path, string Strings, Encoding encode)
        {
            if (!System.IO.File.Exists(Path))
            {
                System.IO.FileStream f = System.IO.File.Create(Path);
                f.Close();
            }
            System.IO.StreamWriter f2 = new System.IO.StreamWriter(Path, false, encode);
            f2.Write(Strings);
            f2.Close();
            f2.Dispose();
        }
        #endregion

        #region 读文件
        /****************************************
          * 函数名称：ReadFile
          * 功能说明：读取文本内容
          * 参     数：Path:文件路径
          * 调用示列：
          *            string Path = Server.MapPath("Default2.aspx");       
          *            string s = EC.FileObj.ReadFile(Path);
         *****************************************/
        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <returns></returns>
        public static string ReadFile(string Path)
        {
            string s = "";
            if (!System.IO.File.Exists(Path))
                s = "待读取的文件或目录不存在";
            else
            {
                //StreamReader f2 = new StreamReader(Path, System.Text.Encoding.GetEncoding("gb2312"));
                //Encoding.Default 为默认编码。如果编码混乱，使用 GetEncoding(filePath)，获取当前文件编码。
                StreamReader f2 = new StreamReader(Path, Encoding.Default);
                s = f2.ReadToEnd();
                f2.Close();
                f2.Dispose();
            }

            return s;
        }

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="encode">编码格式</param>
        /// <returns></returns>
        public static string ReadFile(string Path, Encoding encode)
        {
            string s = string.Empty;
            if (!System.IO.File.Exists(Path))
                s = "待读取的文件或目录不存在";
            else
            {
                StreamReader f2 = new StreamReader(Path, encode);
                s = f2.ReadToEnd();
                f2.Close();
                f2.Dispose();
            }
            return s;
        }
        /// <summary>
        /// 逐行读取文件到List
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static List<string> ReadFileToList(string FilePath)
        {
            //新建文件流
            FileStream fsRead;
            //用FileStream文件流打开文件
            try
            {
                fsRead = new FileStream(FilePath, FileMode.Open);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            //Encoding.Default 为默认编码。如果编码混乱，使用 GetEncoding(filePath)，获取当前文件编码。
            StreamReader reader = new StreamReader(fsRead, Encoding.Default);
            List<string> list = new List<string>();
            string search;
            while ((search = reader.ReadLine()) != null)
            {

                list.Add(search);
            }

            fsRead.Close();

            //去掉结尾的空格
            /* 交给用户自行处理
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace(list[i])) list.RemoveAt(i);
            else  break; //只移除文件结尾的空白行
            }
            */
            return list;
        }

        #endregion

        #region 追加文件
        /****************************************
          * 函数名称：FileAdd
          * 功能说明：追加文件内容
          * 参     数：Path:文件路径,strings:内容
          * 调用示列：
          *            string Path = Server.MapPath("Default2.aspx");     
          *            string Strings = "新追加内容";
          *            EC.FileObj.FileAdd(Path, Strings);
         *****************************************/
        /// <summary>
        /// 追加文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="strings">内容</param>
        public static void FileAdd(string Path, string strings)
        {
            StreamWriter sw = File.AppendText(Path);
            sw.Write(strings);
            sw.Flush();
            sw.Close();
        }
        #endregion

        #region 拷贝文件
        /****************************************
          * 函数名称：FileCoppy
          * 功能说明：拷贝文件
          * 参     数：OrignFile:原始文件,NewFile:新文件路径
          * 调用示列：
          *            string orignFile = Server.MapPath("Default2.aspx");     
          *            string NewFile = Server.MapPath("Default3.aspx");
          *            EC.FileObj.FileCoppy(OrignFile, NewFile);
         *****************************************/
        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="orignFile">原始文件</param>
        /// <param name="newFile">新文件路径</param>
        public static void FileCopy(string orignFile, string newFile)
        {
            File.Copy(orignFile, newFile, true);
        }
        #endregion

        #region 删除文件
        /****************************************
          * 函数名称：FileDel
          * 功能说明：删除文件
          * 参     数：Path:文件路径
          * 调用示列：
          *            string Path = Server.MapPath("Default3.aspx");    
          *            EC.FileObj.FileDel(Path);
         *****************************************/
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="Path">路径</param>
        public static void FileDel(string Path)
        {
            File.Delete(Path);
        }
        #endregion

        #region 移动文件
        /****************************************
          * 函数名称：FileMove
          * 功能说明：移动文件
          * 参     数：OrignFile:原始路径,NewFile:新文件路径
          * 调用示列：
          *             string orignFile = Server.MapPath("../说明.txt");    
          *             string NewFile = Server.MapPath("http://www.cnblogs.com/说明.txt");
          *             EC.FileObj.FileMove(OrignFile, NewFile);
         *****************************************/
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="orignFile">原始路径</param>
        /// <param name="newFile">新路径</param>
        public static void FileMove(string orignFile, string newFile)
        {
            File.Move(orignFile, newFile);
        }
        #endregion

        #region 在当前目录下创建目录
        /****************************************
          * 函数名称：FolderCreate
          * 功能说明：在当前目录下创建目录
          * 参     数：OrignFolder:当前目录,NewFloder:新目录
          * 调用示列：
          *            string orignFolder = Server.MapPath("test/");    
          *            string NewFloder = "new";
          *            EC.FileObj.FolderCreate(OrignFolder, NewFloder);
         *****************************************/
        /// <summary>
        /// 在当前目录下创建目录
        /// </summary>
        /// <param name="orignFolder">当前目录</param>
        /// <param name="newFloder">新目录</param>
        public static void FolderCreate(string orignFolder, string newFloder)
        {
            Directory.SetCurrentDirectory(orignFolder);
            Directory.CreateDirectory(newFloder);
        }
        #endregion

        #region 递归删除文件夹目录及文件
        /****************************************
          * 函数名称：DeleteFolder
          * 功能说明：递归删除文件夹目录及文件
          * 参     数：dir:文件夹路径
          * 调用示列：
          *            string dir = Server.MapPath("test/");  
          *            EC.FileObj.DeleteFolder(dir);       
         *****************************************/
        /// <summary>
        /// 递归删除文件夹目录及文件
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static void DeleteFolder(string dir)
        {
            if (Directory.Exists(dir)) //如果存在这个文件夹删除之
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                        File.Delete(d); //直接删除其中的文件
                    else
                        DeleteFolder(d); //递归删除子文件夹
                }
                Directory.Delete(dir); //删除已空文件夹
            }

        }
        #endregion

        /// <summary>
        /// 修改文件名
        /// </summary>
        /// <param name="srcFileName"></param>
        /// <param name="destFileName"></param>
        public static void RenameFileName(string srcFileName, string destFileName)
        {
            if (File.Exists(srcFileName))
            {
                File.Move(srcFileName, destFileName);
            }
        }
        /// <summary>
        /// 修改目录名
        /// </summary>
        /// <param name="srcDirName"></param>
        /// <param name="destDirName"></param>
        public static void RenameDirectory(string srcDirName, string destDirName)
        {
            if (Directory.Exists(srcDirName))
            {
                Directory.Move(srcDirName, destDirName);
            }
        }

        /// <summary>
        /// 所有目录名(不含子目录里面的目录）
        /// </summary>
        /// <param name="DirPath">父级目录</param>
        /// <param name="directoryList"></param>
        /// <returns></returns>
        public static List<DirectoryInfo> GetDirectoryList(string DirPath)
        {
            var directoryList = new List<DirectoryInfo>();
            //目录不存在
            if (!Directory.Exists(DirPath)) return directoryList;
            DirectoryInfo thisDirectory = new DirectoryInfo(DirPath);
            directoryList.AddRange(thisDirectory.GetDirectories());
            return directoryList;
        }

        #region 将指定文件夹下面的所有内容copy到目标文件夹下面 果目标文件夹为只读属性就会报错。
        /****************************************
          * 函数名称：CopyDir
          * 功能说明：将指定文件夹下面的所有内容copy到目标文件夹下面 果目标文件夹为只读属性就会报错。
          * 参     数：srcPath:原始路径,aimPath:目标文件夹
          * 调用示列：
          *            string srcPath = Server.MapPath("test/");  
          *            string aimPath = Server.MapPath("test1/");
          *            EC.FileObj.CopyDir(srcPath,aimPath);   
         *****************************************/
        /// <summary>
        /// 指定文件夹下面的所有内容copy到目标文件夹下面
        /// </summary>
        /// <param name="srcPath">原始路径</param>
        /// <param name="aimPath">目标文件夹</param>
        public static void CopyDir(string srcPath, string aimPath)
        {
            try
            {
                // 检查目标目录是否以目录分割字符结束如果不是则添加之
                if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;
                // 判断目标目录是否存在如果不存在则新建之
                if (!Directory.Exists(aimPath))
                    Directory.CreateDirectory(aimPath);
                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                //如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
                //string[] fileList = Directory.GetFiles(srcPath);
                string[] fileList = Directory.GetFileSystemEntries(srcPath);
                //遍历所有的文件和目录
                foreach (string file in fileList)
                {
                    //先当作目录处理如果存在这个目录就递归Copy该目录下面的文件

                    if (Directory.Exists(file))
                        CopyDir(file, aimPath + Path.GetFileName(file));
                    //否则直接Copy文件
                    else
                        File.Copy(file, aimPath + Path.GetFileName(file), true);
                }

            }
            catch (Exception ee)
            {
                throw new Exception(ee.ToString());
            }
        }

        #endregion
        /// <summary>
        /// 读入到字节数组中比较(ReadOnlySpan)两个文件的大小
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        public static bool CompareByReadOnlySpan(string file1, string file2)
        {
            const int BYTES_TO_READ = 1024 * 10;

            using (FileStream fs1 = File.Open(file1, FileMode.Open))
            using (FileStream fs2 = File.Open(file2, FileMode.Open))
            {
                byte[] one = new byte[BYTES_TO_READ];
                byte[] two = new byte[BYTES_TO_READ];
                while (true)
                {
                    int len1 = fs1.Read(one, 0, BYTES_TO_READ);
                    int len2 = fs2.Read(two, 0, BYTES_TO_READ);
                    // 字节数组可直接转换为ReadOnlySpan
                    if (!((ReadOnlySpan<byte>)one).SequenceEqual((ReadOnlySpan<byte>)two)) return false;
                    if (len1 == 0 || len2 == 0) break;  // 有文件读取到了末尾,退出while循环
                }
            }

            return true;
        }

        /// <summary>
        /// 替换文件里面的文件内容
        /// </summary>
        /// <param name="fileFullName">文件路径名</param>
        /// <param name="OriginalStr">原字符串</param>
        /// <param name="ReplaceStr">替换字符串</param>
        public static void ReplaceFileContent(string fileFullName, string OriginalStr, string ReplaceStr, Encoding encode)
        {
            //var str = ReadFile(fileFullName, Encoding.UTF8);
            //str = str.Replace(OriginalStr, ReplaceStr);
            //WriteFile(fileFullName,str, Encoding.UTF8);
            var str = ReadFile(fileFullName);
            str = str.Replace(OriginalStr, ReplaceStr);
            WriteFile(fileFullName, str);
        }
        public static void ReplaceFileContent(string fileFullName, string OriginalStr, string ReplaceStr)
        {
            var str = ReadFile(fileFullName, Encoding.Default);
            str = str.Replace(OriginalStr, ReplaceStr);
            WriteFile(fileFullName, str, Encoding.Default);

        }
        public static void ReplaceXamlFileContent(string fileFullName, string OriginalStr, string ReplaceStr)
        {

            FileStream fs = new FileStream(fileFullName, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string con = sr.ReadToEnd();
            con = con.Replace(OriginalStr, ReplaceStr);
            sr.Close();
            fs.Close();
            FileStream fs2 = new FileStream(fileFullName, FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs2);
            sw.WriteLine(con);
            sw.Close();
            fs2.Close();
        }
        #region 查找目录下包含子目录的全部文件
        /// <summary>
        /// 获得目录下所有文件或指定文件类型文件地址
        /// </summary>
        public static List<FileInfo> fileList = new List<FileInfo>();
        public static List<FileInfo> GetDirAllFiles(string fullPath)
        {
            try
            {
                fileList.Clear();

                DirectoryInfo dirs = new DirectoryInfo(fullPath); //获得程序所在路径的目录对象
                DirectoryInfo[] dir = dirs.GetDirectories();//获得目录下文件夹对象
                FileInfo[] file = dirs.GetFiles();//获得目录下文件对象
                int dircount = dir.Length;//获得文件夹对象数量
                int filecount = file.Length;//获得文件对象数量

                //循环文件夹
                for (int i = 0; i < dircount; i++)
                {
                    string pathNode = fullPath + "\\" + dir[i].Name;
                    GetMultiFile(pathNode);
                }

                fileList.AddRange(file);
                return fileList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static bool GetMultiFile(string path)
        {
            if (Directory.Exists(path) == false)
            { return false; }

            DirectoryInfo dirs = new DirectoryInfo(path); //获得程序所在路径的目录对象
            DirectoryInfo[] dir = dirs.GetDirectories();//获得目录下文件夹对象
            FileInfo[] file = dirs.GetFiles();//获得目录下文件对象
            int dircount = dir.Length;//获得文件夹对象数量
            int filecount = file.Length;//获得文件对象数量
            int sumcount = dircount + filecount;

            if (sumcount == 0)
            { return false; }

            //循环文件夹
            for (int j = 0; j < dircount; j++)
            {
                string pathNodeB = path + "\\" + dir[j].Name;
                GetMultiFile(pathNodeB);
            }

            fileList.AddRange(file);

            return true;
        }
        #endregion

        //确定文件编码格式
        public static Encoding GetEncoding(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }
            Encoding encoding1 = Encoding.Default;
            if (File.Exists(filePath))
            {
                try
                {
                    using (FileStream stream1 = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        if (stream1.Length > 0)
                        {
                            using (StreamReader reader1 = new StreamReader(stream1, true))
                            {
                                char[] chArray1 = new char[1];
                                reader1.Read(chArray1, 0, 1);
                                encoding1 = reader1.CurrentEncoding;
                                reader1.BaseStream.Position = 0;
                                if (encoding1 == Encoding.UTF8)
                                {
                                    byte[] buffer1 = encoding1.GetPreamble();
                                    if (stream1.Length >= buffer1.Length)
                                    {
                                        byte[] buffer2 = new byte[buffer1.Length];
                                        stream1.Read(buffer2, 0, buffer2.Length);
                                        for (int num1 = 0; num1 < buffer2.Length; num1++)
                                        {
                                            if (buffer2[num1] != buffer1[num1])
                                            {
                                                encoding1 = Encoding.Default;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        encoding1 = Encoding.Default;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exception1)
                {
                    Console.WriteLine(exception1.Message);
                    throw;
                }
                if (encoding1 == null)
                {
                    encoding1 = Encoding.UTF8;
                }
            }
            return encoding1;
        }
    }
}
