import { JobStatusEnum } from "../../_enum/JobStatusEnum";

export interface JobStatusMessage{
    jobId: number;
    jobType:number;
    status:JobStatusEnum;
    user:string;
    progressPercentage:number;
    userMessage:string;
}