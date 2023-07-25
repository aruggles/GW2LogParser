using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Gw2LogParser.Exceptions;
using Gw2LogParser.EvtcParserExtensions;

namespace Gw2LogParser
{
    public partial class MainForm : Form
    {
        private readonly List<string> logFiles;
        private int runningCount;
        private bool isRunning;
        private BackgroundWorker finalWorker;
        private readonly Queue<GridRow> queue = new Queue<GridRow>();

        public MainForm()
        {
            InitializeComponent();
            logFiles = new List<string>();
            btnCancel.Enabled = false;
            btnParse.Enabled = false;
            btnClear.Enabled = true;
        }

        private void dataGridView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            } else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void dataGridView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileList = (string[]) e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in fileList)
                {
                    if (logFiles.Contains(file)) { continue; }
                    logFiles.Add(file);

                    var gridRow = new GridRow(file, "Ready to Parse")
                    {
                        BgWorker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true }
                    };
                    gridRow.BgWorker.DoWork += BgWorkerDoWork;
                    gridRow.BgWorker.ProgressChanged += BgWorkerProgressChanged;
                    gridRow.BgWorker.RunWorkerCompleted += BgWorkerCompleted;

                    gridBindingSource.Add(gridRow);
                }
                if (logFiles.Count > 0)
                {
                    btnParse.Enabled = true;
                    btnCancel.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Runs the next background worker, if one is available
        /// </summary>
        private void RunNextWorker()
        {
            if (queue.Count > 0)
            {
                GridRow row = queue.Dequeue();
                isRunning = true;
                row.Run();
            }
            else
            {
                if (runningCount == 0)
                {
                    finalWorker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
                    finalWorker.DoWork += FinalWorkerDoWork;
                    finalWorker.ProgressChanged += FinalWorkerProgressChanged;
                    finalWorker.RunWorkerCompleted += FinalWorkerCompleted;
                    finalWorker.RunWorkerAsync(this);
                }
            }
        }

        /// <summary>
        /// Invoked when a BackgroundWorker begins working.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var rowData = e.Argument as GridRow;
            e.Result = rowData;
            runningCount++;
            isRunning = true;
            ProcessManager.ProcessRow(rowData, dataGridView);
        }

        /// <summary>
        /// Invoked when a BackgroundWorker reports a change in progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Redraw rows
            dataGridView.Invalidate();
        }

        /// <summary>
        /// Invoked when a BackgroundWorker completes its task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GridRow row;
            runningCount--;
            if (e.Cancelled || e.Error != null)
            {
                if (e.Error is CancelledException)
                {
                    row = ((CancelledException)e.Error).Row;
                    if (e.Error.InnerException != null)
                    {
                        row.Status = e.Error.InnerException.Message;
                        Console.WriteLine(row.Status);
                    }

                    if (row.State == RowState.ClearOnComplete)
                    {
                        gridBindingSource.Remove(row);
                    }
                    else
                    {
                        row.State = RowState.Ready;
                        row.ButtonText = "Parse";
                    }
                }
                else
                {
                    Console.WriteLine("An Error Has occurred completing task", e);
                }
            }
            else
            {
                row = (GridRow)e.Result;
                if (row.State == RowState.ClearOnComplete)
                {
                    gridBindingSource.Remove(row);
                }
                else
                {
                    row.ButtonText = "Open";
                    row.State = RowState.Complete;
                }
            }
            dataGridView.Invalidate();
            RunNextWorker();
        }

        private void FinalWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // Process Logs and Generate Report.
            ProcessManager.GenerateFile(finalWorker);
        }

        private void FinalWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Report Status.
        }

        private void FinalWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isRunning = false;
            btnParse.Enabled = true;
            btnClear.Enabled = true;
            btnCancel.Enabled = false;
        }

        /// <summary>
        /// Runs a worker.
        /// </summary>
        /// <param name="row"></param>
        private void RunWorker(GridRow row)
        {
            btnClear.Enabled = false;
            btnParse.Enabled = false;
            btnCancel.Enabled = true;
            
            row.Status = "Waiting for a thread";
            row.State = RowState.Pending;
            row.Run();
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            queue.Clear();
            ProcessManager.CompletedLogs = new System.Collections.Concurrent.ConcurrentBag<ParsedLog>();
            if (logFiles.Count > 0)
            {
                btnParse.Enabled = false;
                btnCancel.Enabled = true;

                foreach (GridRow row in gridBindingSource)
                {
                    if (!row.BgWorker.IsBusy)
                    {
                        RunWorker(row);
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            var rows = new HashSet<GridRow>(queue);
            queue.Clear();

            foreach (GridRow row in gridBindingSource)
            {
                if (row.State == RowState.Pending)
                {
                    row.State = RowState.Ready;
                }

                if (row.BgWorker.IsBusy)
                {
                    row.Cancel();
                } else if (rows.Contains(row))
                {
                    row.Status = "Ready to parse";
                }
                dataGridView.Invalidate();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            btnCancel.Enabled = false;
            btnParse.Enabled = false;

            //Clear the queue so that cancelled workers don't invoke queued workers
            queue.Clear();
            logFiles.Clear();

            for (int i = gridBindingSource.Count - 1; i >= 0; i--)
            {
                var row = gridBindingSource[i] as GridRow;
                if (row.BgWorker.IsBusy)
                {
                    row.Cancel();
                    row.State = RowState.ClearOnComplete;
                }
                else
                {
                    gridBindingSource.RemoveAt(i);
                }
            }
        }

        private void btnRefreshAPI_Click(object sender, EventArgs e)
        {
            btnRefreshAPI.Enabled = false;
            ProcessManager.apiController.WriteAPISkillsToFile(ProcessManager.SkillAPICacheLocation);
            ProcessManager.apiController.WriteAPISpecsToFile(ProcessManager.SpecAPICacheLocation);
            ProcessManager.apiController.WriteAPITraitsToFile(ProcessManager.TraitAPICacheLocation);
            btnRefreshAPI.Enabled = true;
            MessageBox.Show("API cache has been refreshed");
        }
    }
}
