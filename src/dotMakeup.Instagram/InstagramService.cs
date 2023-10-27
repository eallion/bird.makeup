using BirdsiteLive.Common.Interfaces;
using BirdsiteLive.Common.Settings;
using BirdsiteLive.DAL.Contracts;

namespace dotMakeup.Instagram;

public class InstagramService : ISocialMediaService
{
        private readonly InstagramUserService _userService;

        #region Ctor
        public InstagramService(InstagramUserService userService, IInstagramUserDal userDal, InstanceSettings settings)
        {
            _userService = userService;
            UserDal = userDal;
        }
        #endregion

        public string ServiceName { get; } = "Instagram";
        public SocialMediaUserDal UserDal { get; }
        public async Task<SocialMediaUser> GetUserAsync(string username)
        {
            var user = await _userService.GetUserAsync(username);
            return user;
        }
}