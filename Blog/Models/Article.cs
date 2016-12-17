using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models
{
    public class Article
    {
        public Article()
        {
            this.ProgrammingLanguage = new HashSet<Tag>();
            this.Comments = new HashSet<Comment>();
        }

        public Article(string authorId, string Friends_Name, string content, int categoryId)
            : this()
        {
            this.AuthorId = authorId;
            this.Friends_Name = Friends_Name;
            this.Content = content;
            this.City = categoryId;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Friends_Name { get; set; }//edit by nasko

        [Required]
        public string Content { get; set; }

        [ForeignKey("Author")]
        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public bool IsAuthor(string name)
        {
            return this.Author.UserName.Equals(name);
        }

        [ForeignKey("Category")]
        [Display(Name = "Category")]
        public int City { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Tag> ProgrammingLanguage { get; set; }
    }
}