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

        public async static Task<byte[]> Download(string id)
        {
            EnsureInitialize();

            var request = Request.Create("DOWNLOAD", _userId, id);
            var response = await _function.Post(request);

            return Convert.FromBase64String(response.Body);
        }

        public async static Task<byte[]> LoadImage(string id)
        {
            EnsureInitialize();

            var request = Request.Create("LOAD_IMAGE", _userId, id);
            var response = await _function.Post(request);

            return Convert.FromBase64String(response.Body);
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

        public async static Task<SelectResponseData> Info(string id)
        {
            EnsureInitialize();

            var request = Request.Create("INFO", _userId, id);
            var response = await _function.Post(request);

            return JsonConvert.DeserializeObject<SelectResponseData>(response.Body);
        }

        public async static Task<List<SelectResponseData>> SelectSelf(SelectEntryType entryType)
        {
            EnsureInitialize();

            var request = Request.Create("SELECT_SELF", _userId, JsonConvert.SerializeObject(entryType));
            var response = await _function.Post(request);

            return JsonConvert.DeserializeObject<List<SelectResponseData>>(response.Body);
        }

        public async static Task<ulong> Count()
        {
            EnsureInitialize();

            var request = Request.Create("COUNT", string.Empty, string.Empty);
            var response = await _function.Post(request);

            return Convert.ToUInt64(response.Body);
        }

        public static CachedSelectStream CreateSelectStream(Sort sort, ulong countOnPage)
        {
            return new CachedSelectStream(_function, _userId, sort, countOnPage);
        }

        private static void EnsureInitialize()
        {
            if (Initialized == false)
                throw new InvalidOperationException(nameof(ServerSharingApi) + " is not initialized");
        }
    }
}