import { JobTypeEnum } from "src/app/_enum/JobTypeEnum";

export interface DirectorStatusMessage{
    registeredWorkers: Array<WorkerDescriptor>,
    jobsList: Array<JobAvailableCount>,
}

export interface WorkerDescriptor{
    workerJobType: JobTypeEnum,
    workerId:string,
}

export interface JobAvailableCount{
    jobType: JobTypeEnum,
    jobCount:Number,
}
