using Azure;
using Azure.AI.TextAnalytics;
using System;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;
namespace FinalYearProject.Models
{
    public class AzureAI
    {

        private static readonly AzureKeyCredential credentials = new AzureKeyCredential("Bx0zKbvpvYnzZaKQe7pPF7ujcJvPaTstmRbnBsN7MxKsmc3uqgiiJQQJ99BDACmepeSXJ3w3AAAaACOG1VyL");
        private static readonly Uri endpoint = new Uri("https://fashion-warehouse-speech.cognitiveservices.azure.com/");

        public string GetKeywords()
        {
            string KeywordToReturn = "";

            var client = new TextAnalyticsClient(endpoint, credentials);
            // You will implement these methods later in the quickstart.

            EntityRecognitionExample(client);
            EntityLinkingExample(client);
            RecognizePIIExample(client);
            KeyPhraseExtractionExample(client);


            return KeywordToReturn;

        }

        public string GetKeywords(string searchterm)
        {
            string KeywordToReturn = "";

            var client = new TextAnalyticsClient(endpoint, credentials);
            // You will implement these methods later in the quickstart.

            KeywordToReturn = KeyPhraseExtractionExample(client, searchterm);


            return KeywordToReturn;

        }

        static void EntityRecognitionExample(TextAnalyticsClient client)
        {
            var response = client.RecognizeEntities("I had a wonderful trip to Seattle last week.");
            System.Diagnostics.Debug.WriteLine("Named Entities:");
            foreach (var entity in response.Value)
            {
                System.Diagnostics.Debug.WriteLine($"\tText: {entity.Text}\tCategory: {entity.Category}\tSub-Category: {entity.SubCategory}");
                System.Diagnostics.Debug.WriteLine($"\t\tScore: {entity.ConfidenceScore:F2}\tLength: {entity.Length}\tOffset: {entity.Offset}\n");
            }
        }

        static void RecognizePIIExample(TextAnalyticsClient client)
        {
            string document = "A developer with SSN 859-98-0987 whose phone number is 800-102-1100 is building tools with our APIs.";

            PiiEntityCollection entities = client.RecognizePiiEntities(document).Value;

            System.Diagnostics.Debug.WriteLine($"Redacted Text: {entities.RedactedText}");
            if (entities.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"Recognized {entities.Count} PII entit{(entities.Count > 1 ? "ies" : "y")}:");
                foreach (PiiEntity entity in entities)
                {
                    System.Diagnostics.Debug.WriteLine($"Text: {entity.Text}, Category: {entity.Category}, SubCategory: {entity.SubCategory}, Confidence score: {entity.ConfidenceScore}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No entities were found.");
            }
        }


        static void EntityLinkingExample(TextAnalyticsClient client)
        {
            var response = client.RecognizeLinkedEntities(
                "Microsoft was founded by Bill Gates and Paul Allen on April 4, 1975 " +
                "to develop and sell BASIC interpreters for the Altair 8800. " +
                "During his career at Microsoft, Gates held the positions of chairman " +
                "chief executive officer, president and chief software architect " +
                "while also being the largest individual shareholder until May 2014.");
            System.Diagnostics.Debug.WriteLine("Linked Entities:");
            foreach (var entity in response.Value)
            {
                System.Diagnostics.Debug.WriteLine($"\tName: {entity.Name}\tID: {entity.DataSourceEntityId}\tURL: {entity.Url}\tData Source: {entity.DataSource}");
                System.Diagnostics.Debug.WriteLine("\tMatches:");
                foreach (var match in entity.Matches)
                {
                    System.Diagnostics.Debug.WriteLine($"\t\tText: {match.Text}");
                    System.Diagnostics.Debug.WriteLine($"\t\tScore: {match.ConfidenceScore:F2}");
                    System.Diagnostics.Debug.WriteLine($"\t\tLength: {match.Length}");
                    System.Diagnostics.Debug.WriteLine($"\t\tOffset: {match.Offset}\n");
                }
            }
        }

        static void KeyPhraseExtractionExample(TextAnalyticsClient client)
        {
            //var response = client.ExtractKeyPhrases("My cat might need to see a veterinarian.");
            var response = client.ExtractKeyPhrases("show me all blue tshirts for men");

            // Printing key phrases
            System.Diagnostics.Debug.WriteLine("Key phrases:");

            foreach (string keyphrase in response.Value)
            {
                System.Diagnostics.Debug.WriteLine($"\t{keyphrase}");
            }
        }

        static string KeyPhraseExtractionExample(TextAnalyticsClient client, string searchterm)
        {
            //var response = client.ExtractKeyPhrases("My cat might need to see a veterinarian.");
            var response = client.ExtractKeyPhrases(searchterm);

            string refinedkeyword = "";

            // Printing key phrases
            System.Diagnostics.Debug.WriteLine("Key phrases:");

            foreach (string keyphrase in response.Value)
            {
                refinedkeyword += keyphrase;
                System.Diagnostics.Debug.WriteLine($"\t{keyphrase}");
            }
            return refinedkeyword;
        }

    }
}
