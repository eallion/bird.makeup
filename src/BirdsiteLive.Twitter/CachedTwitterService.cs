﻿using System;
using BirdsiteLive.Twitter.Models;
using Microsoft.Extensions.Caching.Memory;

namespace BirdsiteLive.Twitter
{
    public class CachedTwitterUserService : ITwitterUserService
    {
        private readonly ITwitterUserService _twitterService;

        private MemoryCache _userCache = new MemoryCache(new MemoryCacheOptions()
        {
            SizeLimit = 5000
        });
        private MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSize(1)//Size amount
            //Priority on removing when reaching size limit (memory pressure)
            .SetPriority(CacheItemPriority.High)
            // Keep in cache for this time, reset time if accessed.
            .SetSlidingExpiration(TimeSpan.FromHours(24))
            // Remove from cache after this time, regardless of sliding expiration
            .SetAbsoluteExpiration(TimeSpan.FromDays(30));

        #region Ctor
        public CachedTwitterUserService(ITwitterUserService twitterService)
        {
            _twitterService = twitterService;
        }
        #endregion

        public TwitterUser GetUser(string username)
        {
            if (!_userCache.TryGetValue(username, out TwitterUser user))
            {
                user = _twitterService.GetUser(username);
                _userCache.Set(username, user, _cacheEntryOptions);
            }

            return user;
        }
    }
}