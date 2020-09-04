using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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

        [Then(@"no email is sent")]
        public async Task EmailIsNotSent()
        {
            var emails = await _emailServerDriver.FindAllEmailsAsync();
            emails.Should().BeNullOrEmpty();
        }

        [Then(@"the email contains the following information")]
        public async Task ThenTheEmailContainsTheFollowingInformation(Table table)
        {
            var expectedContents = table.CreateSet<EmailContentsTable>().First();

            var expected = new
            {
                From = new List<EmailAddress> { new EmailAddress(Empty, expectedContents.From) },
                To = new List<EmailAddress> { new EmailAddress(Empty, expectedContents.To) },
                expectedContents.Text,
                expectedContents.Subject,
            };

            var actual = (await _emailServerDriver.FindAllEmailsAsync()).First();

            actual.Should().BeEquivalentTo(expected, conf => conf.IncludingAllDeclaredProperties());
        }

        [Then(@"the email contains the following attachments")]
        public async Task ThenTheEmailContainsTheFollowingAttachments(Table table)
        {
            var expected = table.CreateSet<Attachments>();

            var actual = (await _emailServerDriver.FindAllEmailsAsync()).First().Attachments;
            actual.Select(x => x.FileName).Should().BeEquivalentTo(expected.Select(x => x.Filename));
        }
        
        private sealed class EmailContentsTable
        {
            public string From { get; set; }
            public string To { get; set; }
            public string Subject { get; set; }
            public string Text { get; set; }
        }

        private sealed class Attachments
        {
            public string Filename { get; set; }
        }
    }
}
