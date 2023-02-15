using NewsparkWiremockDotNetDeepdive.Models;
using System;
using System.Collections.Generic;

namespace NewsparkWiremockDotNetDeepdive.SwaggerPetStoreExamples
{
    public class CreateDummyTestData
    {
        public List<Pet> GenerateDummyData(int numberOfPets)
        {
            Console.WriteLine($"Create dummy test data with {numberOfPets} animals:");
            List<Pet> dummyData = new List<Pet>();

            for (int i = 0; i < numberOfPets; i++)
            {
                dummyData.Add(new Pet
                {
                    Id = 1000 + i,
                    Category = new Category
                    {
                        Id = 2000 + i,
                        Name = $"Lorem Ipsum {i}"
                    },
                    Name = $"doggie {i}",
                    PhotoUrls = new [] {
                        "dummy content",
                        "another dummy content"
                    },
                    Tags = new [] {
                        new Tag {
                            Id = 57400229,
                            Name = "reprehenderit nisi irure ipsum"
                        },
                        new Tag {
                            Id = 2935902,
                            Name = "dolor Excepteur labore proident sint"
                        }
                    },
                    Status = "pending"
                });
                Console.WriteLine($"Created animal {i} has name: '{dummyData[i].Name}'");
            }       

            return dummyData;
        }
    }
}
