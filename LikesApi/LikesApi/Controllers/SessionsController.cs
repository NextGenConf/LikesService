namespace LikesApi.Controllers
{
    using LikesApi.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly LikesService likesService;

        public SessionsController(LikesService likesService)
        {
            this.likesService = likesService;
        }

        [HttpGet("{id}/likes/users")]
        public ActionResult<List<string>> GetSessions(string id)
        {
            var likes = likesService.GetUsersPerSession(id);

            if (likes == null)
            {
                return NotFound();
            }

            return likes;
        }

        [HttpGet("{id}/likes/users/count")]
        public ActionResult<int> GetSessionsCount(string id)
        {
            var likes = likesService.GetUsersPerSession(id);

            if (likes == null)
            {
                return NotFound();
            }

            return likes.Count;
        }

        [HttpPost("{id}/likes")]
        public IActionResult Create(string id, [FromBody] string userId)
        {
            likesService.AddNewLikeForSession(userId, id);

            return new OkObjectResult(id);
        }
    }
}
