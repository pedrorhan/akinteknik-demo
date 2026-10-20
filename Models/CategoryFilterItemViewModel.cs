using System.Collections.Generic;

namespace AkinTeknik.Web.Models;

public class CategoryFilterItemViewModel
{
    public Category Category { get; set; } = null!;
    public int? SelectedCategoryId { get; set; }
    public HashSet<int> ActivePathIds { get; set; } = new();
    public int Level { get; set; } = 0;
    public bool UseSlug { get; set; } = false;
}
