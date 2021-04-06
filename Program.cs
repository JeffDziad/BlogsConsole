using System;
using NLog.Web;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BlogsConsole
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        private static Boolean endProgram = false;
        static void Main(string[] args)
        {
            int userInput;
            while(!endProgram)
            {
                userInput = Menu();
                switch(userInput)
                {
                    case 0:
                        endProgram = true;
                        break;
                    case 1:
                        DisplayBlogs();
                        break;
                    case 2:
                        AddBlog();
                        break;
                    case 3: 
                        DisplayPosts(getBlog());
                        break;
                    case 4: 
                        AddPost(getBlog());
                        break;
                    default:
                        logger.Info("Please enter a valid Menu Option!");
                        break;
                }
            }
        }

        static int Menu()
        {
            string userInputStr;
            int userInputInt;
            Console.WriteLine("Choose a Menu Option: ");
            Console.WriteLine("0. Exit Program");
            Console.WriteLine("1. Display Blogs");
            Console.WriteLine("2. Add Blog");
            Console.WriteLine("3. Display Posts");
            Console.WriteLine("4. Add Post");
            Console.Write("> ");
            userInputStr = Console.ReadLine();
            try
            {
                userInputInt = Convert.ToInt32(userInputStr);
                return userInputInt;
            }catch(Exception ex)
            {
                logger.Error(ex.Message);
                return -1;
            }

        }

        static void DisplayBlogs()
        {
            try
            {
                using(var db = new BloggingContext())
                {
                    int count = 0;
                    var allBlogs = db.Blogs.OrderBy(b => b.Name);
                    Console.WriteLine("Blogs: ");
                    foreach(var blog in allBlogs)
                    {
                        Console.WriteLine($" - {blog.Name}");
                        count++;
                    }
                    Console.WriteLine($"{count} Blogs.");
                }
            }catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
        }


        static void AddBlog()
        {
            try
            {
                Console.Write("Enter Blog Name: ");
                string name = Console.ReadLine();
                Blog blog = new Blog {Name = name};
                using(var db = new BloggingContext())
                {
                    db.AddBlog(blog);
                    db.SaveChanges();
                }
                Console.WriteLine($"Added Blog - {name}");
            }catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
            
        }

        static int getBlog()
        {
            int userInputInt = 0;
            using(var db = new BloggingContext())
            {
                try
                {
                    var allBlogs = db.Blogs;
                    Console.WriteLine("Blogs: ");
                    foreach(var blog in allBlogs)
                    {
                        Console.WriteLine($"{blog.BlogId} - {blog.Name}");
                    }
                    Console.Write($"Select Blog by ID#: ");
                    string userInputStr = Console.ReadLine();
                    try
                    {
                        userInputInt = Convert.ToInt32(userInputStr);
                    }catch(Exception ex)
                    {
                        logger.Error(ex.Message);
                        getBlog();
                    }
                }catch(Exception ex)
                {
                    logger.Error(ex.Message);
                }
                return userInputInt;
            }
        }

        static void DisplayPosts(int blogId)
        {
            using(var db = new BloggingContext())
            {
                try
                {
                    var blogs = db.Blogs.Include(b => b.Posts).ToList();
                    foreach(var blog in blogs)
                    {
                        if(blog.BlogId == blogId)
                        {
                            int count = 0;
                            Console.WriteLine("Blog Name | Post Title | Post Content ");
                            var posts = blog.Posts.ToArray();
                            foreach(var post in posts)
                            {
                                count++;
                                Console.WriteLine($"{count} - {blog.Name} | {post.Title} | {post.Content}");
                            }
                            Console.WriteLine($"{count} Posts.");
                        }
                    }
                }catch(Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }
        }

        static void AddPost(int blogId)
        {
            using(var db = new BloggingContext())
            {
                try
                {
                    Console.Write("Enter Post Title: ");
                    string postTitle = Console.ReadLine();
                    Console.WriteLine("-Enter Post Content-");
                    string postContent = Console.ReadLine();
                    Post post = new Post {Title = postTitle, Content = postContent, BlogId = blogId};
                    db.Posts.Add(post);
                    db.SaveChanges();
                }catch(Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }
        }
    }
}
