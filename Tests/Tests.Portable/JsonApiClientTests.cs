namespace codingfreaks.cfUtils.Tests.Portable
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;

    using Logic.Portable.Utilities;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Drawing;
    using System.IO;

    [TestClass]
    public class JsonApiClientTests
    {

        private readonly Uri _issTrackerEndpoint = new Uri("https://api.wheretheiss.at/v1/");

        /// <summary>
        /// Checks the functionallity of the method <see cref="JsonApiClient.GetWithResultAsync{TResult}(string,System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{string,string}})"/>
        /// </summary>
        [TestMethod]
        public async Task ApiClientGetTest()
        {
            var client = JsonApiClient.GetInstance(_issTrackerEndpoint);
            var result = await client.GetWithResultAsync<object>("satellites/25544");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task ApliClientUploadImageTest()
        {
            var headers = new Dictionary<string, string>()
            {
                { "Ocp-Apim-Trace", "true" },
                { "Ocp-Apim-Subscription-Key", "8O9gL7qOphqyiuHCmv6Y" },
                { "token", "8O9gL7qOphqyiuHCmv6Y" },
                { "client-version", "99.99.99.99" },
                { "culture", CultureInfo.CurrentUICulture.Name },
                { "os-version", "A|?" },
                { "device-model", "A|?" },
                { "utc-offset-seconds", DateTimeOffset.UtcNow.Offset.TotalSeconds.ToString(CultureInfo.InvariantCulture) },
                { "device-identifier", Guid.NewGuid().ToString("N") },
                { "login", "schmidt@devdeer.com" }
            };
            var client = JsonApiClient.GetInstance(headers, new Uri("https://befit-test.azurewebsites.net/api/"));
            var imageData = ImageToByteArray(Image.FromFile(@"C:\Users\AlexanderSchmidt\Pictures\Saved Pictures\me_01.png"));
            var result = await client.UploadBytesAsync<bool>($"Profile/Image", "Test", imageData);
            Assert.IsTrue(result);
        }

        public static byte[] ImageToByteArray(System.Drawing.Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

    }
}
