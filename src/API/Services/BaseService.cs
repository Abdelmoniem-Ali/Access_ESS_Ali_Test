using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using LazyCache;
using ARCRM.Pidgets.MultiUserDiaryAPI.Helpers;

namespace ARCRM.Pidgets.MultiUserDiaryAPI.Services
{
    public class BaseService
    {
        private string _softwareKey;
        private string _accessToken;
        internal DataAccess DataAccess { get; set; }

        internal int? UserId { get; set; }
        public BaseService(string pSoftwareKey, string pAccessToken, string pUserEmail)
        {
            _softwareKey = pSoftwareKey;
            _accessToken = pAccessToken;
             //DataAccess = new DataAccess(GetConnectionString());
            DataAccess = new DataAccess("Data Source=VO-str-sql-03;Initial Catalog=ReflectRecruitmentGroup;User Id=ReflectRecruitmentGroup;Password=376439f24e524b548bdb77311de306bf;");
            UserId = GetUserId(pUserEmail);
            if (UserId == null)
            {
                throw new Exception("User " + pUserEmail + " Not Found");
            }
        }
        public int? GetUserId(string pUserEmail)
        {
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@UserEmail", pUserEmail));
            var userDetailsDT = DataAccess.ExecStoredProcedureReturnResultsDT("ACCESS.WORKSPACE_APP_GET_USER", sqlParams);
            if (userDetailsDT != null && userDetailsDT.Rows.Count > 0)
            {
                var userId = userDetailsDT.Rows[0]["UserId"];
                if (userId != null && userId != DBNull.Value)
                {
                    return (int)userId;
                }
            }
            return null;
        }

        public string GenerateSignature(string secret, string deliveryId, string timestamp, string payload)
        {
            //Convert the secret string to bytes
            var secretBytes = System.Text.Encoding.UTF8.GetBytes(secret);

            //Concat the values used to calculate the hash
            var valueToHash = $"{deliveryId}{timestamp}{payload}";

            using (var provider = new System.Security.Cryptography.HMACSHA256(secretBytes))
            {
                //Compute the hash using the UTF-8 bytes of the string
                var hash = provider.ComputeHash(System.Text.Encoding.UTF8.GetBytes(valueToHash));

                //Format as a lowercase hex encoded string
                var result = new System.Text.StringBuilder(hash.Length * 2);
                foreach (byte b in hash) result.Append(b.ToString("x2"));
                return result.ToString();
            }
        }

        public string GetConnectionString()
        {
            return new CachingService().GetOrAdd(_softwareKey, () =>
            {

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{Program.StaticConfig.GetSection("ConnectionApi").GetSection("Address").Value}?softwareKey={_softwareKey}");



                var secret = Program.StaticConfig.GetSection("ConnectionApi").GetSection("Secret").ToString();
                var deliveryId = Guid.NewGuid().ToString();
                var timeStamp = DateTime.UtcNow.ToString();
                var signature = GenerateSignature(secret, deliveryId, timeStamp, null);

                request.Headers.Add("X-ConnectionString-Signature", signature);
                request.Headers.Add("X-ConnectionString-Timestamp", timeStamp);
                request.Headers.Add("X-ConnectionString-DeliveryId", deliveryId);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                var httpClient = new HttpClient();
                var response = Task.Run(() => httpClient.SendAsyncWithRetry(request)).Result;

                var res = Task.Run(() => response
                        .Content.ReadAsStringAsync()).Result;
                if (!string.IsNullOrWhiteSpace(res) && response.IsSuccessStatusCode)
                {
                    return res;

                }

                return string.Empty;
            }, DateTime.UtcNow.AddDays(1));
        }
    }
}

