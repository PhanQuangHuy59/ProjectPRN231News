﻿using Microsoft.AspNetCore.Mvc;

namespace WebNewsClients.Controllers
{
    public class ArticleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ArticleOfCategory(Guid categoryArticleId)
        {
            return View();
        }
    }
}
