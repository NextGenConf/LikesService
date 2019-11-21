namespace LikesApi.Controllers
{
    using LikesApi.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    [Route("api/[controller]")]
    [ApiController]
    public class ConferencesController : ControllerBase
    {
        private readonly LikesService likesService;

        public ConferencesController(LikesService likesService)
        {
            this.likesService = likesService;
        }

        [HttpGet("{id}/likes/users")]
        public ActionResult<List<string>> GetConferences(string id)
        {
            var likes = likesService.GetUsersPerConference(id);

            if (likes == null)
            {
                return NotFound();
            }

            return likes;
        }

        [HttpGet("{id}/likes/users/count")]
        public ActionResult<int> GetConferencesCount(string id)
        {
            var likes = likesService.GetUsersPerConference(id);

            if (likes == null)
            {
                return NotFound();
            }

            return likes.Count;
        }

        [HttpPost("{id}/likes")]
        public IActionResult Create(string id, [FromBody] string userId)
        {         
            likesService.AddNewLikeForConference(userId, id);

            return new OkObjectResult(id);
        }
    }
}
