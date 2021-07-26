﻿using Thankify.Core.Domain.Category.Dto;

namespace Thankify.Api.Model.V1
{
    public class CategoryDetailViewModel : CategoryViewModel
    {
        public int TotalGratitudes { get; set; }

        public static CategoryDetailViewModel Parse(CategoryDetailDto categoryDetailDto)
        {
            return new CategoryDetailViewModel
            {
                Id = categoryDetailDto.Id,
                Title = categoryDetailDto.Title,
                TotalGratitudes = categoryDetailDto.TotalGratitudes
            };
        }
    }
}