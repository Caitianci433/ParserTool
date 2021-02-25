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
                
                fs.WriteLine("# " + DateTime.Now);
                fs.WriteLine("#CMDTYPE,REQRESTIME");
                fs.WriteLine("FileList_get,0");
                fs.WriteLine("FileList_put,0");
                fs.WriteLine("FileList_getFileInfo,0");
                fs.WriteLine("FileList_getDefaultFileInfo,0");
                fs.WriteLine("File_beginDownload,0");
                fs.WriteLine("File_beginPartiallyDownload,0");
                fs.WriteLine("File_prepareDownload,0");
                fs.WriteLine("File_download,0");
                fs.WriteLine("File_flushDownload,0");
                fs.WriteLine("File_endDownload,0");
                fs.WriteLine("File_flush,0");
                fs.WriteLine("File_beginLazyDownload,0");
                fs.WriteLine("CPU_notifyParameterUpdated,0");
                fs.WriteLine("CPU_isChangeConnectingIpAddr,0");
                fs.WriteLine("File_beginUpload,0");
                fs.WriteLine("File_upload,0");
                fs.WriteLine("File_endUpload,0");
                fs.WriteLine("Sync_lock,0");
                fs.WriteLine("Sync_unlock,0");
                fs.WriteLine("File_beginTarceDownload,0");
                fs.WriteLine("File_endTraceDownload,0");



            }


            return true;
        }
    }
}
