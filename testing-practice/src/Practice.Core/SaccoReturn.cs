namespace Practice.Core;

public enum ReturnStatus
{
    Draft,
    Submitted,
    Approved
}

public class SaccoReturn
{
    public SaccoReturn(string saccoName)
    {
        if (string.IsNullOrWhiteSpace(saccoName))
        {
            throw new ArgumentException("SACCO name is required.", nameof(saccoName));
        }

        SaccoName = saccoName;
    }

    public string SaccoName { get; }
    public ReturnStatus Status { get; private set; } = ReturnStatus.Draft;
    public DateTimeOffset? SubmittedOn { get; private set; }
    public DateTimeOffset? ApprovedOn { get; private set; }

    public void Submit(DateTimeOffset now)
    {
        if (Status != ReturnStatus.Draft)
        {
            throw new InvalidOperationException("Only draft returns can be submitted.");
        }

        Status = ReturnStatus.Submitted;
        SubmittedOn = now;
    }

    public void Approve(DateTimeOffset now)
    {
        if (Status == ReturnStatus.Approved)
        {
            throw new InvalidOperationException("Return is already approved.");
        }

        Status = ReturnStatus.Approved;
        ApprovedOn = now;
    }
}
