using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace vkSender
{
    public class Sender
    {
        private IVkApi api { get; set; }

        private IList<long> userIds { get; set; }

        private Random rnd { get; set; }

        public Sender()
        {
            api = new VkApi();
            rnd = new Random();
        }

        public async Task Init(string token)
        {
            await api.AuthorizeAsync(new ApiAuthParams() { AccessToken = token });
            await GetUsers();
        }

        public async Task GetUsers()
        {
            userIds = new List<long>();
            var count = api.Messages.GetConversations(new GetConversationsParams()
            {
                Count = 0
            }).Count;
            for (long offset = 0; offset < (count - 1) / 200 + 1; offset++)
            {
                var peers = await api.Messages.GetConversationsAsync(new GetConversationsParams()
                {
                    Count = 200,
                    Offset = (ulong)offset * 200,
                });
                foreach (var item in peers.Items)
                {
                    userIds.Add(item.Conversation.Peer.Id);
                }
                double percentage = offset / (count - 1) / 200 + 1;
                Console.WriteLine($"Получение пользователей, выполнено {percentage} %");
            }
            Console.WriteLine($"Получено {userIds.Count()} пользователей");
        }

        public async Task SendMessage(string text)
        {
            var chunks = userIds.Chunk(100);
            foreach (var chunk in chunks)
            {
                await api.Messages.SendToUserIdsAsync(new MessagesSendParams()
                {
                    Message = text,
                    UserIds = chunk,
                    RandomId = rnd.Next(),
                });
                Console.WriteLine($"[+] Выслали сообщение {chunk.Length} юзерам");
            }
            Console.WriteLine("Рассылка завершена");
        }
    }
}
