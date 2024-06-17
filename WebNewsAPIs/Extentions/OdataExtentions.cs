using BusinessObjects.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.UriParser;
using System.Globalization;
using System.Security.Permissions;

namespace WebNewsAPIs.Extentions
{
    public static class OdataExtentions
    {
        public static void ConfigOdata(this IServiceCollection services)
        {
            services.AddControllers()
                    .AddOData(builderOdata => builderOdata.Select().Filter().Count().OrderBy().Expand().SetMaxTop(100)
                    .AddRouteComponents("odata", getEdmModel()));
        }
        public static IEdmModel getEdmModel()
        {
            ODataConventionModelBuilder conventionModelBuilder = new ODataConventionModelBuilder();
            conventionModelBuilder.EntitySet<ArticlePermission>("ArticlePermissions").EntityType
                .HasKey(c => c.ApermissionId);
                
            conventionModelBuilder.EntitySet<Article>("Articles");
            conventionModelBuilder.EntitySet<CategoriesArticle>("CategoriesArticles")
                .EntityType.HasKey(c => new {c.CategoryId});
            conventionModelBuilder.EntitySet<Comment>("Comments");
            conventionModelBuilder.EntitySet<DropEmotion>("DropEmotions");
            conventionModelBuilder.EntitySet<Emotion>("Emotions");
            conventionModelBuilder.EntitySet<Follow>("Follows");
            conventionModelBuilder.EntitySet<Permission>("Permissions");
            conventionModelBuilder.EntitySet<ProcessStatus>("ProcessStatuss")
                .EntityType.HasKey(c => c.ProcessId);
            conventionModelBuilder.EntitySet<Role>("Roles");
            conventionModelBuilder.EntitySet<SaveArticle>("SaveArticles")
                .EntityType.HasKey(c => new {c.SaveId});
            conventionModelBuilder.EntitySet<User>("Users");
            conventionModelBuilder.EntitySet<View>("Views");


            return conventionModelBuilder.GetEdmModel();
        }
    }
	public class MyResolver : UnqualifiedODataUriResolver
	{
		private StringAsEnumResolver enumResolver = new StringAsEnumResolver();

		public MyResolver()
		{
			EnableCaseInsensitive = true;
		}

		public override void PromoteBinaryOperandTypes(BinaryOperatorKind binaryOperatorKind, ref SingleValueNode leftNode, ref SingleValueNode rightNode, out IEdmTypeReference typeReference)
		{
			typeReference = null;

			if (leftNode.TypeReference.IsEnum() && rightNode.TypeReference.IsInt32() && rightNode is ConstantNode)
			{
				string text = (((ConstantNode)rightNode).Value).ToString();
				ODataEnumValue val;
				IEdmTypeReference typeRef = leftNode.TypeReference;

				if (TryParseEnum(typeRef.Definition as IEdmEnumType, text, out val))
				{
					rightNode = new ConstantNode(val, text, typeRef);
					return;
				}
			}
			else if (rightNode.TypeReference.IsEnum() && leftNode.TypeReference.IsInt32() && leftNode is ConstantNode)
			{
				string text = ((ConstantNode)leftNode).Value.ToString();
				ODataEnumValue val;
				IEdmTypeReference typeRef = rightNode.TypeReference;
				if (TryParseEnum(typeRef.Definition as IEdmEnumType, text, out val))
				{
					leftNode = new ConstantNode(val, text, typeRef);
					return;
				}
			}

			enumResolver.PromoteBinaryOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, out typeReference);
		}

		private static bool TryParseEnum(IEdmEnumType enumType, string value, out ODataEnumValue enumValue)
		{
			long parseResult;
			bool num = enumType.TryParseEnum(value, ignoreCase: true, out parseResult);
			enumValue = null;
			if (num)
			{
				enumValue = new ODataEnumValue(parseResult.ToString(CultureInfo.InvariantCulture), enumType.FullTypeName());
			}
			return num;
		}
	}
}
