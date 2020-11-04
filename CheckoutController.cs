using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TwoCheckoutController: ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> Checkout()
        {
            string token = "328aad05-1201-40d1-a275-606f78a2af01";
            try
            {
               var result= await GetResponse(token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.ToString());
            }

           
        }
        private async Task<HttpResponseMessage> GetResponse(string token)
        {
            string sellerId = "250538457371";
            string secretKey = "O)FzpDAa[iJIxh1%@9]Q";
            string date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            string text = $"{sellerId.Length}{sellerId}{date.Length}{date}";
            string hash = CreateMD5Hash(text, secretKey);
            var data = new Rootobject();
            data.PaymentDetails.PaymentMethod.EesToken = token;
            var content = ConvertToJson(data);
            string header = $"code='{sellerId}' date='{date}' hash='{hash}'";
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.avangate.com/rest/6.0/orders/"))
                {
                    request.Headers.Add("Accept", "application/json");
                    request.Headers.Add("X-Avangate-Authentication", header);
                    request.Content = new StringContent(content);
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    var response = await httpClient.SendAsync(request);
                    return response;
                }
            }
        }
        public static String ConvertToJson(object source)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(source, Formatting.None,
                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
        }
        public string CreateMD5Hash(string text, string secret)
        {
            var data = Encoding.UTF8.GetBytes(text);
            var key = Encoding.UTF8.GetBytes(secret);
            var hmac = new HMACMD5(key);
            var hashBytes = hmac.ComputeHash(data);
            return ByteToString(hashBytes);
        }

        static string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); /* hex format */
            }
            return sbinary;
        }

    }
    public class TToken
    {
        public string id { get; set; }
        public string Token { get; set; }
    }
    public class Rootobject
    {
        public Billingdetails BillingDetails { get; set; } = new Billingdetails();
        public string Currency { get; set; } = "SAR";
        public string ExternalReference { get; set; } = "REST_API_AVANGTE";
        public Item[] Items { get; set; } = new Item[] { new Item() };
        public string Language { get; set; } = "en";
        public Paymentdetails PaymentDetails { get; set; } = new Paymentdetails();
    }

    public class Billingdetails
    {
        public string FirstName { get; set; } = "Veeran";
        public string LastName { get; set; } = "Puthumkara";
        public string Address1 { get; set; } = "Albaha";
        public string City { get; set; } = "Albaha";
        public string State { get; set; } = "Albaha";
        public string Zip { get; set; } = "65421";
        public string CountryCode { get; set; } = "SA";
        public string Email { get; set; } = "vkuttyp@gmail.com";
    }

    public class Paymentdetails
    {
        public string Currency { get; set; } = "SAR";
        public Paymentmethod PaymentMethod { get; set; } = new Paymentmethod();
        public string Type => "EES_TOKEN_PAYMENT";
    }

    public class Paymentmethod
    {
        public string EesToken { get; set; }
        public string Vendor3DSReturnURL { get; set; }
        public string Vendor3DSCancelURL { get; set; }
    }

    public class Item
    {
        public string Code { get; set; } = "001";
        public string Quantity { get; set; } = "1";
    }
}

