using Newtonsoft.Json;
using ServerSharing.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerSharingLibrary
{
    public static class ServerSharingApi
    {
        private static YandexFunction _function;
        private static string _userId;

        public static void Initialize(string functionId, string userId)
        {
            if (Initialized)
                throw new InvalidOperationException(nameof(ServerSharingApi) + " has already been initialized");

            _function = new YandexFunction(functionId ?? throw new ArgumentNullException(nameof(functionId)));
            _userId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public static bool Initialized => _function != null && _userId != null;

        public async static Task<Response> Upload(UploadData uploadData)
        {
            EnsureInitialize();

            var request = Request.Create("UPLOAD", _userId, JsonConvert.SerializeObject(uploadData));
            return await _function.Post(request);
        }

        public async static Task<ExtendedResponse<byte[]>> Download(string id)
        {
            EnsureInitialize();

            var request = Request.Create("DOWNLOAD", _userId, id);
            var response = await _function.Post(request);

            return new ExtendedResponse<byte[]>(response, Convert.FromBase64String(response.Body));
        }

        public async static Task<ExtendedResponse<byte[]>> LoadImage(string id)
        {
            EnsureInitialize();

            var request = Request.Create("LOAD_IMAGE", _userId, id);
            var response = await _function.Post(request);

            return new ExtendedResponse<byte[]>(response, Convert.FromBase64String(response.Body));
        }

        public async static Task<Response> Delete(string id)
        {
            EnsureInitialize();

            var request = Request.Create("DELETE", _userId, id);
            return await _function.Post(request);
        }

        public async static Task<Response> Like(string id)
        {
            EnsureInitialize();

            var request = Request.Create("LIKE", _userId, id);
            return await _function.Post(request);
        }

        public async static  Task<Response> Dislike(string id)
        {
            EnsureInitialize();

            var request = Request.Create("DISLIKE", _userId, id);
            return await _function.Post(request);
        }

        public async static Task<Response> Rate(string id, sbyte rate)
        {
            EnsureInitialize();

            var ratingRequest = new RatingRequestBody()
            {
                Id = id,
                Rating = rate,
            };

            var request = Request.Create("RATE", _userId, JsonConvert.SerializeObject(ratingRequest));
            return await _function.Post(request);
        }

        public async static Task<ExtendedResponse<SelectResponseData>> Info(string id)
        {
            EnsureInitialize();

            var request = Request.Create("INFO", _userId, id);
            var response = await _function.Post(request);

            return new ExtendedResponse<SelectResponseData>(response, JsonConvert.DeserializeObject<SelectResponseData>(response.Body));
        }

        public async static Task<ExtendedResponse<List<SelectResponseData>>> Select(EntryType entryType, SelectRequestBody.SelectOrderBy[] orderBy, ulong limit = 10, ulong offset = 0)
        {
            EnsureInitialize();

            var body = new SelectRequestBody()
            {
                EntryType = entryType,
                OrderBy = orderBy,
                Limit = limit,
                Offset = offset,
            };

            var request = Request.Create("SELECT", _userId, JsonConvert.SerializeObject(body));
            var response = await _function.Post(request);
            
            return new ExtendedResponse<List<SelectResponseData>>(response, JsonConvert.DeserializeObject<List<SelectResponseData>>(response.Body));
        }

        public async static Task<ExtendedResponse<ulong>> Count(EntryType entryType)
        {
            EnsureInitialize();

            var request = Request.Create("COUNT", _userId, JsonConvert.SerializeObject(entryType));
            var response = await _function.Post(request);

            return new ExtendedResponse<ulong>(response, Convert.ToUInt64(response.Body));
        }

        private static void EnsureInitialize()
        {
            if (Initialized == false)
                throw new InvalidOperationException(nameof(ServerSharingApi) + " is not initialized");
        }
    }
}