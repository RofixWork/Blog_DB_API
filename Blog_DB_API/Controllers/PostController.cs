using Azure;
using Blog_DB_API.DTOs;
using Blog_DB_API.Helpers;
using Blog_DB_API.Models;
using Blog_DB_API.Repository.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Blog_DB_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[ApiController]
    public class PostController : ControllerBase
    {
        private readonly IRepository<Post> _postRepository;

        public PostController(IRepository<Post> postRepository)
        {
            _postRepository = postRepository;
        }

        //get posts
        [HttpGet(Name = "GetPosts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            var posts = await _postRepository.FindAll(e => e.PublicationDate);
            var formatPosts = posts.Select(p => new
            {
                p.Id,
                p.Title,
                p.Content,
                p.PublicationDate,
                p.PostImage
            }).ToList();
            return Ok(Responses.OkResponse(formatPosts));
        }

        //get Post
        [HttpGet("{id:int}", Name = "GetPost")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            if (id <= 0) return BadRequest(Responses.BadRequestResponse("Enter id value grather than zero..."));

            var post = await _postRepository.FindOne(p => p.Id == id);

            if (post is null) return NotFound(Responses.NotFoundResponse($"Not found any post by this Id <<{id}>>"));

            var formatPost = new
            {
                post.Id,
                post.Title,
                post.Content,
                post.PublicationDate,
                post.PostImage
            };

            return Ok(Responses.OkResponse(formatPost));
        }

        //create post
        [HttpPost(Name = "CreatePost")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePost([FromForm] PostDTO postDTO)
        {
            //check post data
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage).ToList();

                return BadRequest(new {Errors=errors});
            }
            //convert image to array of bytes
            using MemoryStream stream = new();
            await postDTO.File!.CopyToAsync(stream);
            //create new post
            var post = new Post()
            {
                Title = postDTO.Title,
                Content = postDTO.Content,
                Image = stream.ToArray()
            };
            //add post
            await _postRepository.AddOne(post);
            //response
            return CreatedAtRoute("GetPost", new {id=post.Id} ,Responses.OkResponse("Your post created successfully..."));
        }

        //update post
        [HttpPut("{id:int}", Name = "UpdatePost")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePost(int id, [FromForm]PostDTO postDTO)
        {
            if (id <= 0) return BadRequest(Responses.BadRequestResponse("Enter id value grather than zero..."));

            if(!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage).ToList();

                return BadRequest(Responses.BadRequestResponse(errors));
            }

            var post = await _postRepository.FindOne(p => p.Id == id);

            if (post is null) return NotFound(Responses.NotFoundResponse($"Not found any post by this Id <<{id}>>"));

            using MemoryStream stream = new();
            await postDTO.File!.CopyToAsync(stream);
            post.Title = postDTO.Title ;
            post.Content = postDTO.Content;
            post.Image = stream.ToArray();
            await _postRepository.SaveAsync();
            return NoContent();
        }

        //update partial post
        [HttpPatch("{id:int}", Name = "UpdatePartialPost")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateParialPost(int id, [FromBody] JsonPatchDocument<Post> jsonPatchDocument)
        {
            if (id <= 0) return BadRequest(Responses.BadRequestResponse("Enter id value grather than zero..."));

            var post = await _postRepository.FindOne(p => p.Id == id);

            if (post is null) return NotFound(Responses.NotFoundResponse($"Not found any post by this Id <<{id}>>"));

            jsonPatchDocument.ApplyTo(post, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _postRepository.SaveAsync();
            return NoContent();
        }
    }
}
