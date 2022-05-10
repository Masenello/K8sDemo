import { DateEditTemplateComponent } from "@generic-ui/ngx-grid/composition/core/domain-read/edit/template/date-edit-template.component";
import { JobTypeEnum } from "src/app/_enum/JobTypeEnum";

export interface DirectorStatusMessage{
    timestamp: Date,
    registeredWorkers: Array<WorkerDescriptor>,
    totalJobs:number,
    maxConcurrentTasks:number,
    maxWorkers:number,
    maxOpenJobDuration:number,
}

export interface WorkerDescriptor{
    workerId:string,
    currentJobs:number
}

