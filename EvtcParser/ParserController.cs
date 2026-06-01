namespace GW2EIEvtcParser;


public abstract class ParserController
{

    protected readonly List<string> StatusList;
    /// <summary>
    /// Uncompressed file size
    /// </summary>
    public long FileSize { get; private set; }

    protected ParserController()
    {
        StatusList = [];
    }

    protected virtual void ThrowIfCanceled()
    {

    }

    public void WriteLogMessages(StreamWriter sw)
    {
        foreach (string str in StatusList)
        {
            sw.WriteLine(str);
        }
    }

    public virtual void Reset()
    {
        StatusList.Clear();
    }

    public virtual void UpdateProgressWithCancellationCheck(string status)
    {
        UpdateProgress(status);
        ThrowIfCanceled();
    }
    public virtual void UpdateProgress(string status)
    {
        StatusList.Add(status);
    }

    internal void SetFileSize(long fileSize)
    {
        FileSize = fileSize;
    }
}
