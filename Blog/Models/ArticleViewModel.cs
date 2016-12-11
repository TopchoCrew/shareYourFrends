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
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string AuthorId { get; set; }
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public string Tags { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}