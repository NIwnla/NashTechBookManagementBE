using NashTechProjectBE.Domain.Entities;
using NashTechProjectBE.Infractructure.Context;

namespace NashTechProjectBE.Web.Extensions;

public static class ApplicationExtension
{
    public static IApplicationBuilder SeedData(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
            if (!dbContext.Categories.Any())
            {
                var categories = new[]
                {
                    new Category { Name = "Fiction" },
                    new Category { Name = "Non-Fiction" },
                    new Category { Name = "Science" },
                    new Category { Name = "History" },
                    new Category { Name = "Biography" }
                };
                dbContext.Categories.AddRange(categories);
                dbContext.SaveChanges();

            }

            // Check if there are any books, if not, seed the database
            if (!dbContext.Books.Any())
            {
                var random = new Random();
                var categories = dbContext.Categories.ToList();

                var titles = new[]
                                {
                                        "The Great Adventure",
                                        "Mystery of the Old House",
                                        "The Science of Everything",
                                        "Historical Legends",
                                        "Life of a Genius",
                                        "Journey to the Unknown",
                                        "The Secrets of Space",
                                        "Memoirs of a Traveler",
                                        "The Lost City",
                                        "Tales of the Ancient World",
                                        "The Last Frontier",
                                        "Wisdom of the Ages",
                                        "The Art of War",
                                        "Secrets of the Deep",
                                        "Voyage of Discovery",
                                        "The Enchanted Forest",
                                        "A Glimpse of the Future",
                                        "The Chronicles of Time",
                                        "Echoes of the Past",
                                        "The Innovator's Dilemma",
                                        "Legends of the Sea",
                                        "The Eternal Quest",
                                        "Stories from the Heart",
                                        "The Realm of Fantasy",
                                        "Adventures in Wonderland",
                                        "The Edge of Reality",
                                        "The Whispering Wind",
                                        "Guardians of the Galaxy",
                                        "The Unseen World",
                                        "The Quest for Knowledge",
                                        "Mysteries of the Mind",
                                        "The Hidden Treasures",
                                        "The Journey Within",
                                        "Tales of Valor",
                                        "The Art of Exploration",
                                        "Songs of the Soul",
                                        "The Final Countdown",
                                        "Paths to Enlightenment",
                                        "The Lost Chronicles",
                                        "Visions of the Future",
                                        "The World Beyond",
                                        "Echoes of Eternity",
                                        "The Masterpiece",
                                        "The Grand Illusion",
                                        "The Odyssey",
                                        "The Explorer's Handbook",
                                        "Journey Through Time",
                                        "The Last Expedition",
                                        "Guardians of Wisdom"
                                    };

                for (int i = 1; i <= 50; i++)
                {
                    var book = new Book
                    {
                        Title = titles[i % titles.Length],
                        Body = $"This is the description of {titles[i % titles.Length]}.",
                        Quantity = random.Next(1, 6), // Quantity between 1 and 5
                        Categories = categories.OrderBy(c => random.Next()).Take(random.Next(1, 4)).ToList() // Assign 1-3 random categories
                    };

                    dbContext.Books.Add(book);
                }
                dbContext.SaveChanges();
            }

            return app;
        }

    }

}
