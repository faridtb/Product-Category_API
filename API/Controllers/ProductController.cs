using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ProductController : BaseApiController
    {
        private readonly DataContext _context;
        public ProductController(DataContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
           return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            return await _context.Products.FindAsync(id);
        }
        
        

        [HttpPost("create")]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            if(await ProductExist(product.Name)) return BadRequest("This Product already in database");

            var pro = new Product
            {
                Name = product.Name.ToLower()
            };

            _context.Products.Add(pro);

            await _context.SaveChangesAsync();

            return pro;
        }
        private async Task<bool> ProductExist(string productName)
        {
            return await _context.Products.AnyAsync(x=>x.Name == productName.ToLower());
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveProduct(int id)
        {
            var pro = _context.Products.Find(id);

            if(pro == null) return BadRequest($"Cant Find any product in that {id}");
            
            _context.Products.Remove(pro);

            await _context.SaveChangesAsync();

            return Ok("Product Deleted from Database");
        }

    }
}