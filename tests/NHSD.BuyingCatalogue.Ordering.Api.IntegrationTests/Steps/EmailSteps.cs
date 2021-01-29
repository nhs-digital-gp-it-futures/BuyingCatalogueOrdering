using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using NHSD.BuyingCatalogue.EmailClient.IntegrationTesting.Data;
using NHSD.BuyingCatalogue.EmailClient.IntegrationTesting.Drivers;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class EmailSteps
    {
        private readonly EmailServerDriver emailServerDriver;

        public EmailSteps(EmailServerDriver emailServerDriver)
        {
            this.emailServerDriver = emailServerDriver ?? throw new ArgumentNullException(nameof(emailServerDriver));
        }

        [Then(@"only one email is sent")]
        public async Task OnlyOneEmailIsSent()
        {
            var emailCount = (await emailServerDriver.FindAllEmailsAsync()).Count;
            emailCount.Should().Be(1);
        }

        [Then(@"no email is sent")]
        public async Task EmailIsNotSent()
        {
            var emails = await emailServerDriver.FindAllEmailsAsync();
            emails.Should().BeNullOrEmpty();
        }

        [Then(@"the email contains the following information")]
        public async Task ThenTheEmailContainsTheFollowingInformation(Table table)
        {
            var expectedContents = table.CreateSet<EmailContentsTable>().First();

            var expected = new
            {
                From = new List<EmailAddress> { new(expectedContents.FromName, expectedContents.FromAddress) },
                To = new List<EmailAddress> { new(expectedContents.ToName, expectedContents.ToAddress) },
                expectedContents.Text,
                expectedContents.Subject,
            };

            var actual = (await emailServerDriver.FindAllEmailsAsync())[0];

            actual.Should().BeEquivalentTo(expected, conf => conf.IncludingAllDeclaredProperties());
        }

        [Then(@"the email contains the following attachments")]
        public async Task ThenTheEmailContainsTheFollowingAttachments(Table table)
        {
            var expected = table.CreateSet<Attachments>();

            var actual = (await emailServerDriver.FindAllEmailsAsync())[0].Attachments;
            actual.Select(a => a.FileName).Should().BeEquivalentTo(expected.Select(a => a.Filename));
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class EmailContentsTable
        {
            public string FromAddress { get; init; }

            public string FromName { get; init; }

            public string ToAddress { get; init; }

            public string ToName { get; init; }

            public string Subject { get; init; }

            public string Text { get; init; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.Members)]
        private sealed class Attachments
        {
            public string Filename { get; init; }
        }
    }
}
