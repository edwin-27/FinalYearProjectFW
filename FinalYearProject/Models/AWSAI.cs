using Amazon.Util.Internal.PlatformServices;
using FinalYearProject.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using OpenSearch.Client;
using OpenSearch.Net;
using System.Data;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FinalYearProject.Models
{
    public class AWSAI
    {

    }

    public class AWSProduct 
    {
        //public AWSProduct() {
        //    Variants = new List<IVariant>();
        //}

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryCode { get; set; }
        public string Code { get; set; }
        public string CategoryName { get; set; }
        public string Search_Field { get; set; }
        public string Image { get; set; }

        public AWSVariant MainVariant { get; set; }

        public List<AWSVariant> Variants { get; set; }

    }

    public class AWSVariant //: IVariant
    {
        public int Id { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        public double Price { get; set; }
        
        public string ColourCode { get; set; }

    }

    public class ProductSearchService
    {
        private readonly IOpenSearchClient _client;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        public ProductSearchService(IOpenSearchClient client, ApplicationDbContext db, IConfiguration configuration)
        {
            _client = client;
            _db = db;
            _configuration = configuration;

        }

        public async Task IndexAllProductAsync()
        {
            AWSProduct product = new AWSProduct { Id = 1, Name = "test1", Description = "desc 1", CategoryCode = "cat1", Code = "prod1", CategoryName = "Clothing", Variants = new List<AWSVariant> { new AWSVariant { Id = 1, Colour = "red", Size = "10", Price = 9.99 } } };

            string query = "select p.id, p.Name, p.Description, p.code, p.CategoryCode, v.id as sku, v.Colour, v.Size, v.Price, 'cat_name'=c.Name, p.Image, v.ColourCode from product p join ProductVariant v on v.ProductId = p.id join Category c on c.Code = p.CategoryCode order by p.id, v.ProductId";

            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            connection.Open();

            SqlCommand command = new SqlCommand(query, connection);

            SqlDataReader reader = command.ExecuteReader();

            List<AWSProduct> oProds = new List<AWSProduct>();
            while (reader.Read())
            {

                AWSProduct oProd;// = new AWSProduct();
                AWSVariant oVar ;

                int id = Convert.ToInt32(reader["id"].ToString());
                string ProdName = reader["name"].ToString();
                string ProdDesc = reader["Description"].ToString();
                string ProdCode = reader["code"].ToString();
                string CategoryCode = reader["CategoryCode"].ToString();
                
                int sku = Convert.ToInt32(reader["sku"].ToString());
                string Colour = reader["Colour"].ToString();
                string Size = reader["Size"].ToString();
                double Price = Convert.ToDouble(reader["Price"].ToString());
                string cat_name = reader["cat_name"].ToString();
                string Search_Field = string.Format("{0} {1} Catetory: {5} Colour: #COLOUR# Size:#SIZE# Price:{4}", ProdName, ProdDesc, Colour, Size, Price, cat_name);
                string Image = reader["Image"].ToString();
                string ColourCode = reader["ColourCode"].ToString();



                oProd = oProds.Where(a => a.Id == id).FirstOrDefault();
                if (oProd==null)
                {
                    oProd = new AWSProduct { Id = id, Name = ProdName, Description = ProdDesc, CategoryCode = CategoryCode, Code = ProdCode, CategoryName = cat_name, Search_Field = Search_Field,  Variants = new List<AWSVariant>(), Image = Image };
                    oProds.Add(oProd);
                }
                oVar = new AWSVariant { Id = sku, Colour = Colour, Size = Size, Price  = Price, ColourCode = ColourCode };

                if (oProd.MainVariant == null)
                {
                    oProd.MainVariant = oVar;
                }

                oProd.Variants.Add(oVar);

            }

            
            reader.Close();


            foreach(var oProd in  oProds)
            {
                oProd.Search_Field = oProd.Search_Field.Replace("#COLOUR#", string.Join(", ", oProd.Variants.Select(a=>a.Colour).Distinct().ToList()));
                await IndexProductAsync(oProd);
            }

            
        
        }



        public async Task IndexProductAsync(AWSProduct product)
        {
            var response = await _client.IndexDocumentAsync(product);
            if (!response.IsValid)
            {
                throw new Exception($"Failed to index product: {response.OriginalException.Message}");
            }
        }


        public async Task<List<AWSProduct>> SearchProductsAsync(string query)
        {
            /*
            var response2 = await _client.SearchAsync<AWSProduct>(s => s
                .Query(q => q.QueryString(m => m.DefaultField(f=>f.Search_Field).Query(query)
                ))
            );

            var response22 = await _client.SearchAsync<AWSProduct>(s => s
                .Query(q => q.Wildcard(w => w.Field(f => f.Search_Field).Value($"*{query.ToLower()}*")
                ))
            );
            */

            var response = await _client.SearchAsync<AWSProduct>(s => s
                .Size(20)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Search_Field) // must be a "text" field
                        .Query(query)
                        .Fuzziness(Fuzziness.Auto) // optional: allows minor typos like "t-shrt"
                    )
                )
            );

            var products = new List<AWSProduct>();
            products = response.Documents.ToList<AWSProduct>();

            return products;

        }

    public async Task<List<AWSCustomer>> SearchProducts2Async(string query)
        {
            var response = await _client.SearchAsync<AWSCustomer>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.customer_first_name).Field(f1 => f1.customer_gender)
                        .Query(query)
                    )
                )
            );

            //var response = await _client.SearchAsync<AWSCustomer>(s => s
            //    .Query(q => q.QueryString(m => m.Fields(["customer_first_name","customer_gender"]).Query(query)
            //    ))
            //);

            //var response = await _client.SearchAsync<AWSCustomer>(s => s
            //    .Query(q => q.QueryString(m=>m.DefaultField(f=>f.customer_first_name).Query(query)
            //    ))
            //);

            return response.Documents.ToList();
        }
    }

    //public class AWSProduct
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public double Price { get; set; }
    //}

    public class AWSCustomer
    {
        public List<string> category { get; set; }
        public string currency { get; set; }
        public string customer_first_name { get; set; }
        public string customer_full_name { get; set; }
        public string customer_gender { get; set; }
        public int customer_id { get; set; }
        public string customer_last_name { get; set; }
        public string customer_phone { get; set; }
        public string day_of_week { get; set; }
        public int day_of_week_i { get; set; }
        public string email { get; set; }
        public List<string> manufacturer { get; set; }
        public DateTime order_date { get; set; }
        public int order_id { get; set; }
        public List<string> sku { get; set; }
        public double taxful_total_price { get; set; }
        public double taxless_total_price { get; set; }
        public int total_quantity { get; set; }
        public int total_unique_products { get; set; }
        public string type { get; set; }
        public string user { get; set; }
    }

}
