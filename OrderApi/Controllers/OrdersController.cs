using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Data;
using OrderApi.Models;
using RestSharp;
using shardmodels;
using Order = OrderApi.Models.Order;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<Order> repository;

        public OrdersController(IRepository<Order> repos)
        {
            repository = repos;
        }

        // GET: orders
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return repository.GetAll();
        }

        // GET orders/5
        [HttpGet("{id}", Name = "GetOrder")]
        public IActionResult Get(int id)
        {
            var item = repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST orders
        [HttpPost]
        public IActionResult Post([FromQuery]Order order)
        {
            if (order == null)
            {
                return BadRequest();
            }

            if(!CheckCustomerCreditStanding(order.customerId))
            {
                return BadRequest("Customer not found or has an unpaid bill)");
            }

            // Call ProductApi to get the product ordered
            // You may need to change the port number in the BaseUrl below
            // before you can run the request.
            RestClient c = new RestClient("https://localhost:5001/products/");
            if(order.OrderLines.Any())
            {
                foreach(var orderLine in order.OrderLines)
                {
                    var request = new RestRequest(orderLine.ProductId.ToString());
                    var response = c.GetAsync<shardmodels.Product>(request);
                    response.Wait();
                    var orderedProduct = response.Result;

                    if (orderLine.Quantity <= orderedProduct.ItemsInStock - orderedProduct.ItemsReserved)
                    {
                        // reduce the number of items in stock for the ordered product,
                        // and create a new order.
                        orderedProduct.ItemsReserved += orderLine.Quantity;
                        var updateRequest = new RestRequest(orderedProduct.Id.ToString());
                        updateRequest.AddJsonBody(orderedProduct);
                        var updateResponse = c.PutAsync(updateRequest);
                        updateResponse.Wait();

                        if (updateResponse.IsCompletedSuccessfully)
                        {
                            UpdateCreditStanding(order.customerId);

                            var newOrder = repository.Add(order);
                            return CreatedAtRoute("GetOrder",
                                new { id = newOrder.Id }, newOrder);
                        }
                    }
                }
            }
            // If the order could not be created, "return no content".
            return NoContent();
        }
        [NonAction]
        public bool CheckCustomerCreditStanding(int? id)
        {
            RestClient c = new RestClient("https://localhost:7082/customer/");
            var request = new RestRequest(id.ToString());
            var response = c.Execute<Customer>(request);

            if(response.Data != null)
            {
                return response.Data.CreditStanding;
            }
            else
            {
                return false;
            }
        }
        [NonAction]
        public void UpdateCreditStanding(int? id)
        {
            RestClient c = new RestClient("https://localhost:7082/customer/");
            Customer updateCustomer = new Customer()
            {
                Id = id,
                CreditStanding = false
            };
            var updateCustomerRequest = new RestRequest(id.ToString());
            updateCustomerRequest.AddJsonBody(updateCustomer);
            c.Execute(updateCustomerRequest);
        }

    }
}
