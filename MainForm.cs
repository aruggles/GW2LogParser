using System.ComponentModel;
using GW2EIEvtcParser;
using GW2EIParserCommons.Exceptions;
using Gw2LogParser.EvtcParserExtensions;

namespace Gw2LogParser;

public partial class MainForm : Form
{
    private readonly List<string> _logFiles;
    private int _runningCount = 0;
    private bool _anyRunning => _runningCount > 0;
    private readonly ProgramHelper _programHelper;
    private readonly Queue<FormOperationController> _logQueue = new();
    private readonly string _traceFileName;
    private int _fileNameSorting = 1;

    public MainForm(ProgramHelper programHelper)
    {
        _programHelper = programHelper;
        _logFiles = [];
        DateTime now = DateTime.Now;
        _traceFileName = ProgramHelper.EILogPath + "EILogs-" + now.Year + "-" + now.Month + "-" + now.Day + "-" + now.Hour + "-" + now.Minute + "-" + now.Second + ".txt";
        InitializeComponent();
        
        btnCancel.Enabled = false;
        btnParse.Enabled = false;
        btnClear.Enabled = true;
         //finalWorker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
    }

    private void DataGridView_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effect = DragDropEffects.Move;
        } else
        {
            e.Effect = DragDropEffects.None;
        }
    }

    private void DataGridView_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data == null) { 
            return;
        }
        string[] filesArray = (string[])e.Data.GetData(DataFormats.FileDrop, false);
        AddLogFiles(filesArray);
    }

    private void AddTraceMessage(string message)
    {
        if (dataGridView.InvokeRequired)
        {
            dataGridView.Invoke(new Action(() => _AddTraceMessage(message)));
        }
        else
        {
            _AddTraceMessage(message);
        }
    }

    /// <summary>
    /// Adds log files to the bound data source for display in the interface
    /// </summary>
    /// <param name="filesArray"></param>
    private void AddLogFiles(IEnumerable<string> filesArray)
    {
        bool any = false;
        foreach (string file in filesArray)
        {
            if (_logFiles.Contains(file))
            {
                //Don't add doubles
                continue;
            }
            any = true;
            _logFiles.Add(file);
            AddTraceMessage("UI: Added " + file);

            var operation = new FormOperationController(file, "Ready to parse", dataGridView, gridBindingSource, _logFiles.Count);

            if (Properties.Settings.Default.AutoParse)
            {
                QueueOrRunOperation(operation);
            }
        }
        if (_fileNameSorting != 0)
        {
            SortDgvFiles();
        }

        btnParse.Enabled = !_anyRunning && any;
        btnCancel.Enabled = false;
    }

    private void SortDgvFiles()
    {
        if (_fileNameSorting == 0)
        {
            _fileNameSorting = 1;
        }
        var auxList = new List<FormOperationController>();
        foreach (FormOperationController val in gridBindingSource)
        {
            auxList.Add(val);
        }
        auxList.Sort((form1, form2) =>
        {
            string right = new FileInfo(form2.InputFile).Name;
            string left = new FileInfo(form1.InputFile).Name;
            return _fileNameSorting * string.Compare(left, right, StringComparison.Ordinal);
        });
        gridBindingSource.Clear();
        var i = 0;
        foreach (FormOperationController val in auxList)
        {
            val.Index = ++i;
            gridBindingSource.Add(val);
        }
    }

    private void _RunOperation(FormOperationController operation)
    {
        _programHelper.ExecuteMemoryCheckTask();
        _runningCount++;
        // _settingsForm.ConditionalSettingDisable(_anyRunning);
        operation.ToQueuedState();
        AddTraceMessage("Operation: Queued " + operation.InputFile);
        var cancelTokenSource = new CancellationTokenSource();// Prepare task
        Task task = Task.Run(() =>
        {
            operation.ToRunState();
            AddTraceMessage("Operation: Parsing " + operation.InputFile);
            _programHelper.DoWork(operation);
        }, cancelTokenSource.Token).ContinueWith(t =>
        {
            GC.Collect();
            cancelTokenSource.Dispose();
            _runningCount--;
            AddTraceMessage("Operation: Parsed " + operation.InputFile);
            // Exception management
            if (t.IsFaulted)
            {
                if (t.Exception != null)
                {
                    if (t.Exception.InnerExceptions.Count > 1)
                    {
                        operation.UpdateProgress("Program: something terrible has happened");
                    }
                    else
                    {
                        Exception ex = t.Exception.InnerExceptions[0];
                        if (ex is not ProgramException)
                        {
                            operation.UpdateProgress("Program: something terrible has happened");
                        }
                        if (ex.InnerException is OperationCanceledException)
                        {
                            operation.UpdateProgress("Program: operation Aborted");
                        }
                        else if (ex.InnerException != null)
                        {
                            var finalException = ParserHelper.GetFinalException(ex);
                            operation.UpdateProgress("Program: " + finalException.Source);
                            operation.UpdateProgress("Program: " + finalException.StackTrace);
                            operation.UpdateProgress("Program: " + finalException.TargetSite);
                            operation.UpdateProgress("Program: " + finalException.Message);
                        }
                    }
                }
                else
                {
                    operation.UpdateProgress("Program: something terrible has happened");
                }
            }
            if (operation.State == OperationState.ClearOnCancel)
            {
                gridBindingSource.Remove(operation);
            }
            else
            {
                if (t.IsFaulted)
                {
                    operation.ToUnCompleteState();
                }
                else if (t.IsCanceled)
                {
                    operation.UpdateProgress("Program: operation Aborted");
                    operation.ToUnCompleteState();
                }
                else if (t.IsCompleted)
                {
                    operation.ToCompleteState();
                }
                else
                {
                    operation.UpdateProgress("Program: something terrible has happened");
                    operation.ToUnCompleteState();
                }
            }
            _programHelper.GenerateTraceFile(operation);
            if (operation.State != OperationState.Complete)
            {
                operation.Reset();
            }
            _RunNextOperation();
        }, TaskScheduler.FromCurrentSynchronizationContext());
        operation.SetContext(cancelTokenSource, task);
    }

    /// <summary>
    /// Runs the next operation, if one is available
    /// </summary>
    private void _RunNextOperation()
    {
        if (_logQueue.Count > 0 && (_programHelper.ParseMultipleLogs() || !_anyRunning))
        {
            _RunOperation(_logQueue.Dequeue());
        }
        else
        {
            if (!_anyRunning)
            {
                ProgramHelper.GenerateSummary(_programHelper.ParserVersion);
                btnParse.Enabled = true;
                btnClear.Enabled = true;
                btnCancel.Enabled = false;
                // _settingsForm.ConditionalSettingDisable(_anyRunning);
            }
        }
    }

    /// <summary>
    /// Queues an operation. If the 'MultipleLogs' setting is true, operations are run asynchronously
    /// </summary>
    /// <param name="operation"></param>
    private void QueueOrRunOperation(FormOperationController operation)
    {
        btnClear.Enabled = false;
        btnParse.Enabled = false;
        btnCancel.Enabled = true;
        if (_programHelper.ParseMultipleLogs() && _runningCount < _programHelper.GetMaxParallelRunning())
        {
            _RunOperation(operation);
        }
        else
        {
            if (_anyRunning)
            {
                _logQueue.Enqueue(operation);
                operation.ToPendingState();
            }
            else
            {
                _RunOperation(operation);
            }
        }

    }

    private void BtnParse_Click(object sender, EventArgs e)
    {
        AddTraceMessage("UI: Parse all files");
        //Clear queue before parsing all
        _logQueue.Clear();
        ProgramHelper.CompletedLogs.Clear();
        if (_logFiles.Count > 0)
        {
            btnParse.Enabled = false;
            btnCancel.Enabled = true;
            ProgramHelper.timestamp = DateTime.Now.ToFileTime();

            foreach (FormOperationController operation in gridBindingSource)
            {
                if (!operation.IsBusy())
                {
                    QueueOrRunOperation(operation);
                }
            }
        }
    }

    private void BtnCancel_Click(object sender, EventArgs e)
    {
        AddTraceMessage("UI: Cancelling all pending and ongoing parsing operations");
        //Clear queue so queued workers don't get started by any cancellations
        var operations = new HashSet<FormOperationController>(_logQueue);
        _logQueue.Clear();
        ProgramHelper.CompletedLogs.Clear();

        //Cancel all workers
        foreach (FormOperationController operation in gridBindingSource)
        {
            if (operation.IsBusy())
            {
                operation.ToCancelState();
            }
            else if (operations.Contains(operation))
            {
                operation.ToReadyState();
            }
        }

        btnClear.Enabled = true;
        btnParse.Enabled = true;
        btnCancel.Enabled = false;
    }

    private void BtnClear_Click(object sender, EventArgs e)
    {
        AddTraceMessage("UI: Clearing all logs");
        btnCancel.Enabled = false;
        btnParse.Enabled = false;

        //Clear the queue so that cancelled workers don't invoke queued workers
        _logQueue.Clear();
        _logFiles.Clear();

        for (int i = gridBindingSource.Count - 1; i >= 0; i--)
        {
            var operation = gridBindingSource[i] as FormOperationController;
            if (operation.IsBusy())
            {
                operation.ToCancelAndClearState();
            }
            else
            {
                gridBindingSource.RemoveAt(i);
            }
        }
    }

    private void BtnRefreshAPI_Click(object sender, EventArgs e)
    {
        btnRefreshAPI.Enabled = false;
        if (!Directory.Exists(ProgramHelper.CacheLocation))
        {
            Directory.CreateDirectory(ProgramHelper.CacheLocation);
        }
        ProgramHelper.apiController.WriteAPISkillsToFile(ProgramHelper.SkillAPICacheLocation);
        ProgramHelper.apiController.WriteAPISpecsToFile(ProgramHelper.SpecAPICacheLocation);
        ProgramHelper.apiController.WriteAPITraitsToFile(ProgramHelper.TraitAPICacheLocation);
        btnRefreshAPI.Enabled = true;
        MessageBox.Show("API cache has been refreshed");
    }

    private void _AddTraceMessage(string message)
    {
        if (!Properties.Settings.Default.ApplicationTraces)
        {
            return;
        }
        if (!Directory.Exists(ProgramHelper.EILogPath))
        {
            Directory.CreateDirectory(ProgramHelper.EILogPath);
        }
        if (!File.Exists(_traceFileName))
        {
            using (StreamWriter sw = File.CreateText(_traceFileName))
            {
                sw.WriteLine(message);
            }
        }
        else
        {
            using (StreamWriter sw = File.AppendText(_traceFileName))
            {
                sw.WriteLine(message);
            }
        }
    }
}
