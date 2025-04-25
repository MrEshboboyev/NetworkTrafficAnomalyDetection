using System.ComponentModel.DataAnnotations;

namespace AnomalyDetection.Web.Models;

public class EvaluateModelViewModel
{
    [Required]
    public int ModelId { get; set; }

    [Display(Name = "Model Name")]
    public string ModelName { get; set; }

    [Display(Name = "Test Data Size")]
    [Range(10, 100000, ErrorMessage = "Test data size must be between 10 and 100,000 records")]
    public int? TestDataSize { get; set; }

    [Display(Name = "Use Separate Test Set")]
    public bool UseSeparateTestSet { get; set; } = true;

    [Display(Name = "Test Data Percentage")]
    [Range(10, 90, ErrorMessage = "Test percentage must be between 10% and 90%")]
    public int? TestPercentage { get; set; } = 20;
}
