import { JobStatusEnum } from "src/app/_enum/JobStatusEnum";
import { JobTypeEnum } from "src/app/_enum/JobTypeEnum";

export interface TestJobCreationResult{
    jobId: number;
    creationTime:Date;
    user:string;
    userMessage:string;
    jobType: JobTypeEnum;
    jobStatus: JobStatusEnum;
}