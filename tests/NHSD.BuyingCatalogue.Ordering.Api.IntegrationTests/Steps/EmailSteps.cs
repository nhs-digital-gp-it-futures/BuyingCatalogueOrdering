using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using Flurl.Http;
using NHSD.BuyingCatalogue.EmailClient.IntegrationTesting.Data;
using NHSD.BuyingCatalogue.EmailClient.IntegrationTesting.Drivers;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

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
            // Email server needs fixing, as has HTML property
            // var actualCount = await _emailServerDriver.GetEmailCountAsync();

            var emailCount = (await FindAllEmailsAsync()).Count;
            emailCount.Should().Be(1);
        }

        // Temp fix
        public async Task<IReadOnlyList<Email>> FindAllEmailsAsync()
        {
            var responseBody = await new Uri("http://localhost:1180/email")
                .AbsoluteUri
                .AppendPathSegment("email")
                .GetJsonAsync<Class1[]>();

            return responseBody.Select(x => new Email
            {
                PlainTextBody = x.text,
                Subject = x.subject,
                From = x.from[0].address,
                To = x.to[0].address
            }).ToList();
        }

        [Then(@"the email sent contains the following information")]
        public async Task ThenEmailContains(Table table)
        {
            var expectedEmail = table.CreateInstance<EmailTable>();
            var actualEmail = (await FindAllEmailsAsync()).FirstOrDefault();
            actualEmail.Should().BeEquivalentTo(expectedEmail);
        }

        private sealed class EmailTable
        {
            public string From { get; set; }

            public string To { get; set; }

            public string Subject { get; set; }
        }




        // Temp classes, need to be removed
        public class Rootobject
        {
            public Class1[] Property1 { get; set; }
        }

        public class Class1
        {
            public string text { get; set; }
            public Headers headers { get; set; }
            public string subject { get; set; }
            public string messageId { get; set; }
            public string priority { get; set; }
            public From1[] from { get; set; }
            public To1[] to { get; set; }
            public DateTime date { get; set; }
            public Attachment[] attachments { get; set; }
            public string id { get; set; }
            public DateTime time { get; set; }
            public bool read { get; set; }
            public Envelope envelope { get; set; }
            public string source { get; set; }
        }

        public class Headers
        {
            public string from { get; set; }
            public string date { get; set; }
            public string subject { get; set; }
            public string messageid { get; set; }
            public string to { get; set; }
            public string mimeversion { get; set; }
            public string contenttype { get; set; }
        }

        public class Envelope
        {
            public From from { get; set; }
            public To[] to { get; set; }
            public string host { get; set; }
            public string remoteAddress { get; set; }
        }

        public class From
        {
            public string address { get; set; }
            public bool args { get; set; }
        }

        public class To
        {
            public string address { get; set; }
            public bool args { get; set; }
        }

        public class From1
        {
            public string address { get; set; }
            public string name { get; set; }
        }

        public class To1
        {
            public string address { get; set; }
            public string name { get; set; }
        }

        public class Attachment
        {
            public string contentType { get; set; }
            public string fileName { get; set; }
            public string contentDisposition { get; set; }
            public string transferEncoding { get; set; }
            public string generatedFileName { get; set; }
            public string contentId { get; set; }
            public string checksum { get; set; }
            public int length { get; set; }
        }


    }
}
