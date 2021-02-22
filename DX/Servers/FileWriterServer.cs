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
        public static bool WriteTheFile(string path) 
        {
            






            using (System.IO.StreamWriter fs = new System.IO.StreamWriter(path, false))
            {
                
                fs.WriteLine("test " + DateTime.Now);
                fs.WriteLine("CMDTYOE,REQRESTIME");
                fs.WriteLine("FileList_get");
                fs.WriteLine("FileList_put");
                fs.WriteLine("FileList_getFileInfo");
                fs.WriteLine("FileList_getDefaultFileInfo");
                fs.WriteLine("File_beginDownload");
                fs.WriteLine("File_beginPartiallyDownload");
                fs.WriteLine("File_prepareDownload");
                fs.WriteLine("File_download");
                fs.WriteLine("File_flushDownload");
                fs.WriteLine("File_endDownload");
                fs.WriteLine("File_flush");
                fs.WriteLine("File_beginLazyDownload");
                fs.WriteLine("CPU_notifyParameterUpdated");
                fs.WriteLine("CPU_isChangeConnectingIpAddr");
                fs.WriteLine("File_beginUpload");
                fs.WriteLine("File_upload");
                fs.WriteLine("File_endUpload");
                fs.WriteLine("Sync_lock");
                fs.WriteLine("Sync_unlock");
                fs.WriteLine("File_beginTarceDownload");
                fs.WriteLine("File_endTraceDownload");



            }


            return true;
        }
    }
}
