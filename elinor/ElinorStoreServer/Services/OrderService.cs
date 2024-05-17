﻿using ElinorStoreServer.Data.Domain;
using ElinorStoreServer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using share.Models.Order;


namespace ElinorStoreServer.Services
{
    public class OrderService
    {
        private readonly StoreDbContext _context;
        private List<Order> order;

        public int ProductId { get; private set; }

        public OrderService(StoreDbContext context)
        {
            _context = context;
        }
        public async Task<Order?> GetAsync(int id)
        {
            Order? order = await _context.Orders.FindAsync(id);
            return order;
        }
        public async Task<List<Order>> GetsAsync()
        {
            List<Order> orders = await _context.Orders.ToListAsync();
            return orders;
        }
        public async Task<List<Order>> GetsByProductAsync(int productId)
        {
            List<Order> orders = await _context.Orders.Where(order => order.ProductId == productId).ToListAsync();
            return order;
        }
        public async Task<List<Order>> GetsByUserAsync(int userId)
        {
            List<Order> orders = await _context.Orders.Where(order => order.UserId == userId).ToListAsync();
            return order;
        }
        public async Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
         
        }

        public async Task AddRangeAsync(List<OrderAddRequestDto> orders)
        {
            var order = orders.Select(orderDto => new Order
            {

                Count = orderDto.Count,
                Price = orderDto.Price,
                ProductId = orderDto.ProductId,
                UserId = orderDto.UserId,
            
            }).ToList();

            _context.Orders.AddRange(order);
            await _context.SaveChangesAsync();
        }





        public async Task EditAsync(Order order)
        {
            Order? oldOrder = await _context.Orders.FindAsync(order.Id);
            if (oldOrder is null)
            {
                throw new Exception("سفارشی  با این شناسه پیدا نشد.");
            }
            oldOrder.Count = order.Count;
            oldOrder.Price = order.Price; 
            oldOrder.ProductId = order.ProductId;
            oldOrder.UserId = order.UserId;

            _context.Orders.Update(oldOrder);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Order? order = await _context.Orders.FindAsync(id);
            if (order is null)
            {
                throw new Exception("سفارشی  با این شناسه پیدا نشد.");
            }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderSearchResponseDto>> SearchAsync(OrderSearchRequestDto model)
        {
            var Orders = await _context.Orders
                                .Where(a =>
                                (model.Count == null || a.Count <= model.Count)
                               /* && (model.FromDate == null || a.CreatedAt >= model.FromDate)
                                && (model.ToDate == null || a.CreatedAt <= model.ToDate)*/
                               && (model.UserName == null || a.User.Name.Contains(model.UserName))
                               && (model.ProductName == null || a.Product.Name.Contains(model.ProductName))
                                )
                                .Skip(model.PageNo * model.PageSize)
                                .Take(model.PageSize)
                                .Select(a => new OrderSearchResponseDto
                                {
                                    ProductId = a.Product.Id,
                                    UserName = a.User.Name,
                                    ProductName = a.Product.Name,
                                    count = a.Count,
                                    Price = a.Product.Price,
                                    /*  CreatedAt = a.CreatedAt,*/
                                    Description = a.Product.Description
                                }
                )
                                .ToListAsync();
            return Orders;
        }

    }
}