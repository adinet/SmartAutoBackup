using SmartAutoBackup.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentFTP;

namespace SmartAutoBackup.Job
{
    public class JobBackup
    {
        public static void Backup(JobModel model)
        {
            var connStr = $"data source={model.Server};initial catalog={model.DBName};persist security info=True;user id={model.UserName};password={model.Password};MultipleActiveResultSets=True;App=SmartAutoBackup";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                try
                {
                    var fileName = $"{model.DBName}_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff")}.bak";
                    var path = Path.Combine(GlobalVar.Config.SavePath, model.Name + "\\");
                    var filePath = Path.Combine(path, fileName);
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    conn.Open();
                    var cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = $@"BACKUP DATABASE [{model.DBName}] TO  DISK = N'{filePath}' WITH NOFORMAT, NOINIT,  NAME = N'{Path.GetFileNameWithoutExtension(fileName)}', SKIP, REWIND, NOUNLOAD, {(model.IsCompression ? "COMPRESSION," : "")}  STATS = 10";
                    cmd.ExecuteNonQuery();
                    GlobalVar.Logger.Info($"任务[{model.Name}]备份完成.{(model.IsUpload ? "即将开始上传..." : "不上传")}");
                    if (File.Exists(filePath) && model.IsUpload)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            Upload(model, filePath);
                        });

                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("COMPRESSION"))
                    {
                        model.IsCompression = false;
                        Backup(model);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static List<string> GetAllDataBase(string server, string username, string password)
        {
            //SELECT Name FROM Master..SysDatabases ORDER BY Name 
            var connStr = $"data source={server};initial catalog=master;persist security info=True;user id={username};password={password};MultipleActiveResultSets=True;App=SmartAutoBackup";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                try
                {
                    var list = new List<string>();
                    conn.Open();
                    var cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT Name FROM Master..SysDatabases ORDER BY Name";
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        list.Add(dr[0].ToString());
                    }

                    return list;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static void Upload(JobModel model, string bakFilePath)
        {
            try
            {
                GlobalVar.Logger.Info($"任务 [{model.Name}] 开始连接ftp...");
                var userCredential = new System.Net.NetworkCredential(model.FtpUserName, model.FtpPassword);
                FtpClient client = new FtpClient();
                client.Host = model.FtpHost;
                client.Port = model.FtpPort;
                client.Credentials = userCredential;
                client.Connect();
                if (client.IsConnected)
                {
                    if (!model.FtpPath.StartsWith("/"))
                    {
                        model.FtpPath = string.Concat("/", model.FtpPath);
                    }
                    if (!model.FtpPath.EndsWith("/"))
                    {
                        model.FtpPath = string.Concat(model.FtpPath, "/");
                    }
                    var fileName = Path.GetFileName(bakFilePath);
                    var remoteFilePath = string.Concat(model.FtpPath, fileName);
                    GlobalVar.Logger.Info($"任务 [{model.Name}] 正在上传文件{fileName}");
                    client.UploadFile(bakFilePath, remoteFilePath, overwrite: true, createRemoteDir: true);
                }
                else
                {
                    GlobalVar.Logger.Info($"任务 [{model.Name}] ftp连接未成功.");
                }
            }
            catch (Exception ex)
            {
                GlobalVar.Logger.Error(ex, $"任务 [{model.Name}] 上传失败... 原因:{ex.Message} | {ex.StackTrace}");
                //throw;
            }
        }

        public static bool TestConnection(string server, string username, string password)
        {
            var connStr = $"data source={server};initial catalog=master;persist security info=True;user id={username};password={password};MultipleActiveResultSets=True;App=SmartAutoBackup";
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public static bool TestFtpConnection(string host, int port, string username, string password)
        {
            try
            {
                var userCredential = new System.Net.NetworkCredential(username, password);
                FtpClient client = new FtpClient();
                client.Host = host;
                client.Port = port;
                client.Credentials = userCredential;
                client.Connect();
                var state = client.IsConnected;
                if (client.IsConnected)
                {
                    client.Disconnect();
                    client.Dispose();
                }

                return state;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
