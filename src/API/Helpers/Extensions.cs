using Polly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace ARCRM.Pidgets.MultiUserDiaryAPI.Helpers
{
    public static class Extensions
    {

        /// <summary>
        /// Convert data table to list of objects. Column names need to match property names (case insensitive)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dataTable) where T : new()
        {
            var dataList = new List<T>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            var objFieldNames = (from PropertyInfo aProp in typeof(T).GetProperties(flags)
                                 select aProp.Name).ToList();

            var dataTblFieldNames = (from DataColumn aHeader in dataTable.Columns
                                     select aHeader.ColumnName).ToList();

            var commonFields = objFieldNames.Intersect(dataTblFieldNames, StringComparer.OrdinalIgnoreCase).ToList();

            foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            {
                var aTSource = new T();
                foreach (var aField in commonFields)
                {
                    PropertyInfo propertyInfos = aTSource.GetType().GetProperty(aField);
                    var value = (dataRow[aField] == DBNull.Value) ? null : dataRow[aField]; //if database field is nullable
                    propertyInfos.SetValue(aTSource, value, null);
                }
                dataList.Add(aTSource);
            }


            return dataList;
        }

        public async static Task<HttpResponseMessage> SendAsyncWithRetry(this HttpClient httpClient, HttpRequestMessage requestMessage)
        {
            var generalExceptionPolicy = Policy
               .Handle<Exception>()
               .WaitAndRetryAsync(new[]
               {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(3)
               }, (exception, timeSpan) =>
               {
                   throw exception;
               });

            var responseStatusCode = Policy
              .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
              .WaitAndRetryAsync(new[]
              {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(3)
              });

            var response = await generalExceptionPolicy.ExecuteAsync<HttpResponseMessage>(async () =>
            {
                return await responseStatusCode.ExecuteAsync(async () => await httpClient.SendAsync(requestMessage));
            });

            return response;
        }
    }
}
