using FinalYearProject.Models;

namespace FinalYearProject.ViewModels
{
    public class SearchViewModel
    {

        public SearchViewModel() { }

        public string SearchTerm {  get; set; }
        public List<AWSProduct> Products { get; set; }

    }
}
