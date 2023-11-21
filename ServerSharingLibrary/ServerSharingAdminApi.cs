using ServerSharing.Data;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServerSharingLibrary
{
    public static class ServerSharingAdminApi
    {
        private static readonly HttpClient _client = new HttpClient();

        private static YandexFunction _function;

        public static void Initialize(string functionId, string authToken)
        {
            if (Initialized)
                throw new InvalidOperationException(nameof(ServerSharingAdminApi) + " has already been initialized");

            _function = new YandexFunction(functionId ?? throw new ArgumentNullException(nameof(functionId)));
            _function.Authorize(authToken);
        }

        public static bool Initialized => _function != null;

        public async static Task<string> UserId(string id)
        {
            EnsureInitialize();

            var request = Request.Create("USER_ID", string.Empty, id);
            var response = await _function.Post(request);

            return response.Body.ToString();
        }

        public async static Task<Response> Delete(string id, string userId)
        {
            EnsureInitialize();

            var request = Request.Create("DELETE", userId, id);
            return await _function.Post(request);
        }

        private static void EnsureInitialize()
        {
            if (Initialized == false)
                throw new InvalidOperationException(nameof(ServerSharingAdminApi) + " is not initialized");
        }
    }
}