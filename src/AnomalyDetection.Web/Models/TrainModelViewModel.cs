using System.ComponentModel.DataAnnotations;

namespace AnomalyDetection.Web.Models;

public class TrainModelViewModel
{
    [Required(ErrorMessage = "Model name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Please select a model type")]
    [Display(Name = "Model Type")]
    public string SelectedModelType { get; set; }

    public List<string> ModelTypes { get; set; }

    [Display(Name = "Training Data Size")]
    [Range(100, 1000000, ErrorMessage = "Training data size must be between 100 and 1,000,000 records")]
    public int? TrainingDataSize { get; set; }

    [Display(Name = "Contamination Ratio (for Isolation Forest)")]
    [Range(0.01, 0.5, ErrorMessage = "Contamination must be between 1% and 50%")]
    public double? Contamination { get; set; } = 0.1;

    [Display(Name = "Number of Components (for PCA)")]
    [Range(1, 10, ErrorMessage = "Number of components must be between 1 and 10")]
    public int? NumberOfComponents { get; set; } = 3;
}
