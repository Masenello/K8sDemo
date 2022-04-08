using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using K8sBackendShared.Jobs;
using K8sBackendShared.Messages;
using K8sBackendShared.Settings;

namespace K8sBackendShared.Workers
{
    public class CyclicWorker
    {
        //Defines pause between consecutive _workerJob.DoWork runs
        private int _cycleTime = 500;


        private AbstractWorkerJob _workerJob = null;

        //Used to report progress through Rabbit messages
        private IBus _rabbitBus = RabbitHutch.CreateBus(NetworkSettings.RabbitHostResolver());

        
        private BackgroundWorker _bw = new BackgroundWorker();
        public CyclicWorker(int cycleTime, AbstractWorkerJob workerJob)
        {
            _cycleTime = cycleTime;
            _bw.WorkerReportsProgress = false;
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += BackgroundWorkerOnDoWork;
            _bw.RunWorkerCompleted += BackgroundWorkerOnWorkCompleted;

            _workerJob = workerJob;
            _workerJob.JobProgressChanged += new AbstractWorkerJob.JobProgressChangedHandler(JobProgressChanged);

            _bw.RunWorkerAsync();
        }

        private void JobProgressChanged(object sender, JobProgressEventArgs e)
        {
            _rabbitBus.PubSub.Publish(e.Status);
            Console.WriteLine($"{DateTime.Now}: Job Id:{e.Status.JobId} Status: {e.Status.Status} Progress: {e.Status.ProgressPercentage}% ");
        }

        private void BackgroundWorkerOnWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Run again after previous run is complete
            _bw.RunWorkerAsync();
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            while (!_bw.CancellationPending)
            {
                Thread.Sleep(_cycleTime);   // If you need to make a pause between runs
                //Console.WriteLine("Worker run started");
                //Do Work!
                _workerJob.DoWork();
                //Console.WriteLine("Worker run completed");
            } 
        
        }


        //TODO complete and test!
        public void CancelWork()
        {
            _bw.CancelAsync();
            Console.WriteLine("Job Aborted");
        }
    }
}