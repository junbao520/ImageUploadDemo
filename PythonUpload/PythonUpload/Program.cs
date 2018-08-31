using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PythonUpload
{
    class Program
    {
        static void Main(string[] args)
        {
            String UploadUrl = "http://2176mf7449.51mypc.cn:57268/upload";

            //String UpLoadFilePath_1 = "0.jpg";
            //String UpLoadFilePath_2 = "1.jpg";

            //Test
            //BinaryReader oBinaryReaderFile1 = new BinaryReader(new FileStream(UpLoadFilePath_1, FileMode.Open, FileAccess.Read));
            //BinaryReader oBinaryReaderFile2 = new BinaryReader(new FileStream(UpLoadFilePath_2, FileMode.Open, FileAccess.Read));

            //Test Modified
            //Test Modified Second
            String PostData = String.Empty;





            var request = WebRequest.Create(UploadUrl) as HttpWebRequest;
            request.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.None;
            request.Credentials = CredentialCache.DefaultNetworkCredentials;
            request.UseDefaultCredentials = false;
            request.KeepAlive = true;
            request.PreAuthenticate = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.116 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            //request.Timeout = 3000;


            string boundary = "WebKitFormBoundary" + GetRnd(16, false, true, true, false, String.Empty);
            string beginBoundary = "------" + boundary;
            string endBoundary = "------" + boundary + "--";
            request.ContentType = "multipart/form-data; boundary=----" + boundary;

            request.Method = "POST";
            using (var postStream = request.GetRequestStream())
            {
                {
                    StringBuilder fileParam = new StringBuilder();
                    fileParam.AppendLine(beginBoundary);
                    fileParam.AppendLine("Content-Disposition: form-data; name=\"myfile\"; filename=\"0.jpg\"");
                    fileParam.AppendLine("Content-Type: image/jpeg");
                    fileParam.AppendLine();
                    byte[] fileParamBytes = Encoding.UTF8.GetBytes(fileParam.ToString());
                    postStream.Write(fileParamBytes, 0, fileParamBytes.Length);

                    byte[] fileByte = File.ReadAllBytes(@"0.jpg");
                    postStream.Write(fileByte, 0, fileByte.Length);

                }

                {
                    StringBuilder fileParam = new StringBuilder();
                    fileParam.AppendLine();
                    fileParam.AppendLine(beginBoundary);
                    fileParam.AppendLine("Content-Disposition: form-data; name=\"myfile2\"; filename=\"1.jpg\"");
                    fileParam.AppendLine("Content-Type: image/jpeg");
                    fileParam.AppendLine();
                    byte[] fileParamBytes = Encoding.UTF8.GetBytes(fileParam.ToString());
                    postStream.Write(fileParamBytes, 0, fileParamBytes.Length);

                    byte[] fileByte = File.ReadAllBytes(@"1.jpg");
                    postStream.Write(fileByte, 0, fileByte.Length);
                }


                {
                    StringBuilder endParam = new StringBuilder();
                    endParam.AppendLine();
                    endParam.AppendLine(endBoundary);
                    byte[] endByte = Encoding.UTF8.GetBytes(endParam.ToString());
                    postStream.Write(endByte, 0, endByte.Length);
                }


            }

            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response != null)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Console.Write(String.Concat("请求地址:", request.RequestUri, " 失败,HttpStatusCode", response.StatusCode));
                    }
                    using (var streamResponse = response.GetResponseStream())
                    {
                        if (streamResponse != null)
                        {
                            var contentEncodingStr = response.ContentEncoding;
                            var contentEncoding = Encoding.UTF8;
                            if (!String.IsNullOrEmpty(contentEncodingStr))
                                contentEncoding = Encoding.GetEncoding(contentEncodingStr);
                            var streamRead = new StreamReader(streamResponse, contentEncoding);
                            var str = streamRead.ReadToEnd();

                            Console.Write(str);


                        }
                    }
                    response.Close();
                }
            }

        }

        /// <summary>
        /// 获取随机的组装请求头部
        /// </summary>
        /// <param name="length"></param>
        /// <param name="useNum"></param>
        /// <param name="useLow"></param>
        /// <param name="useUpp"></param>
        /// <param name="useSpe"></param>
        /// <param name="custom"></param>
        /// <returns></returns>
        public static string GetRnd(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;

            if (useNum) { str += "0123456789"; }
            if (useLow) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            if (string.IsNullOrEmpty(str)) return string.Empty;
            for (int i = 0; i < length; i++) { s += str.Substring(r.Next(0, str.Length - 1), 1); }

            return s;
        }
    }
}
