export class SetDirectorParametersRequest{
    maxJobsPerWorker: number;
    maxWorkers:number;
    scalingEnabled:boolean;
    idleSecondsBeforeScaleDown:number;
}