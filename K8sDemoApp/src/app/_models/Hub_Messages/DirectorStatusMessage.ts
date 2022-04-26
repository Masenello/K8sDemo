import { DateEditTemplateComponent } from "@generic-ui/ngx-grid/composition/core/domain-read/edit/template/date-edit-template.component";
import { JobTypeEnum } from "src/app/_enum/JobTypeEnum";

export interface DirectorStatusMessage{
    timestamp: Date,
    registeredWorkers: Array<WorkerDescriptor>,
    jobsList: Array<JobAvailableCount>,
}

export interface WorkerDescriptor{
    workerJobType: JobTypeEnum,
    workerId:string,
}

export interface JobAvailableCount{
    jobType: JobTypeEnum,
    jobCount:number,
}
