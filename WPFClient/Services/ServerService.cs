using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Infra;

namespace WPFClient.Services
{
    public class ServerService : IServerService
    {
        public IEnumerable<User> GetAllUsers(string Uri)
        {
            Uri BaseUri = new Uri("http://localhost:52527/api/user");
            IEnumerable<User> users;
            using (var client = new HttpClient())
            {
                var getOnlineUsers = client.GetAsync($"{BaseUri}/getOnlineUsers").Result.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<IEnumerable<User>>(getOnlineUsers);
            }
            return users;
        }

        public void DisconnectFromServer(string Uri, User user)
        {
            Uri BaseUri = new Uri("http://localhost:52527/api/user");
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                var result = client.PostAsync($"{BaseUri}/offlineUser", content).Result;
                if (result.IsSuccessStatusCode == true)
                {
                    var a = result.Content.ReadAsStringAsync().Result;
                }
            }
        }
        public bool ConnectToServerRegister(string Uri, User user)
        {
            using (var client = new HttpClient())
            {
                Uri BaseUri = new Uri("http://localhost:52527/api/user");
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                var result = client.PostAsync($"{BaseUri}/register", content).Result;
                if (result.IsSuccessStatusCode == true)
                {
                    var a = result.Content.ReadAsStringAsync().Result;
                    return true;
                }
                return false;
            }
        }
        public bool ConnectToServerSignIn(string Uri, User user)
        {
            using (var client = new HttpClient())
            {
                Uri BaseUri = new Uri("http://localhost:52527/api/user");
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                var result = client.PostAsync($"{BaseUri}/checkUserValidation", content).Result;
                if (result.IsSuccessStatusCode == true)
                {
                    var a = result.Content.ReadAsStringAsync().Result;
                    return true;
                }
            }
            return false;
        }
        public bool ConnectToServerInGame(string Uri, User user)
        {
            using (var client = new HttpClient())
            {
                Uri BaseUri = new Uri("http://localhost:52527/api/user");
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                var result = client.PostAsync($"{BaseUri}/inGameUser", content).Result;
                if (result.IsSuccessStatusCode == true)
                {
                    var a = result.Content.ReadAsStringAsync().Result;
                    return true;
                }
                return false;
            }
        }


        public bool AddLossToUser(string Uri, User user)
        {
            using (var client = new HttpClient())
            {
                Uri BaseUri = new Uri("http://localhost:52527/api/user");
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                var result = client.PostAsync($"{BaseUri}/addLossToUser", content).Result;
                if (result.IsSuccessStatusCode == true)
                {
                    var a = result.Content.ReadAsStringAsync().Result;
                    return true;
                }
                return false;
            }
        }
    }
}
