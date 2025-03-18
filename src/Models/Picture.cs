// filepath: c:\Dev\CatDogBearMicroservice\src\Models\Picture.cs
using System;

namespace CatDogBearMicroservice.Models
{
    public class Picture
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string AnimalType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}