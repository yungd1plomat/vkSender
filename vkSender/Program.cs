namespace vkSender
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Введите токен: ");
            string token = Console.ReadLine();
            Console.Write("Введите текст сообщения");
            string text = Console.ReadLine();
            Sender sender = new Sender();
            await sender.Init(token);
            await sender.SendMessage(text);
            Console.ReadLine();
        }
    }
}