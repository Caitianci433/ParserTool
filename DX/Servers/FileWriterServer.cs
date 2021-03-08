using DX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX.Servers
{
    class FileWriterServer
    {
        public static bool WriteTheFile(string path, List<HttpModel> httplist) 
        {
            List<SaveFileModel> saveFileModels = InitCompareFileModel(httplist);

            using (System.IO.StreamWriter fs = new System.IO.StreamWriter(path, false))
            {
                
                fs.WriteLine("# Time" + DateTime.Now+"Length:"+ saveFileModels.Count.ToString());
                fs.WriteLine("#CMDTYPE,COMMANDINTERVAL,REQRESINTERVAL,IP,PORT");

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
            ParserServer.Parser(httplist);
            for (int i = 0; i < ParserServer.list.Count; i++)
            {
                SaveFileModel saveFileModel = new SaveFileModel();

                saveFileModel.IP = ParserServer.list[i].Req.IP_SourceAddress.Replace(',', '.');

                saveFileModel.Port = ParserServer.list[i].Req.TCP_SourcePort;

                saveFileModel.CommandType = GetCmdTypeFromHttpModel(ParserServer.list[i]);

                saveFileModel.CommandInterval = GetCmdIntervalFromHttpModel(ParserServer.list[i]);

                saveFileModel.ReqResInterval = GetReqResIntervalFromHttpModel(ParserServer.list[i]);



                saveFileModels.Add(saveFileModel);
            }
            return saveFileModels;
        }

        private static string GetCmdTypeFromHttpModel(ReqRse reqres) 
        {
            string cmd;
            if (reqres.Req.Content.Length==0)
            {
                cmd = reqres.Req.Head;
            }
            else
            {
                cmd = reqres.Req.Content;
            }

            var ret = "Unkown CMD";
            _ = Common.CommandManager.Commands.Any(c =>
            {
                if (cmd.Contains(c))
                {
                    ret = c;
                    return true;
                }
                return false;
            });
            return ret;


        }
        private static int GetCmdIntervalFromHttpModel(ReqRse reqres)
        {






            return 0;
        }
        private static int GetReqResIntervalFromHttpModel(ReqRse reqres)
        {






            return 0;
        }

    }
}
