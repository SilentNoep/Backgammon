using Common;
using Newtonsoft.Json;
using SignalRChat.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SignalRChat.Controllers
{
    public class UserController : ApiController
    {
        Repository _rep = new Repository();


        [HttpPost]
        [ActionName("checkUserValidation")]
        public async Task<HttpResponseMessage> checkUserValidation()
        {
            byte[] parms = await Request.Content.ReadAsByteArrayAsync();
            string jsonStr = Encoding.UTF8.GetString(parms);
            var user = JsonConvert.DeserializeObject<User>(jsonStr); // Convert JSON to Users
            if (_rep.IsValid(user.UserName, user.Password))
            {
                User currentUser = _rep.GetUser(user.UserName);
                return Request.CreateResponse(HttpStatusCode.Created, currentUser);
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "error");
        }



        // POST api/user
        [HttpPost]
        [ActionName("register")]
        public async Task<HttpResponseMessage> register()
        {
            byte[] parms = await Request.Content.ReadAsByteArrayAsync();
            string jsonStr = Encoding.UTF8.GetString(parms);

            var user = JsonConvert.DeserializeObject<User>(jsonStr); // Convert JSON to Users
            Task.WaitAll();
            var newUser = _rep.AddUser(user.FirstName, user.LastName,user.UserName,user.Password,user.Birthdate);
            if (newUser == null)
                return Request.CreateResponse(HttpStatusCode.Created, "User Exists");
            return Request.CreateResponse(HttpStatusCode.Created, newUser);
        }

        #region Status Actions
        [HttpPost]
        [ActionName("offlineUser")]
        public async Task<HttpResponseMessage> offlineUser()
        {
            byte[] parms = await Request.Content.ReadAsByteArrayAsync();
            string jsonStr = Encoding.UTF8.GetString(parms);
            var user = JsonConvert.DeserializeObject<User>(jsonStr); // Convert JSON to Users
            _rep.ChangeUserStatus(user,Status.Offline);
            return Request.CreateResponse(HttpStatusCode.Created);
        }
        [HttpPost]
        [ActionName("onlineUser")]
        public async Task<HttpResponseMessage> onlineUser()
        {
            byte[] parms = await Request.Content.ReadAsByteArrayAsync();
            string jsonStr = Encoding.UTF8.GetString(parms);
            var user = JsonConvert.DeserializeObject<User>(jsonStr); // Convert JSON to Users
            _rep.ChangeUserStatus(user, Status.Online);
            return Request.CreateResponse(HttpStatusCode.Created);
        }
        [HttpPost]
        [ActionName("inGameUser")]
        public async Task<HttpResponseMessage> inGameUser()
        {
            byte[] parms = await Request.Content.ReadAsByteArrayAsync();
            string jsonStr = Encoding.UTF8.GetString(parms);
            var user = JsonConvert.DeserializeObject<User>(jsonStr); // Convert JSON to Users
            _rep.ChangeUserStatus(user, Status.InGame);
            return Request.CreateResponse(HttpStatusCode.Created);
        }
        #endregion



        [HttpPost]
        [ActionName("addLossToUser")]
        public async Task<HttpResponseMessage> addLossToUser()
        {
            byte[] parms = await Request.Content.ReadAsByteArrayAsync();
            string jsonStr = Encoding.UTF8.GetString(parms);
            var user = JsonConvert.DeserializeObject<User>(jsonStr); // Convert JSON to Users
            _rep.AddLossToUser(user);
            return Request.CreateResponse(HttpStatusCode.Created);
        }




        [HttpGet]
        [ActionName("getOnlineUsers")]
        public IEnumerable<User> getOnlineUsers()
        {
            return _rep.GetAllUsers();
        }

        [HttpGet]
        [ActionName("runingServer")]
        public string runingServer()
        {
            string baseUrl = Url.Request.RequestUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
            return $"The server is running on {baseUrl}";
        }
    }
}
