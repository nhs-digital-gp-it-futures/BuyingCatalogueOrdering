using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using System.Net.Mime;
using System.Text;
using NHSD.BuyingCatalogue.EmailClient.IntegrationTesting.Data;
using NHSD.BuyingCatalogue.EmailClient.IntegrationTesting.Drivers;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using static System.String;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class EmailSteps
    {
        private readonly EmailServerDriver _emailServerDriver;

        public EmailSteps(EmailServerDriver emailServerDriver)
        {
            _emailServerDriver = emailServerDriver ?? throw new ArgumentNullException(nameof(emailServerDriver));
        }

        [Then(@"only one email is sent")]
        public async Task OnlyOneEmailIsSent()
        {
            var emailCount = (await _emailServerDriver.FindAllEmailsAsync()).Count;
            emailCount.Should().Be(1);
        }

        [Then(@"the email sent contains the following information")]
        public async Task ThenEmailContains(Table table)
        {
            var expectedContents = table.CreateSet<EmailContents>().First();

            var expected = new
            {
                Attachments = new List<EmailAttachmentData>
                {
                    new EmailAttachmentData(
                        Encoding.UTF8.GetBytes($"{expectedContents.AttachmentHeader1}{Environment.NewLine}{expectedContents.OrderId}{Environment.NewLine}"),
                        expectedContents.FileName,
                        new ContentType("text/csv"))
                },
                From = new List<EmailAddress>
                {
                    new EmailAddress(Empty, expectedContents.From)
                },
                expectedContents.Text,
                expectedContents.Subject,
                To = new List<EmailAddress>
                {
                    new EmailAddress(Empty, expectedContents.To)
                }
            };
            
            var actual = (await _emailServerDriver.FindAllEmailsAsync()).First();
            expected.Should().BeEquivalentTo(actual, conf => conf.Excluding(x => x.Html));

        }

        private sealed class EmailContents
        {
            public string From { get; set; }
            public string To { get; set; }
            public string Subject { get; set; }
            public string Text { get; set; }
            public string FileName { get; set; }
            public string AttachmentHeader1 { get; set; }
            public string OrderId { get; set; }
        }
    }
}
