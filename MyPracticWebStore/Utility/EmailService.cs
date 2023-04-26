﻿using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace MyPracticWebStore.Utility
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Restaurant Order", "maks.hokey@yandex.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.yandex.ru", 465, true);
                await client.AuthenticateAsync("maks.hokey@yandex.ru", "war_man333");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        } 
    }
}
