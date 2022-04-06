using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace K8sBackendShared.Workers
{
    public class CyclicWorker
    {
        private int _cycleTime = 500;
        private Action _doWorkAction = null;

        private BackgroundWorker _bw = new BackgroundWorker();
        public CyclicWorker(int cycleTime, Action doWorkAction)
        {
            _cycleTime = cycleTime;
            _bw.WorkerReportsProgress = false;
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += BackgroundWorkerOnDoWork;
            //_bw.ProgressChanged += BackgroundWorkerOnProgressChanged;
            _bw.RunWorkerCompleted += BackgroundWorkerOnWorkCompleted;
            _doWorkAction = doWorkAction;
            _bw.RunWorkerAsync();
        }

        private void BackgroundWorkerOnWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Run again after previous run is complete
            _bw.RunWorkerAsync();
        }

        //private void BackgroundWorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    object userObject = e.UserState;
        //    int percentage = e.ProgressPercentage;
            //Report progress action
        //}

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            while (!_bw.CancellationPending)
            {
                Thread.Sleep(_cycleTime);   // If you need to make a pause between runs
                //Console.WriteLine("Worker run started");
                //Do Work!
                _doWorkAction();
                //Console.WriteLine("Worker run completed");
            } 
        
        }

        public void CancelWork()
        {
            _bw.CancelAsync();
            Console.WriteLine("Job Aborted");
        }
    }
}