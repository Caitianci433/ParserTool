using DX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX.Servers
{
    public enum CMDTYPE
    {
        ERRORCMD = 0,
        Download_FileList_get,
        Download_FileList_put,
        Download_FileList_getFileInfo,
        Download_FileList_getDefaultFileInfo,
        Download_File_beginDownload,
        Download_File_beginPartiallyDownload,
        Download_File_prepareDownload,
        Download_File_download,
        Download_File_flushDownload,
        Download_File_endDownload,
        Download_File_flush,
        Download_File_beginLazyDownload,
        Download_CPU_notifyParameterUpdated,
        Download_CPU_isChangeConnectingIpAddr,
        Download_File_beginUpload,
        Download_File_upload,
        Download_File_endUpload,
        Download_Sync_lock,
        Download_Sync_unlock,
        Download_File_beginTarceDownload,
        Download_File_endTraceDownload,
        OTHERS
    }



    class FileWriterServer
    {
        public static bool WriteTheFile(string path, List<HttpModel> httplist) 
        {
            List<SaveFileModel> saveFileModels = InitCompareFileModel(httplist);

            using (System.IO.StreamWriter fs = new System.IO.StreamWriter(path, false))
            {
                
                fs.WriteLine("# " + DateTime.Now);
                fs.WriteLine("#CMDTYPE,IP,PORT,COMMANDINTERVAL,REQRESINTERVAL");

                foreach (var saveFileModel in saveFileModels)
                {
                    fs.WriteLine(saveFileModel.CommandType+","+
                                 saveFileModel.CommandInterval + "," +
                                 saveFileModel.ReqResInterval + "," +
                                 saveFileModel.IP + "," +
                                 saveFileModel.Port 
                                 );
                }
            }
            return true;
        }

        private static List<SaveFileModel> InitCompareFileModel(List<HttpModel> httplist) 
        {
            List<SaveFileModel> saveFileModels = new List<SaveFileModel>();

            //httplist to saveFileModels
            SaveFileModel saveFileModel = new SaveFileModel();
            saveFileModel.IP = "192.168.0.196";
            saveFileModel.Port = 80;
            saveFileModel.CommandType = "FileList_get";
            saveFileModel.CommandInterval = 1;
            saveFileModel.ReqResInterval = 2;
            saveFileModels.Add(saveFileModel);

            return saveFileModels;
        }
    }
}
