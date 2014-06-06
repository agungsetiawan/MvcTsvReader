using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;

namespace MvcTsvReader.Common
{
    public class TsvReader
    {

        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// TSVファイルから読み込み
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="hasHeader"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static List<T> Read<T>(string fileName, bool hasHeader, Encoding encoding = null)
        {
            // if null encode
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            //Tタイプのプロパティ取得
            Type type = typeof(T);
            List<string> header = (from p in (type.GetProperties())
                                   select p.Name).ToList<string>();

            //TSVから読み込み結果リスト
            List<T> resultList = new List<T>();
            StreamReader fileStream = null;
            try
            {
                string filePath;
                try
                {
                    filePath = HttpContext.Current.Server.MapPath(fileName);
                }
                catch (Exception e)
                {
                    try
                    {
                        LocalDataStoreSlot serverData = Thread.GetNamedDataSlot("Server");
                        HttpServerUtilityBase server = Thread.GetData(serverData) as HttpServerUtilityBase;
                        filePath = server.MapPath(fileName);
                    }
                    catch (Exception)
                    {
                        throw e;
                    }
                }
                fileStream = new StreamReader(filePath, encoding);
                ConstructorInfo ci = type.GetConstructor(new Type[0]);
                string line;
                // TSVファイルにヘッダがある場合
                if (hasHeader) fileStream.ReadLine();
                while (!fileStream.EndOfStream)
                {
                    line = fileStream.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;
                    string[] lineArr = line.Split('\t');
                    //結果対象構造
                    T detail = (T)ci.Invoke(new Object[0]);
                    //TSVからdetailに読み込む
                    for (int i = 0; i < lineArr.Length; i++)
                    {
                        (type.GetProperty(header[i])).SetValue(detail, lineArr[i].Trim(), null);
                    }
                    resultList.Add(detail);
                }
            }
            catch (FileNotFoundException fex)
            {
                _logger.Info(fex.Message);
            }
            catch (Exception ex)
            {
                resultList.Clear();
                StringBuilder errLogMsg = new StringBuilder();
                errLogMsg.Append("\r\n");
                errLogMsg.Append("0: TSVファイル(" + fileName + ")から取得するエラー： ---Exception.\r\n");
                errLogMsg.Append("0: ExceptionType: " + ex.GetType() + "\r\n");
                errLogMsg.Append("0: ExceptionMessage: " + ex.Source.ToString(CultureInfo.InvariantCulture) + "\r\n");
                errLogMsg.Append("0: ExceptionStackTrace: " + ex.StackTrace + "\r\n");
                errLogMsg.Append("0: ExceptionMessage: " + ex.Message + "\r\n");
                errLogMsg.Append("0: ExceptionTargetSite: " + ex.TargetSite + "\r\n");
                _logger.Fatal(errLogMsg.ToString());
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
            return resultList;
        }
    }
}