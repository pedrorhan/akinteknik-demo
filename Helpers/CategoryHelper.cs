using System.Collections.Generic;
using System.Linq;
using AkinTeknik.Web.Models;

namespace AkinTeknik.Web.Helpers;

public static class CategoryHelper
{
    /// <summary>
    /// Veritabanından gelen tüm kategorileri in-memory hiyerarşik ağaç haline getirir.
    /// Hem ParentCategory hem de SubCategories tüm derinliklerde (Level 1, 2, 3...) birbirine bağlanır.
    /// </summary>
    public static List<Category> BuildCategoryHierarchy(List<Category> allCategories)
    {
        if (allCategories == null || !allCategories.Any())
            return new List<Category>();

        var lookup = allCategories.ToDictionary(c => c.Id);

        foreach (var cat in allCategories)
        {
            cat.SubCategories = new List<Category>();
        }

        foreach (var cat in allCategories)
        {
            if (cat.ParentCategoryId.HasValue && lookup.TryGetValue(cat.ParentCategoryId.Value, out var parent))
            {
                cat.ParentCategory = parent;
                parent.SubCategories.Add(cat);
            }
        }

        return allCategories
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToList();
    }

    /// <summary>
    /// Bir kategorinin ve onun altındaki tüm alt/torun kategorilerin ID'lerini döner.
    /// </summary>
    public static HashSet<int> GetAllDescendantIds(int categoryId, IEnumerable<Category> allCategories)
    {
        var result = new HashSet<int> { categoryId };
        var allList = allCategories.ToList();
        var queue = new Queue<int>();
        queue.Enqueue(categoryId);

        while (queue.Count > 0)
        {
            var curId = queue.Dequeue();
            var childIds = allList.Where(c => c.ParentCategoryId == curId).Select(c => c.Id);
            foreach (var childId in childIds)
            {
                if (result.Add(childId))
                {
                    queue.Enqueue(childId);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Verilen kategori ID'sinin kendisini ve tüm ata (üst) kategorilerinin ID'lerini döner.
    /// Bu sayede sidebar accordionlarında tüm üst menüler otomatik açık (expanded) kalır.
    /// </summary>
    public static HashSet<int> GetActivePathIds(int categoryId, IDictionary<int, Category> categoryMap)
    {
        var activeIds = new HashSet<int>();
        int? currentId = categoryId;

        while (currentId.HasValue && categoryMap.TryGetValue(currentId.Value, out var cat))
        {
            activeIds.Add(cat.Id);
            currentId = cat.ParentCategoryId;
        }

        return activeIds;
    }

    /// <summary>
    /// Verilen kategori için kökten hedefe doğru tam breadcrumb listesi döner.
    /// Örn: [ELMAS PASTALAR, NACTA ELMAS PASTALAR, NACTA - 40 GRAM]
    /// </summary>
    public static List<Category> GetBreadcrumbTrail(int categoryId, IDictionary<int, Category> categoryMap)
    {
        var trail = new List<Category>();
        int? currentId = categoryId;

        while (currentId.HasValue && categoryMap.TryGetValue(currentId.Value, out var cat))
        {
            trail.Insert(0, cat);
            currentId = cat.ParentCategoryId;
        }

        return trail;
    }

    /// <summary>
    /// Kategori hiyerarşik tam adını döner: "ELMAS PASTALAR > NACTA ELMAS PASTALAR > NACTA - 40 GRAM"
    /// </summary>
    public static string GetFullHierarchyName(int categoryId, IDictionary<int, Category> categoryMap)
    {
        var trail = GetBreadcrumbTrail(categoryId, categoryMap);
        return string.Join(" > ", trail.Select(c => c.Name));
    }
}
