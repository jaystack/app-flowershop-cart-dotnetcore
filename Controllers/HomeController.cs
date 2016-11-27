using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using App.Flowershop.Cart.Models;
using Newtonsoft.Json;
using App.Flowershop.Cart.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace App.Flowershop.Cart.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptions<Config> config;
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> loggerAccessor, IOptions<Config> optionsAccessor)
        {
            logger = loggerAccessor;
            config = optionsAccessor;
        }

        [HttpGet("/summary")]
        public async Task<IActionResult> Summary()
        {
            var cart = getCartFromCookie();

            var selectedFlowers = await getFlowersByIdAsync(cart.Items.ToArray());

            var data = new CartViewModel()
            {
                CartValue = selectedFlowers.Sum(p => (p.Amount * p.Price)),
                Flowers = selectedFlowers
            };

            setCartCookie(JsonConvert.SerializeObject(cart));

            return PartialView("Summary", data);
        }


        [HttpPost("/checkout")]
        public async Task<IActionResult> CheckoutPost(string customerName, string customerAddress)
        {
            var cart = getCartFromCookie();

            var result = await postAsync("/data/order", new Dictionary<string, string>()
            {
                { "customerName", customerName},
                { "customerAddress", customerAddress },
                { "oids", JsonConvert.SerializeObject(cart.Items) },
            });

            if (result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                setCartCookie(String.Empty);
                return StatusCode(StatusCodes.Status201Created);
            }
            else
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        [HttpGet("/checkout")]
        public async Task<IActionResult> CheckoutGet()
        {
            var cart = getCartFromCookie();

            var selectedFlowers = await getFlowersByIdAsync(cart.Items.ToArray());

            var data = new CartViewModel()
            {
                CartValue = selectedFlowers.Sum(p => (p.Amount * p.Price)),
                Flowers = selectedFlowers
            };

            setCartCookie(JsonConvert.SerializeObject(cart));

            return PartialView("Checkout", data);
        }

        [HttpGet("/add/{id}")]
        public async Task<IActionResult> Add(string id)
        {
            var cart = getCartFromCookie();

            var selectedFlowers = await getFlowersByIdAsync(new string[] { id });

            cart.Items.Add(selectedFlowers.First()._id);
            setCartCookie(JsonConvert.SerializeObject(cart));

            return StatusCode(StatusCodes.Status200OK);
        }

        public IActionResult Error()
        {
            return PartialView();
        }

        private async Task<List<Flower>> getFlowersByIdAsync(string[] ids)
        {
            List<Flower> result = new List<Flower>();

            foreach (var item in ids.GroupBy(p => p))
            {
                var flowerData = await getResponseAsync(String.Format("data/flower({0})", item.Key));

                var flower = JsonConvert.DeserializeObject<Flower>(flowerData);
                flower.Amount = item.Count();
                result.Add(flower);
            }

            return result;
        }

        private async Task<string> getResponseAsync(string url)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(config.Value.DataApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        private async Task<HttpResponseMessage> postAsync(string url, Dictionary<string, string> formContent)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(config.Value.DataApi);

                var content = new FormUrlEncodedContent(formContent.ToArray());

                HttpResponseMessage response = await client.GetAsync(url);

                var result = await client.PostAsync(url, content);

                return result;
            }
        }

        private CartViewModel getCartFromCookie()
        {
            var fs_cart = HttpContext.Request.Cookies["fs_cart"];

            var cart = new CartViewModel();

            if (!String.IsNullOrWhiteSpace(fs_cart))
            {
                logger.LogDebug("actual cart: " + fs_cart);
                cart = JsonConvert.DeserializeObject<CartViewModel>(fs_cart);
            }

            return cart;
        }

        private void setCartCookie(string value)
        {
            HttpContext.Response.Cookies.Append("fs_cart", value);
        }
    }
}
