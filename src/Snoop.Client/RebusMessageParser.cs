using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rebus.Compression;
using Rebus.Messages;

namespace Snoop.Client
{
    public static class RebusMessageParser
    {
        public static MessageViewModel ParseMessage(MessageQueryModel message)
        {
            try
            {
                var couldDeserializeHeaders = DeserializeHeaders(message, out var headers);

                if (!couldDeserializeHeaders) return null;

                var couldDecodeBody = TryDecodeBody(message, headers, out var body);

                if (!couldDecodeBody) return null;

                //remove ErrorDetails from headers as we assign this to its own property
                var headerViewModels = headers
                    .Where(x => x.Key != Headers.ErrorDetails)
                    .Select(item => new MessageHeaderViewModel(item.Key, item.Value))
                    .ToList();

                var sentTimeString = headers[Headers.SentTime];
                sentTimeString = sentTimeString.Substring(0, sentTimeString.LastIndexOf(':'));

                DateTimeOffset.TryParse(sentTimeString, out var sentTime);
                headers.TryGetValue(Headers.ErrorDetails, out var errors);

                return new MessageViewModel(message.Id, headerViewModels, headers[Headers.Type], headers[Headers.ReturnAddress], sentTime, body, errors);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static bool DeserializeHeaders(MessageQueryModel message, out Dictionary<string, string> dictionary)
        {
            try
            {
                var headersAsJsonString = Encoding.UTF7.GetString(message.Headers);
                var headers = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsJsonString);
                dictionary = headers;
                return true;
            }
            catch
            {
                dictionary = null;
                return false;
            }
        }

        private static bool TryDecodeBody(MessageQueryModel message, Dictionary<string, string> headers, out string body)
        {
            try
            {
                if (headers == null)
                {
                    body = "Message has no headers that can be understood by Rebus";
                    return false;
                }

                if (!headers.ContainsKey(Headers.ContentType))
                {
                    body = string.Format("Message headers don't contain an element with the '{0}' key", Headers.ContentType);

                    return false;
                }

                var destination = new MemoryStream(message.Body);


                var bytes = destination.ToArray();

                var bodyIsGzipped = headers.ContainsKey(Headers.ContentEncoding) &&
                                    string.Equals(headers[Headers.ContentEncoding], "gzip", StringComparison.InvariantCultureIgnoreCase);

                if (bodyIsGzipped)
                {
                    bytes = new Zipper().Unzip(bytes);
                }

                var encoding = headers[Headers.ContentType];
                var encoder = GetEncoding(encoding);
                var str = encoder.GetString(bytes);

                body = FormatJson(str);

                return true;
            }
            catch (Exception e)
            {
                body = string.Format("An error occurred while decoding the body: {0}", e);
                return false;
            }
        }

        private static Encoding GetEncoding(string contentType)
        {
            var contentTypeSettings = contentType
                .Split(';')
                .Select(token => token.Split('='))
                .Where(tokens => tokens.Length == 2)
                .ToDictionary(tokens => tokens[0], tokens => tokens[1], StringComparer.InvariantCultureIgnoreCase);

            var encoding = contentTypeSettings.ContainsKey("charset")
                ? Encoding.GetEncoding(contentTypeSettings["charset"])
                : Encoding.ASCII;

            return encoding;
        }

        private static string FormatJson(string input)
        {
            try
            {
                return JsonConvert.SerializeObject(JsonConvert.DeserializeObject<JObject>(input), Formatting.Indented);
            }
            catch
            {
                return input;
            }
        }
    }
}