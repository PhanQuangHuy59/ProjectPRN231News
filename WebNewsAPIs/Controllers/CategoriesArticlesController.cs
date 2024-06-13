using AutoMapper;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using WebNewsAPIs.Dtos;

namespace WebNewsAPIs.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoriesArticlesController : ControllerBase
	{
		private ICategoriesArticleRepository _categoryRepository;
		private IMapper _mapper;
		public CategoriesArticlesController(ICategoriesArticleRepository categoriesArticleRepository
			, IMapper mapper)
		{
			this._categoryRepository = categoriesArticleRepository;
			this._mapper = mapper;
		}
		[HttpGet("getAllCategory")]
		public async Task<ActionResult<IEnumerable<ViewCategoriesArticleDto>>> getAllCategory()
		{
			string[] includes = new string[]
			{
				nameof(CategoriesArticle.ParentCategory),
				nameof(CategoriesArticle.InverseParentCategory)
			};
			if (_categoryRepository.Equals(null))
			{
				return BadRequest();
			}
			var temp = _categoryRepository.GetMulti(c => c.ParentCategory == null, includes);
			var resultList = _mapper.Map<IEnumerable<ViewCategoriesArticleDto>>(temp);

			return Ok(resultList);
		}
	}
}
