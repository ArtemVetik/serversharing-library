using Newtonsoft.Json;
using ServerSharing.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerSharingLibrary
{
    public class CachedSelectStream
    {
        private readonly YandexFunction _function;
        private readonly string _userId;
        private readonly Sort _sort;
        private readonly ulong _countOnPage;
        private readonly List<List<SelectResponseData>> _pages;

        internal CachedSelectStream(YandexFunction function, string userId, Sort sort, ulong countOnPage)
        {
            if (countOnPage <= 0)
                throw new ArgumentOutOfRangeException($"Argument {nameof(countOnPage)} must be positive");

            _function = function;
            _userId = userId;
            _sort = sort;
            _countOnPage = countOnPage;

            _pages = new List<List<SelectResponseData>>();
        }

        /// <summary>
        /// Loads a specific page by a given sorting order.
        /// </summary>
        /// <param name="page">Page Number. Starts from zero.</param>
        /// <returns></returns>
        public async Task<List<SelectResponseData>> LoadPage(int page)
        {
            while (_pages.Count <= page)
            {
                if (_pages[^1].Count < (int)_countOnPage)
                    return new List<SelectResponseData>(new List<SelectResponseData>());

                var body = CreateRequestBody();

                var request = Request.Create("SELECT", _userId, JsonConvert.SerializeObject(body));
                var response = await _function.Post(request);

                _pages.Add(new List<SelectResponseData>(JsonConvert.DeserializeObject<List<SelectResponseData>>(response.Body)));
            }

            return _pages[page];
        }

        private SelectRequestBody CreateRequestBody()
        {
            if (_pages.Count == 0)
            {
                return new SelectRequestBody()
                {
                    Parameters = new SelectRequestBody.SortParameters()
                    {
                        Sort = _sort,
                        Date = DateTime.Now.AddDays(1),
                        DownloadCount = uint.MaxValue,
                        LikeCount = uint.MaxValue,
                        RatingCount = uint.MaxValue,
                        RatingAverage = uint.MaxValue,
                    },
                    Limit = _countOnPage
                };
            }

            return new SelectRequestBody()
            {
                Parameters = new SelectRequestBody.SortParameters()
                {
                    Sort = _sort,
                    Date = _pages[^1][^1].Datetime,
                    DownloadCount = _pages[^1][^1].Downloads,
                    LikeCount = _pages[^1][^1].Likes,
                    RatingCount = _pages[^1][^1].RatingCount,
                    RatingAverage = _pages[^1][^1].RatingAverage,
                    Id = _pages[^1][^1].Id,
                },
                Limit = _countOnPage
            };
        }
    }
}