import { JobStatusEnum } from "../../_enum/JobStatusEnum";

export interface JobStatusMessage{
    jobId: string;
    jobType:number;
    status:JobStatusEnum;
    user:string;
    progressPercentage:number;
    userMessage:string;
    endDate:Date;
}