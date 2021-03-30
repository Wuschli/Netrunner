using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using Netrunner.Server.Models;
using Netrunner.Shared.Chat;

namespace Netrunner.Server.Controllers.V1.Chat
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize]
    public class RoomController : ControllerBase
    {
        private readonly IMongoCollection<ChatRoom> _rooms;

        public RoomController(IDatabaseSettings settings)
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _rooms = database.GetCollection<ChatRoom>(settings.ChatRoomCollectionName);
        }

        // GET: api/<RoomController>
        [HttpGet]
        public async Task<IEnumerable<ChatRoom>> Get()
        {
            return await _rooms.Find(FilterDefinition<ChatRoom>.Empty).ToListAsync();
        }

        // GET api/<RoomController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChatRoom>> Get(string id)
        {
            return await _rooms.Find(room => room.Id == id).FirstAsync();
        }

        // POST api/<RoomController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ChatRoom value)
        {
            await _rooms.InsertOneAsync(value);
            return Ok();
        }

        // PUT api/<RoomController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] string value)
        {
            return Ok();
        }

        // DELETE api/<RoomController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            return Ok();
        }
    }
}