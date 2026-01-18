using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicApi.Data;
using MusicApi.Helpers;
using MusicApi.Models;
using System.Linq;

namespace MusicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private ApiDbContext _dbContext;

        public SongsController(ApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Song song)
        {
            var imageUrl = await FileHelper.UploadImage(song.Image);
            song.ImageUrl = imageUrl;
            var audioUrl = await FileHelper.UploadFile(song.AudioFile);
            song.AudioUrl = audioUrl;
            song.UploadedDate = DateTime.UtcNow;
            await _dbContext.Songs.AddAsync(song);
            await _dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);

        }

        [HttpGet]
        public async Task<IActionResult> GetAllSongs(int? pageNumber, int? pageSize)
        {
            //Pagination
            int currentPageNumber = pageNumber ?? 1;
            int currentPageSize = pageSize ?? 5;

            var songs = await (from song in _dbContext.Songs
                        select new
                        {
                            Id = song.Id,
                            Title = song.Title,
                            ImageUrl = song.ImageUrl,
                            AudioUrl = song.AudioUrl,
                            UploadedDate = song.UploadedDate,
                            ArtistId = song.ArtistId,
                            AlbumId = song.AlbumId
                        }).ToListAsync();
            return Ok(songs.Skip((currentPageNumber - 1)* currentPageSize).Take(currentPageSize));
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> FeaturedSongs()
        {
            var songs = await (from featuredSong in _dbContext.Songs
                               where featuredSong.IsFeatured == true
                               select new
                               {
                                   Id = featuredSong.Id,
                                   Title = featuredSong.Title,
                                   ImageUrl = featuredSong.ImageUrl,
                                   AudioUrl = featuredSong.AudioUrl,
                                   UploadedDate = featuredSong.UploadedDate,
                                   ArtistId = featuredSong.ArtistId,
                                   AlbumId = featuredSong.AlbumId
                               }).ToListAsync();
            return Ok(songs);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> NewSongs()
        {
            var songs = await (from song in _dbContext.Songs
                               orderby song.UploadedDate descending
                               select new
                               {
                                   Id = song.Id,
                                   Title = song.Title,
                                   ImageUrl = song.ImageUrl,
                                   AudioUrl = song.AudioUrl,
                                   UploadedDate = song.UploadedDate,
                                   ArtistId = song.ArtistId,
                                   AlbumId = song.AlbumId
                               }).Take(15).ToListAsync();
            return Ok(songs);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SearchSongs(string query)
        {
            var songs = await (from song in _dbContext.Songs
                               where song.Title.StartsWith(query)
                               select new
                               {
                                   Id = song.Id,
                                   Title = song.Title,
                                   ImageUrl = song.ImageUrl,
                                   AudioUrl = song.AudioUrl,
                                   UploadedDate = song.UploadedDate,
                                   ArtistId = song.ArtistId,
                                   AlbumId = song.AlbumId
                               }).Take(15).ToListAsync();
            return Ok(songs);
        }
    }
}
