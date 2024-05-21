﻿using ElinorStoreServer.Data.Domain;
using ElinorStoreServer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using share.Models.Basket;
using share.Models.Order;
using share.Models.Product;
using share.Models.User;

namespace ElinorStoreServer.Services
{
    public class BasketService
    {
        private readonly StoreDbContext _context;
        private List<Basket> basket;

        public int ProductId { get; private set; }

        public BasketService(StoreDbContext context)
        {
            _context = context;
        }
        public async Task<Basket?> GetAsync(int id)
        {
            Basket? basket = await _context.Baskets.FindAsync(id);
            return basket;
        }
        public async Task<List<Basket>> GetsAsync()
        {
            List<Basket> baskets = await _context.Baskets.ToListAsync();
            return baskets;
        }
        public async Task<List<Basket>> GetsByProductAsync(int productId)
        {
            List<Basket> baskets = await _context.Baskets.Where(basket => basket.ProductId == ProductId).ToListAsync();
            return basket;
        }
        public async Task<List<Basket>> GetsByUserAsync(int userId)
        {
            List<Basket> baskets = await _context.Baskets.Where(basket => basket.UserId == userId).ToListAsync();
            return basket;
        }
        public async Task AddAsync(BasketAddRequestDto model)
        {
            Basket basket = new Basket
            {
                UserId = model.UserId,
                Count = model.Count,
                ProductId = model.ProductId,
            };

            _context.Baskets.Add(basket);
            await _context.SaveChangesAsync();
        }
        public async Task EditAsync(Basket basket)
        {
            Basket? oldBasket = await _context.Baskets.FindAsync(basket.Id);
            if (oldBasket is null)
            {
                throw new Exception("سبد خریدی  با این شناسه پیدا نشد.");
            }
            oldBasket.Count = basket.Count;
            oldBasket.ProductId = basket.ProductId;
            oldBasket.UserId = basket.UserId;

            _context.Baskets.Update(oldBasket);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            Basket? basket = await _context.Baskets.FindAsync(id);
            if (basket is null)
            {
                throw new Exception("سبد خریدی  با این شناسه پیدا نشد.");
            }
            _context.Baskets.Remove(basket);
            await _context.SaveChangesAsync();
        }

        internal async Task AddAsync(Basket basket)
        {
            throw new NotImplementedException();
        }

       public async Task<List<BasketSearchResponseDto>> SearchAsync(BasketSearchRequestDto model)
        {
            IQueryable<Basket> baskets = _context.Baskets
                                .Where(a =>
                                (model.Count == null || a.Count <= model.Count)
                               /* && (model.FromDate == null || a.CreatedAt >= model.FromDate)
                                && (model.ToDate == null || a.CreatedAt <= model.ToDate)*/
                               && (model.UserName == null || a.User.Name.Contains(model.UserName))
                               && (model.ProductName == null || a.Product.Name.Contains(model.ProductName))
                                );
            if (!string.IsNullOrEmpty(model.SortBy))
            {
                switch (model.SortBy)
                {
                    case "CountAsc":
                        baskets = baskets.OrderBy(a => a.Count);
                        break;
                    case "CountDesc":
                        baskets = baskets.OrderByDescending(a => a.Count);
                        break;
                }
            }

            baskets = baskets.Skip(model.PageNo * model.PageSize).Take(model.PageSize);

            var searchResults = await baskets
                                .Select(a => new BasketSearchResponseDto
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
            return searchResults;
        }


   /*     public async Task<List<share.Models.Basket.BasketReportByProductResponseDto>> BaskerReportByUserIdAsync(BasketReportByProductRequestDto model)
        {
            var BasketsQuery = _context.Baskets.Where(a =>
                                model.UserId == null || a.User.Id == model.UserId

                                )
                .GroupBy(a => a.UserId)
                .Select(a => new
                {
                    UserId = a.Key,
                    TotalSum = a.Count(s => s.ProductId)
                });

            var productsQuery = from product in _context.Products
                                from Basket in BasketsQuery.Where(a => a.UserId == User.Id).DefaultIfEmpty()
                                select new BasketReportByProductResponseDto
                                {
                                    ProductName = product.Name,
                                    ProductCategoryName = product.Category.Name,
                                    ProductId = product.Id,
                                    TotalSum = (int?)order.TotalSum
                                };

            productsQuery = productsQuery.Skip(model.PageNo * model.PageSize)
                                .Take(model.PageSize);
            var result = await productsQuery.ToListAsync();
            return result;
        }*/
    }
}
