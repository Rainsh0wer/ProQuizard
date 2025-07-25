using System;
using System.Collections.Generic;

namespace ProjectQuizard.Models;

public partial class Enrollment
{
    public int EnrollmentId { get; set; }

    public int ClassId { get; set; }

    public int StudentId { get; set; }

    public DateTime? EnrolledAt { get; set; }

    public int ClassroomId
    {
        get => ClassId;
        set => ClassId = value;
    }
    public Classroom Classroom
    {
        get => Class;
        set => Class = value;
    }

    public virtual Classroom Class { get; set; } = null!;

    public virtual User Student { get; set; } = null!;
}
