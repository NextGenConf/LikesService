namespace LikesApi.Controllers
{
    using LikesApi.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    [Route("api/[controller]")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly LikesService likesService;

        public UserController(LikesService likesService)
        {
            this.likesService = likesService;
        }

        [HttpGet("{userId}/likes/conferences")]
        public ActionResult<List<string>> GetConferences(string userId)
        {
            var likes = likesService.GetConferencesPerUser(userId);
            
            if (likes == null)
            {
                return NotFound();
            }

            return likes;
        }

        [HttpGet("{userId}/likes/conferences/count")]
        public ActionResult<int> GetConferencesCount(string userId)
        {
            var likes = likesService.GetConferencesPerUser(userId);

            if (likes == null)
            {
                return NotFound();
            }

            return likes.Count;
        }

        [HttpGet("{userId}/likes/sessions")]
        public ActionResult<List<string>> GetSessions(string userId)
        {
            var likes = likesService.GetSessionsPerUser(userId);

            if (likes == null)
            {
                return NotFound();
            }

            return likes;
        }

        [HttpGet("{userId}/likes/sessions/count")]
        public ActionResult<int> GetSessionsCount(string userId)
        {
            var likes = likesService.GetSessionsPerUser(userId);

            if (likes == null)
            {
                return NotFound();
            }

            return likes.Count;
        }
    }
}
