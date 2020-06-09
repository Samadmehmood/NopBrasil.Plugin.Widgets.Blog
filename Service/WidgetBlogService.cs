using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Services.Blogs;
using Nop.Services.Seo;
using NopBrasil.Plugin.Widgets.Blog.Models;

namespace NopBrasil.Plugin.Widgets.Blog.Service
{
    public class WidgetBlogService : IWidgetBlogService
    {
        private readonly IBlogService _blogService;
        private readonly BlogSettings _blogSettings;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IUrlRecordService _urlRecordService;

        public WidgetBlogService(IBlogService blogService, BlogSettings blogSettings, IStaticCacheManager cacheManager, IUrlRecordService urlRecordService)
        {
            this._blogService = blogService;
            this._blogSettings = blogSettings;
            this._cacheManager = cacheManager;
            this._urlRecordService = urlRecordService;
        }

        private IPagedList<BlogPost> GetAllBlogPosts()
        {
            CacheKey cacheKey = new CacheKey($"nopBrasil:blogPosts:qtd:{_blogSettings.QtdBlogPosts}");
            return _cacheManager.Get<IPagedList<BlogPost>>(cacheKey, () => _blogService.GetAllBlogPosts(pageIndex: 0, pageSize: _blogSettings.QtdBlogPosts));
        }

        public PublicInfoModel GetModel()
        {
            PublicInfoModel model = new PublicInfoModel();
            foreach (var post in GetAllBlogPosts())
            {
                model.BlogItems.Add(new BlogItemModel()
                {
                    CreatedOn = post.CreatedOnUtc,
                    Title = post.Title,
                    Short = post.BodyOverview,
                    Full = post.Body,
                    SeName = _urlRecordService.GetSeName(post, post.LanguageId, ensureTwoPublishedLanguages: false),
                    Id = post.Id
                });
            }
            return model;
        }
    }
}
