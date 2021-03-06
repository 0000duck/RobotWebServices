﻿using RWS.IRC5.FileServices.ResponseTypes;
using RWS.IRC5.ResponseTypes;
using System;
using System.Threading.Tasks;
using static RWS.Enums;

namespace RWS.IRC5.RobotWareServices
{
    public class FileService
    {
        public IRC5Session ControllerSession { get; set; }

        public FileService(IRC5Session cs)
        {
            ControllerSession = cs;

        }


        public async Task<BaseResponse<GetDirectoryListingState>> GetDirectoryListingAsync(string path)
        {

            Tuple<string, string>[] dataParameters = null;
            Tuple<string, string>[] urlParameters = { Tuple.Create("json", "1") };

            return await ControllerSession.CallAsync<GetDirectoryListingState>(RequestMethod.GET, "fileservice/" + path, dataParameters, urlParameters).ConfigureAwait(false);

        }

        public async Task<dynamic> UploadFileAsync(string fromPath, string toPath, bool overwrite)
        {

            Tuple<string, string>[] dataParameters = { Tuple.Create(fromPath, "") };
            Tuple<string, string>[] urlParameters = { Tuple.Create("json", "1") };

            return await ControllerSession.CallAsync<dynamic>(RequestMethod.PUT, "/fileservice/" + toPath, dataParameters, urlParameters).ConfigureAwait(false);

        }

    }
}
