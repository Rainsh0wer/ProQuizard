using System;
using System.Collections.Generic;

namespace ProjectQuizard.Models;

public partial class QuestionOption
{
    public int OptionId { get; set; }

    public int QuestionId { get; set; }

    public string OptionLabel { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string OptionText
    {
        get => Content;
        set => Content = value;
    }
    public bool IsCorrect { get; set; }

    public virtual Question Question { get; set; } = null!;
}
