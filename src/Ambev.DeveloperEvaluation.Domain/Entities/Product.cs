﻿namespace Domain.Entities
{
    public class Product
    {

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
    }
}
