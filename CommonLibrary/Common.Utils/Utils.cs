using Common.Constant;
using Common.Enum;
using log4net;
using System.Web;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
// using Model.RequestModel;
using System.Net.Security;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System;
using Model.ResponseModel;

namespace Common.Utils
{
    public static class Utils
    {
        public static BaseResponse<T> CreateResponseModel<T>(T data, int record = 0)
        {
            return new BaseResponse<T>
            {
                Message = StatusMessage.Success,
                Data = data,
                TotalRecord = record
            };
        }

        public static BaseResponse<T> CreateErrorModel<T>(StatusCode statusCode = StatusCode.BabRequire, string message = StatusMessage.Error, Exception? exception = null)
        {
            var sttCode = (int)statusCode;
            try
            {
                switch (exception)
                {
                    case MyException ex:
                    {
                        if (message == exception.Message) { }
                        else message += $"\n{exception.GetExceptionMessage()}";
                        sttCode = (int)ex.RetCode;
                        break;
                    }
                    default:
                    {
                        if (exception != null)
                        {
                            sttCode = exception.HResult;
                            if (message == exception.Message) { }
                            else message += $"\n{exception.GetBaseException().Message}";
                        }

                        break;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return new BaseResponse<T>
            {
                StatusCode = sttCode,
                Message = message
            };
        }
        
        public static string? GetResources(string resourceKey, string language)
        {
            try
            {
                string lan = CultureHelper.GetImplementedCulture(language);
                // Modify current thread's cultures
                CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(lan);
                //Thread.CurrentThread.CurrentCulture = cultureInfo;
                // Set the thread's CurrentUICulture.
                //Thread.CurrentThread.CurrentUICulture = new CultureInfo(lan);
                Resources.Culture = cultureInfo;
                var resourceSet = Resources.ResourceManager.GetResourceSet(cultureInfo, true, true);
                Debug.Assert(resourceSet != null, nameof(resourceSet) + " != null");
                var msgResource = resourceSet.GetString(resourceKey);
                return msgResource;
            }
            catch
            {
                // ignored
            }

            return string.Empty;
        }
        
        public static void WriteLogInfo(ILog log, string userId, HttpRequest request, object body)
        {
            //Write log HEADER & DATA & API Name
            var apiRequested = request.Path.Value ?? "";
            log.Info($"USERID: {userId} - URL: {apiRequested} - BODY: {JsonConvert.SerializeObject(body)} - MESSAGE: {StatusMessage.Success}");
        }
        
        public static void WriteLogError(ILog log, string userId, HttpRequest request, Exception ex)
        {
            //Write log
            string apiRequested = request.Path.Value ?? "";
            log.Error($"USERID: {userId} - URL: {apiRequested} failed. - BODY:  - MESSAGE: {ex.Message}");
        }
    }

    public static class ExceptionExtend
    {
        public static string GetExceptionMessage(this Exception ex)
        {
            return GetInnerMessage(ex);
        }

        private static string GetInnerMessage(this Exception ex)
        {
            if (ex.InnerException != null)
            {
                return GetInnerMessage(ex.InnerException);
            }
            else
            {
                return ex.Message;
            }
        }
    }

    public static class UtilsExtension
    {
        #region For dynamic

        /// <summary>
        /// Convert json string to expando object. 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static dynamic? JsonToExpandoObject(string json)
        {
            return JsonConvert.DeserializeObject<ExpandoObject>(json, new ExpandoObjectConverter());
        }

        public static string ToJsonString(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        #endregion
    }

    public static class Caster
    {
        #region Cast Model

        public static T Cast<T>(this Object myobj)
        {
            var target = typeof(T);
            var x = Activator.CreateInstance(target, false);
            var d = from source in target.GetMembers().ToList()
                    where source.MemberType == MemberTypes.Property
                    select source;
            var memberInfos = d as MemberInfo[] ?? d.ToArray();
            List<MemberInfo> members = memberInfos.Where(memberInfo => memberInfos.Select(c => c.Name)
                .ToList().Contains(memberInfo.Name)).ToList();
            foreach (var memberInfo in members)
            {
                var propertyInfo = typeof(T).GetProperty(memberInfo.Name);
                var value = myobj.GetType().GetProperty(memberInfo.Name)?.GetValue(myobj, null);

                propertyInfo?.SetValue(x, value, null);
            }
            return (T)x!;
        }

        #endregion
    }

    public class BaseResponse<T>
    {
        public int StatusCode { get; set; } = (int)Constant.StatusCode.Success;
        public string Message { get; set; } = "";
        public int TotalRecord { get; set; } = 0;
        public T? Data { get; set; }
    }
}