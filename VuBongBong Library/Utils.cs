using Library.Resource;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Library
{
    public static class UtilsExtension
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UtilsExtension));
        public static IEnumerable<T> PocoSerializeList<T>(this IEnumerable<T> list) where T : class
        {
            foreach (T item in list)
            {
                yield return PocoSerialize(item);
            }
        }


        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }


        public static T PocoSerialize<T>(T entites) where T : class
        {
            //var convertTobye = entites.ToByteArray();
            //return convertTobye.DeSerialize() as T;
            var jSon = JsonConvert.SerializeObject(entites);
            return JsonConvert.DeserializeObject<T>(jSon);
        }

        public static string SerializeObject(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static byte[] ToByteArray(this object objObject)
        {
            try
            {
                if (objObject == null)
                    return null;

                using (var memoryStream = new MemoryStream())
                {
                    (new BinaryFormatter()).Serialize(memoryStream, objObject);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception.GetInnerMessage());
            }
            return null;
        }

        public static object DeSerialize(this byte[] arrBytes)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var binaryFormatter = new BinaryFormatter();
                    var decompressed = Decompress(arrBytes);

                    memoryStream.Write(decompressed, 0, decompressed.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    return binaryFormatter.Deserialize(memoryStream);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception.GetInnerMessage());
            }
            return null;
        }

        private static byte[] Decompress(byte[] input)
        {
            byte[] decompressedData;

            using (var outputStream = new MemoryStream())
            {
                using (var inputStream = new MemoryStream(input))
                {
                    using (var zip = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        zip.CopyTo(outputStream);
                    }
                }

                decompressedData = outputStream.ToArray();
            }

            return decompressedData;
        }

        /// <summary>
        /// Get inner message
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>Inner message</returns>
        public static string GetInnerMessage(this Exception exception)
        {
            if (exception != null)
            {
                if (exception.InnerException != null)
                {
                    return exception.InnerException.GetInnerMessage();
                }
                return exception.Message;
            }
            return string.Empty;
        }

        /// <summary>
        /// Make random string 8 (default) char in length
        /// </summary>
        /// <param name="length">length</param>
        /// <returns></returns>
        public static string RandomString(int? length)
        {
            return Guid.NewGuid().ToString("n").Substring(0, length ?? 8);
        }

        /// <summary>
        /// Get inner message
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>Inner message</returns>
        public static string ReplaceSpecialSymbol(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            string newStr = Regex.Replace(str, @"[^0-9a-zA-Z]+", ",");
            return newStr;
        }

        /// <summary>
        /// Compare 2 array
        /// </summary>
        /// <param name="arModelA"></param>
        /// <param name="arModelB"></param>
        /// <returns>True: Equals - False: Not Equals</returns>
        public static bool CompareArrays(this byte[] arModelA, byte[] arModelB)
        {
            if (arModelA.Length != arModelB.Length)
                return false;

            for (int i = 0; i < arModelA.Length; i++)
            {
                if (arModelA[i] != arModelB[i])
                    return false;
            }
            return true;
        }

        public static IEnumerable<string[]> SplitColon(this IEnumerable<string> input)
        {
            foreach (var item in input)
            {
                yield return item.Split(':');
            }
        }

        public static IEnumerable<string> RemoveDash(this IEnumerable<string> stringList)
        {
            foreach (var str in stringList)
            {
                yield return str.Replace("-", string.Empty);
            }
        }


        public static IEnumerable<Guid> ConvertStringToGuid(this IEnumerable<string> input)
        {
            foreach (var item in input)
            {
                Guid output;
                if (Guid.TryParse(item, out output))
                    yield return output;
            }
        }

        public static string RemoveMetaCharacters(this string input)
        {
            return input.Replace(@"{", string.Empty).Replace(@"}", string.Empty).Replace(@"[", string.Empty).Replace(@"]", string.Empty);
        }

        public static T GetPluginConfig<T>(this IEnumerable<object> extensionConfig, string configCode)
        {
            var value = extensionConfig.Cast<dynamic>().Where(g =>
             {
                 string code = g.GetType().GetProperty("Code")?.GetValue(g);
                 return code.Equals(configCode);
             }).Select(g => g.GetType().GetProperty("Value")?.GetValue(g)).FirstOrDefault();
            return (T)Convert.ChangeType(value, typeof(T));
        }

        #region Unicode

        //private static readonly string[] VietnameseSigns = new string[]
        //{
        //    "aAeEoOuUiIdDyY",
        //    "áàạảãâấầậẩẫăắằặẳẵ",
        //    "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        //    "éèẹẻẽêếềệểễ",
        //    "ÉÈẸẺẼÊẾỀỆỂỄ",
        //    "óòọỏõôốồộổỗơớờợởỡ",
        //    "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        //    "úùụủũưứừựửữ",
        //    "ÚÙỤỦŨƯỨỪỰỬỮ",
        //    "íìịỉĩ",
        //    "ÍÌỊỈĨ",
        //    "đ",
        //    "Đ",
        //    "ýỳỵỷỹ",
        //    "ÝỲỴỶỸ"
        //};

        //public static string NonUnicode(this string str)
        //{
        //    for (int i = 1; i < VietnameseSigns.Length; i++)
        //    {
        //        for (int j = 0; j < VietnameseSigns[i].Length; j++)
        //            str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
        //    }
        //    return str;
        //}

        public static string NonUnicode(this string str)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = str.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        #endregion Unicode

        /// <summary>
        /// Dùng để gọi store procedure mà có tham số truyền vào là danh sách Guid.
        /// Use for call store procedure have paramater is guid array. 
        /// </summary>
        /// <param name="arGuids"></param>
        /// <returns></returns>
        public static DataTable BuildGuidDataType(this IEnumerable<Guid> arGuids)
        {
            DataTable retVal = new DataTable("GuidIds");
            retVal.Columns.Add("Id", typeof(Guid));
            if (arGuids == null) return retVal;
            foreach (Guid g in arGuids)
            {
                var dr = retVal.NewRow();
                dr[0] = g;
                retVal.Rows.Add(dr);
            }
            return retVal;
        }

        public static DataTable BuildIntDataType(this IEnumerable<int> arInts)
        {
            DataTable retVal = new DataTable("IntIds");
            retVal.Columns.Add("Id", typeof(int));
            if (arInts == null) return retVal;
            foreach (int g in arInts)
            {
                var dr = retVal.NewRow();
                dr[0] = g;
                retVal.Rows.Add(dr);
            }
            return retVal;
        }

        public static bool ExistIn(this string stepCode, string[] arStepCode)
        {
            return Array.IndexOf(arStepCode, stepCode) != -1;
        }
    }

    public class Utils
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UtilsExtension));

        public JToken GetDirectory(DirectoryInfo directory, int readLevel = 0)
        {
            if (readLevel <= ConstResource.I_READ_SERVER_FOLDER_LEVEL)
            {
                readLevel++;
                var dirJson = directory.EnumerateDirectories()
                    .ToDictionary(x => x.Name, x => GetDirectory(x, readLevel));
                return JToken.FromObject(new
                {
                    directory = dirJson,
                    //file = directory.EnumerateFiles().Select(x => x.Name).ToList()
                });
            }
            return null;
        }

        public T GetValue<T>(string key, string prefix = "config")
        {
            var entry = string.Format("{0}:{1}", prefix, key);
            var value = ConfigurationManager.AppSettings[entry];
            // Change the entry value to the specified type
            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Hash input string to MD5
        /// </summary>
        /// <param name="input">input string for hash</param>
        /// <returns>string of md5</returns>
        public string HashMd5(string input)
        {
            string result = string.Empty;
            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();
                // Loop through each byte of the hashed data
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                // Return the hexadecimal string.
                result = sBuilder.ToString();
            }
            return result;
        }


        public bool VerifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = HashMd5(input);
            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return 0 == comparer.Compare(hashOfInput, hash);
        }

        public byte[] ReScaleImage(byte[] imageByteArray, int maxWidth, int maxHeight)
        {
            if (imageByteArray != null && imageByteArray.Length > 0)
            {
                System.Drawing.Image rescaled = ReScaleImage(ByteArrayToImage(imageByteArray), maxWidth, maxHeight);
                if (rescaled == null) return null;

                using (var ms = new MemoryStream())
                {
                    rescaled.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
            return null;
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public System.Drawing.Image ByteArrayToImage(byte[] byteArrayIn)
        {
            System.Drawing.Image returnImage = null;
            try
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArrayIn);
                returnImage = System.Drawing.Image.FromStream(stream);//Paramter is not valid
            }
            catch (Exception e)
            {
                //string str = e.Message;
                Log.Debug(e.Message);
            }
            return returnImage;
        }

        public void SaveImage(byte[] imageByte, string path)
        {
            System.Drawing.Image image = this.ByteArrayToImage(imageByte);
            this.SaveImage(image, path);
        }

        public void SaveImage(System.Drawing.Image image, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                image.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        public System.Drawing.Image ReScaleImage(System.Drawing.Image image, int maxWidth, int maxHeight)
        {
            if (image == null)
                return null;

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        public static DateTime ConvertStrToDateTime(string strDate, string strTime, DateTime? defaultVal = null)
        {
            try
            {
                if (string.IsNullOrEmpty(strTime))
                {
                    return Convert.ToDateTime(strDate);
                }
                if (!string.IsNullOrEmpty(strDate) && !string.IsNullOrEmpty(strTime))
                {
                    DateTime value = DateTime.Parse(strDate);
                    DateTime timeValue = Convert.ToDateTime(strTime);
                    value = value.AddHours(timeValue.Hour).AddMinutes(timeValue.Minute);
                    return value;
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
            }
            return defaultVal ?? DateTime.Now;
        }
    }

}