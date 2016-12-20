using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class ArticleViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Friends_Name { get; set; }//edit by nasko

        //[Required]
        //[StringLength(50)]
        //public string GitHubLink { get; set; }//add by nasko


        [Required]
        public string Content { get; set; }

        public string AuthorId { get; set; }

        public int City { get; set; }

        public string ProgrammingLanguage { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}