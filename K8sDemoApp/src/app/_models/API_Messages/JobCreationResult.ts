import { JobStatusEnum } from "src/app/_enum/JobStatusEnum";
import { JobTypeEnum } from "src/app/_enum/JobTypeEnum";

export interface JobCreationResult{
    jobId: string;
    creationTime:Date;
    user:string;
    userMessage:string;
    jobType: JobTypeEnum;
    jobStatus: JobStatusEnum;
}