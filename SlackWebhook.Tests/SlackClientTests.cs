﻿using SlackWebhook.Enums;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace SlackWebhook.Tests
{
    public class SlackClientTests
    {
        private const string WebhookFileName = "webhookurl.txt";
        private const string WebhookEnvironmentName = "WEBHOOKURL";

        [Fact]
        public async Task Integration_Test()
        {
            var webhookUrl = await GetWebhookUrlAsync();
            Assert.NotNull(webhookUrl);

            await new SlackClient(webhookUrl).SendAsync(b => b
                .WithText("Hello from *SlackWebhook*")
                .WithUsername("SlackWebhook")
                .WithIcon(IconType.Url, "https://raw.githubusercontent.com/micdah/SlackWebhook/master/icon.png")
                .WithAttachment(a => a
                    .WithTitle("How to install")
                    .WithText("`PM> Install-Package SlackWebhook`")
                    .WithColor(Color.DarkSlateBlue))
                .WithAttachment(a => a
                    .WithTitle("Find out more")
                    .WithText("Find out more by taking a look at github.com/micdah/SlackWebhook")
                    .WithLink("https://github.com/micdah/SlackWebhook")
                    .WithField(
                        "Use builder pattern",
                        "```\n" +
                        "await slackClient.SendASync(b => b\n" +
                        "   .WithUsername(\"My Bot\")\n" +
                        "   .WithText(\"Hello *World*\"));\n" +
                        "```")
                    .WithField(
                        "Use object initializer",
                        "```\n" +
                        "await slackClient.SendAsync(new SlackMessage {\n" +
                        "   Username = \"My Bot\",\n" +
                        "   Text = \"Hello *World*\"\n" +
                        "});\n" +
                        "```"))
                .WithAttachment(a => a
                    .WithTitle("Now with actions")
                    .WithText("...which are pretty neat")
                    .WithLinkButtonAction("Click me", "http://google.com", ActionStyle.Primary)
                    .WithLinkButtonAction("Don't click me", "http://bing.com", ActionStyle.Danger))
            );
        }

        [Fact]
        public async Task Test_Sending_To_Other_Channel()
        {
            var webhookUrl = await GetWebhookUrlAsync();
            Assert.NotNull(webhookUrl);

            await new SlackClient(webhookUrl).SendAsync(b => b
                .WithText("Testing sending to different channel, than the one configured by the webhook URL")
                .WithUsername("SlackWebhook")
                .WithIcon(IconType.Url, "https://raw.githubusercontent.com/micdah/SlackWebhook/master/icon.png")
                .WithChannel("#other-test"));
        }

        private static async Task<string> GetWebhookUrlAsync()
        {
            var file = GetFilePath(WebhookFileName);
            if (File.Exists(file))
            {
                return await File.ReadAllTextAsync(file);
            }

            return Environment.GetEnvironmentVariable(WebhookEnvironmentName);
        }

        private static string GetFilePath(string filename)
        {
            var baseDirectory = AppContext.BaseDirectory;
            // added because of test run issues on MacOS
            var indexOfBin = baseDirectory.LastIndexOf("bin", StringComparison.OrdinalIgnoreCase);
            var connectionStringFileDirectory =
                baseDirectory.Substring(0, (indexOfBin > 0) ? indexOfBin : baseDirectory.Length);
            return Path.Combine(connectionStringFileDirectory, filename);
        }
    }
}