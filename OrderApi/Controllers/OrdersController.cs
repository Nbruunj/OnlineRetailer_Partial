﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Data;
using OrderApi.Models;
using RestSharp;
using static OrderApi.Models.Order;

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

            // Call ProductApi to get the product ordered
            // You may need to change the port number in the BaseUrl below
            // before you can run the request.
            RestClient c = new RestClient("https://localhost:5001/products/");
            if(order.OrderLines.Any())
            {
                foreach(var orderLine in order.OrderLines)
                {
                    var request = new RestRequest(orderLine.ProductId.ToString());
                    var response = c.GetAsync<Product>(request);
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

    }
}
