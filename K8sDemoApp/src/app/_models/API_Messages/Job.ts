import { JobStatusEnum } from "src/app/_enum/JobStatusEnum";
import { JobTypeEnum } from "src/app/_enum/JobTypeEnum";

export interface Job{
    jobId: number;
    creationDate:Date;
    startDate:Date;
    endDate:Date;
    status:JobStatusEnum;
    description:string;
    errors:string;
    jobType:JobTypeEnum;
}